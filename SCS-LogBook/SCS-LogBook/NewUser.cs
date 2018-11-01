using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;
using SCS_LogBook.Objects;


namespace SCS_LogBook
{
    public partial class NewUser : Form { 
        public NewUser( ) {
            InitializeComponent();
            comboBox1.DataSource = Enum.GetValues(typeof(SCSGame));
            comboBox1.SelectedIndex = 0;
        }
        internal Account newUser {
            get {
                Enum.TryParse<SCSGame>(comboBox1.SelectedValue.ToString(), out var game);
                return new Account(tb_username.Text, game, 0d,0d,0d );
            }
    }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tb_username.Text.Length==0) {
                MessageBox.Show("No user name given", "Information");

                return;
            }
            DialogResult = DialogResult.OK;
            Close();
            
        }

        public void ResetFields() {
            comboBox1.SelectedIndex = 0;
            tb_username.Clear();
        }

    }
}
