using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using LiteDB;
using NLog;
using SCS_LogBook.Objects;
using SCS_LogBook.Properties;
using Logger = NLog.Logger;

namespace SCS_LogBook {
    /// <summary>
    ///     Display small gui to select accounts
    /// </summary>
    public partial class AccountSelector : Form {
        /// <summary>
        ///     Connection string to access the database
        /// </summary>
        internal static string ConnectionString = "Data/accounts.lite";

        /// <summary>
        ///     Name of the Database which contains the accounts
        /// </summary>
        internal static string DBAccountName = "Accounts_V1";

        /// <summary>
        ///     static logger object to handle log events for this class
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     New User Object
        /// </summary>
        private readonly NewUser nu = new NewUser();

        /// <summary>
        ///     LiteDatabase object to access the database
        /// </summary>
        private LiteDatabase _accountDb;

        /// <summary>
        ///     Object with all accounts
        /// </summary>
        private List<Account> _accounts;

        /// <summary>
        ///     Init the account selector gui
        /// </summary>
        public AccountSelector() {
            // Test Translations
            // CultureInfo.CurrentUICulture = new CultureInfo("de");
            Log.Debug("Load Account Selector");
            InitializeComponent();
        }

        /// <summary>
        ///     Handle the click on the Button Create New user.
        /// </summary>
        /// <param name="sender">The buttons that was clicked</param>
        /// <param name="e">The event args</param>
        private void B_createNew_Click(object sender, EventArgs e) => DisplayCreateNewUserDialog();

        /// <summary>
        ///     Show the dialog, that lets the user to create a new account and handle every thing around that.
        /// </summary>
        private void DisplayCreateNewUserDialog() {
            nu.Closed += Nu_Closed;
            nu.ShowDialog();
        }


        /// <summary>
        ///     Handle the closing event of the create new user dialog.
        ///     Create new user if the user wants to add some or discard everything and close dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nu_Closed(object sender, EventArgs e) {
            if (!(sender is NewUser nu)) {
                return;
            }

            if (nu.DialogResult == DialogResult.OK) {
                var data = nu.newUser;
                var store = _accountDb.GetCollection<Account>(DBAccountName);
                store.Insert(data);
                ReloadAccounts();
                MessageBox.Show(accountSelecterCode.AccountSelecter_button2_Click_User_succsesfull_added);
                nu.ResetFields();
                CreateUserFiles(data);
            }

            nu.Closed -= Nu_Closed;
        }

        /// <summary>
        ///     Handle some stuff before the gui is init
        ///     <list type="bullet"^>
        ///         <listheader>
        ///             <term>stuff</term><description>description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>First Start</term><description>Show special first start dialog on first start</description>
        ///         </item>
        ///         <item>
        ///             <term>Load Accounts</term><description>Reload accounts so the list is init with real values</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountSelector_Load(object sender, EventArgs e) {
            Log.Debug("Started. Check FirstStart");
            if (Settings.Default.firstStart) {
                Log.Debug("First Start of the Application. Show FirstStart Information");
                new First_start().ShowDialog();
                Settings.Default.firstStart = false;
                Settings.Default.Save();
            }

            ReloadAccounts();
        }

        /// <summary>
        ///     Reload the list of accounts.
        /// </summary>
        private void ReloadAccounts() {
            Log.Debug("Load saved profiles");
            _accountDb = new LiteDatabase(ConnectionString);
            var store = _accountDb.GetCollection<Account>(DBAccountName);
            _accounts = store.FindAll().ToList();

            Log.Debug("Successful loaded: {0}", _accounts.Count);
            ReloadAccountList();
        }

        /// <summary>
        ///     Reload the account list on the gui.
        /// </summary>
        private void ReloadAccountList() {
            lv_accountList.Items.Clear();
            foreach (var account in _accounts) {
                var tempLvi = new ListViewItem(account.Name);
                tempLvi.SubItems.Add(account.PlayTime.ToString(CultureInfo.CurrentCulture));
                tempLvi.SubItems.Add(account.InGameTime.ToString(CultureInfo.CurrentCulture));
                tempLvi.SubItems.Add(account.Miles.ToString(CultureInfo.CurrentCulture));
                tempLvi.SubItems.Add(account.Game.ToString());
                lv_accountList.Items.Add(tempLvi);
            }

            Log.Debug("Filled list with profiles");
            ResizeListViewColumns(lv_accountList);
        }

        /// <summary>
        ///     Handle selected index changed of the account list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lv_accountList_SelectedIndexChanged(object sender, EventArgs e) {
            if (lv_accountList.SelectedIndices.Count == 0) {
                b_loadSelectedAccount.Enabled = false;
            } else {
                if (lv_accountList.SelectedIndices[0] == -1) {
                    b_loadSelectedAccount.Enabled = false;
                }

                b_loadSelectedAccount.Enabled = true;
            }
        }

        // TODO: Extension: Convert this function to an extension
        /// <summary>
        ///     Resize the columns of an list view.
        /// </summary>
        /// <param name="lv"></param>
        private static void ResizeListViewColumns(ListView lv) {
            if (lv.InvokeRequired) {
                lv.Invoke((MethodInvoker) (() => {
                                               foreach (ColumnHeader column in lv.Columns) {
                                                   column.Width = -2;
                                               }
                                           }));
            } else {
                foreach (ColumnHeader column in lv.Columns) {
                    column.Width = -2;
                }
            }
        }
        // TODO: Static?: Other position for this function

        /// <summary>
        ///     Create files for a new user.
        /// </summary>
        /// <param name="data"></param>
        private static void CreateUserFiles(Account data) {
            Directory.CreateDirectory("Data/" + data.Name);
            Log.Info("Created User Directory");
        }

        /// <summary>
        ///     Handle click of the load selected account button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_loadSelectedAccount_Click(object sender, EventArgs e) => LoadSelectedAccount();

        /// <summary>
        ///     Load the selected account in the account list.
        /// </summary>
        private void LoadSelectedAccount() {
            foreach (var account in _accounts) {
                if (!account.Name.Equals(lv_accountList.SelectedItems[0].Text)) {
                    continue;
                }

                var logBook = new Thread(() => Application.Run(new LogBook(account)));
                logBook.SetApartmentState(ApartmentState.STA);
                logBook.Start();

                Log.Debug("Selected profile found. Start logbook and hide accountselector.");
            }

            Hide();
        }
    }
}