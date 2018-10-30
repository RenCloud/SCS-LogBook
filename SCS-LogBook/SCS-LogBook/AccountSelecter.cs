using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS_LogBook
{
    public partial class AccountSelecter : Form
    {
        public AccountSelecter()
        {
            // Test Translations
            //CultureInfo.CurrentUICulture = new CultureInfo("de");
            InitializeComponent();
            new First_start().ShowDialog();
        }
    }
}
