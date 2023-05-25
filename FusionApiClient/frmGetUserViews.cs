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
    public partial class frmGetUserViews : Form
    {
        public HttpClient Client { get; set; }
        public string Email { get; set; }
        public frmGetUserViews(HttpClient client, string email)
        {
            InitializeComponent();
            Client = client;
            Email = email;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var x = Client;
            var xx = Email;
        }
    }
}
