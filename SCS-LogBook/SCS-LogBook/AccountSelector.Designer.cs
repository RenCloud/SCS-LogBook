namespace SCS_LogBook
{
    partial class AccountSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountSelector));
            this.lv_accountList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.b_loadSelectedAccount = new System.Windows.Forms.Button();
            this.b_createNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lv_accountList
            // 
            this.lv_accountList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            resources.ApplyResources(this.lv_accountList, "lv_accountList");
            this.lv_accountList.FullRowSelect = true;
            this.lv_accountList.MultiSelect = false;
            this.lv_accountList.Name = "lv_accountList";
            this.lv_accountList.UseCompatibleStateImageBehavior = false;
            this.lv_accountList.View = System.Windows.Forms.View.Details;
            this.lv_accountList.SelectedIndexChanged += new System.EventHandler(this.Lv_accountList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // b_loadSelectedAccount
            // 
            resources.ApplyResources(this.b_loadSelectedAccount, "b_loadSelectedAccount");
            this.b_loadSelectedAccount.Name = "b_loadSelectedAccount";
            this.b_loadSelectedAccount.UseVisualStyleBackColor = true;
            this.b_loadSelectedAccount.Click += new System.EventHandler(this.B_loadSelectedAccount_Click);
            // 
            // b_createNew
            // 
            resources.ApplyResources(this.b_createNew, "b_createNew");
            this.b_createNew.Name = "b_createNew";
            this.b_createNew.UseVisualStyleBackColor = true;
            this.b_createNew.Click += new System.EventHandler(this.B_createNew_Click);
            // 
            // AccountSelector
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.b_createNew);
            this.Controls.Add(this.b_loadSelectedAccount);
            this.Controls.Add(this.lv_accountList);
            this.MaximizeBox = false;
            this.Name = "AccountSelector";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.AccountSelector_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lv_accountList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button b_loadSelectedAccount;
        private System.Windows.Forms.Button b_createNew;
    }
}

