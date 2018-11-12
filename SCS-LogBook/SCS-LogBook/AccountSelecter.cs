using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using NLog;
using SCS_LogBook.Objects;
using Logger = NLog.Logger;

namespace SCS_LogBook
{
    public partial class AccountSelecter : Form {
        internal static string ConnectionString = "Data/accounts.lite";
        internal static string DBAccountName = "Accounts_V1";
        private List<Account> _accounts;
        private LiteDatabase _accountDb;
        private NewUser nu = new NewUser();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        
        public AccountSelecter()
        {
            // Test Translations
            //CultureInfo.CurrentUICulture = new CultureInfo("de");
            Log.Debug("Load Account Selector");
            InitializeComponent();
            


        }

        private void button2_Click(object sender, EventArgs e) {
            nu.Closed += Nu_Closed;
            nu.ShowDialog();
          
           
        }

        private void Nu_Closed(object sender, EventArgs e) {
            if (!(sender is NewUser nu))
            {
                return;
            }
            if (nu.DialogResult == DialogResult.OK)
            {
                 
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

        private void AccountSelecter_Load(object sender, EventArgs e)
        {
            Log.Debug("Started. Check FirstStart");
            if (Properties.Settings.Default.firstStart)
            {
                Log.Debug("First Start of the Application. Show FirstStart Information");
                new First_start().ShowDialog();
                Properties.Settings.Default.firstStart = false;
                Properties.Settings.Default.Save();
            }
            ReloadAccounts();
        }

        private void ReloadAccounts() {

            Log.Debug("Load saved profiles");
            _accountDb = new LiteDatabase(ConnectionString);
            var store = _accountDb.GetCollection<Account>(DBAccountName);
            _accounts = store.FindAll().ToList();

            Log.Debug("Successful loaded: {0}",_accounts.Count);
            ReloadAccountList();
        }

        private void ReloadAccountList() {
            listView1.Items.Clear();
            foreach (var account in _accounts) {
                var tempLvi = new ListViewItem(account.Name);
                tempLvi.SubItems.Add(account.PlayTime.ToString(CultureInfo.CurrentCulture));
                tempLvi.SubItems.Add(account.InGameTime.ToString(CultureInfo.CurrentCulture));
                tempLvi.SubItems.Add(account.Miles.ToString(CultureInfo.CurrentCulture));
                tempLvi.SubItems.Add(account.Game.ToString());
                listView1.Items.Add(tempLvi);
            }

            Log.Debug("Filled list with profiles");
            ResizeListViewColumns(listView1);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count==0) {
                button1.Enabled = false;
            } else {
                if (listView1.SelectedIndices[0] == -1) {
                    button1.Enabled = false;
                }
                button1.Enabled = true;
            }
        }
        private void ResizeListViewColumns(ListView lv)
        {
            if (lv.InvokeRequired)
            {
                lv.Invoke(
                          (MethodInvoker)(() => {
                                              foreach (ColumnHeader column in lv.Columns)
                                              {
                                                  column.Width = -2;
                                              }
                                          }));
            }
            else
            {
                foreach (ColumnHeader column in lv.Columns)
                {
                    column.Width = -2;
                }
            }
        }

        private void CreateUserFiles(Account data) {

            Directory.CreateDirectory("Data/" + data.Name);

            Log.Info("Created User Directory");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var account in _accounts) {
                if (!account.Name.Equals(listView1.SelectedItems[0].Text)) {
                    continue;
                }

                var logBook = new Thread(()=> Application.Run(new LogBook(account)));
                logBook.SetApartmentState(ApartmentState.STA);
                logBook.Start();

                Log.Debug("Selected profile found. Start logbook and hide accountselector.");
            }

           Hide();
        }
    }
}
