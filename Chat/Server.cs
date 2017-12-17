using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace Chat
{
    public partial class Server : Form
    {
        //update the txtLog TextBox from another thread
        private delegate void UpdateStatusCallback(string strMessage);
     
        public Server()
        {
            InitializeComponent();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chat application" + Environment.NewLine + "steve@mycosmos.gr", "About");
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {

            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");      // Parse the server's IP address out of the TextBox
            ChatServer mainServer = new ChatServer(ipAddr);     // Create a new instance of the ChatServer object
            ChatServer.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged); // Hook the StatusChanged event handler to mainServer_StatusChanged

            mainServer.StartListening();    // Start listening for connections
            txtLog.AppendText("Monitoring for connections...\r\n");
        }

        public void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // Call the method that updates the form
            this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
        }

        private void UpdateStatus(string strMessage)
        {
            // Updates the log with the message
            txtLog.AppendText(strMessage + "\r\n");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chat application" + Environment.NewLine + "steve@mycosmos.gr", "About");

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");      // Parse the server's IP address out of the TextBox
            ChatServer mainServer = new ChatServer(ipAddr);     // Create a new instance of the ChatServer object
            ChatServer.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged); // Hook the StatusChanged event handler to mainServer_StatusChanged

            mainServer.StartListening();    // Start listening for connections
            txtLog.AppendText("Listening for connections...\r\n");
        }
  
    }
}
