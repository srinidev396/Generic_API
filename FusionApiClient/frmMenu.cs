using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FusionApiClient
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
            lblWarning1.ForeColor = Color.Red;
        }

        private async void lblGetUserViews_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var client = await GetToken.GetClient(txtUserName.Text, txtPassword.Text, txtDataBase.Text, txtUrl.Text);
            var frm = new frmGetUserViews(client, txtEmail.Text);
            frm.Show();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            txtUserName.Text = Properties.Settings.Default.username;
            txtPassword.Text = Properties.Settings.Default.password;
            txtDataBase.Text = Properties.Settings.Default.databasename;
            txtUrl.Text = Properties.Settings.Default.url;
            txtEmail.Text = Properties.Settings.Default.email;
        }

        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.username = txtUserName.Text;
            Properties.Settings.Default.password = txtPassword.Text;
            Properties.Settings.Default.databasename = txtDataBase.Text;
            Properties.Settings.Default.url = txtUrl.Text;
            Properties.Settings.Default.email = txtEmail.Text;
            Properties.Settings.Default.Save();
        }
    }
}
