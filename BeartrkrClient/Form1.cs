using System;
using System.Threading;
using System.Windows.Forms;

namespace BeartrkrClient
{
    public partial class Form1 : Form
    {
        const string VALID_KEY = "Client key valid; providing status updates as usual.";
        const string INVALID_KEY = "Invalid or missing client key.";

        public Form1()
        {
            InitializeComponent();
            Resize += Form1_Resize;

            Utilities.AddToStartup();

            TrackerThread tt = new TrackerThread(); // pronounced "titty"
            Thread theThread = new Thread(tt.DoAllTheThings);
            theThread.Start();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Application.UserAppDataRegistry.GetValue("ClientKey") != null) txtClientKey.Text = (string)Application.UserAppDataRegistry.GetValue("ClientKey");
            lblStatus.Text = Utilities.ValidateClientKey() ? VALID_KEY : INVALID_KEY;
            Thread.Sleep(1000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtClientKey.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Utilities.ValidateClientKey(txtClientKey.Text))
            {
                Application.UserAppDataRegistry.SetValue("ClientKey", txtClientKey.Text);
                MessageBox.Show("Client key successfully updated!");
                txtClientKey.Enabled = false;
                lblStatus.Text = VALID_KEY;
            }
            else
            {
                MessageBox.Show("Invalid client key. Please check the key and try again.");
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmAbout().Show();
        }
    }
}
