using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Client
{
    public partial class Client : Form
    {
        //Login LoginForm;

        private string UserName = "Anonymous";
        private StreamWriter swSender;
        private StreamReader srReceiver;
        private TcpClient tcpServer;
        private delegate void UpdateLogCallback(string strMessage);      // Needed to update the form with messages from another thread
        private delegate void CloseConnectionCallback(string strReason); // Needed to set the form to a "disconnected" state from another thread
        private Thread thrMessaging;
        private IPAddress ip;
        private bool connected;

        public Client()
        {
            // On application exit, don't forget to disconnect first
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
        }

        private void InitializeConnection()
        {
            try
            {
                ip = IPAddress.Parse(txtIP.Text);   // Parse the IP address from the TextBox into an IPAddress object
                tcpServer = new TcpClient();        // Start a new TCP connections to the chat server
                tcpServer.Connect(ip, 1986);

                connected = true;                   // Helps us track whether we're connected or not
                UserName = txtUser.Text;

                btnSend.Enabled = true;
                txtMessage.Enabled = true;

                // Send the desired username to the server
                swSender = new StreamWriter(tcpServer.GetStream());
                swSender.WriteLine(txtUser.Text);
                swSender.Flush();

                // Start the thread for receiving messages and further communication
                thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
                thrMessaging.Start();
            }
            catch
            {
                MessageBox.Show("Could not establish connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveMessages()
        {
            srReceiver = new StreamReader(tcpServer.GetStream());   // Receive the response from the server

            // If the first character of the response is 1, connection was successful
            string ConResponse = srReceiver.ReadLine();
            if (ConResponse[0] == '1')
            {
                try
                {

                    // Update the form to tell it we are now connected
                    this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { "Connected Successfully!" });
                }
                catch { }
            }
            else
            { // If the first character is not a 1 (probably a 0), the connection was unsuccessful
                string Reason = "Not Connected: ";
                // Extract the reason out of the response message. The reason starts at the 3rd character
                Reason += ConResponse.Substring(2, ConResponse.Length - 2);
                // Update the form with the reason why we couldn't connect
                this.Invoke(new CloseConnectionCallback(this.CloseConnection), new object[] { Reason });
                // Exit the method
                return;
            }

            // While we are successfully connected, read incoming lines from the server
            while (connected)
            {
                try
                {
                    // Show the messages in the log TextBox
                    this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { srReceiver.ReadLine() });
                }
                catch { }
            }
        }

        private void UpdateLog(string strMessage)
        {
            try
            {
                txtLog.AppendText(strMessage + "\r\n"); // Append text also scrolls the TextBox to the bottom each time
            }
            catch { }
        }

        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (connected == true)
            {
                // Closes the connections, streams, etc.
                connected = false;
                swSender.Close();
                srReceiver.Close();
                tcpServer.Close();
            }
        }


        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connected = false;
            swSender.Close();
            srReceiver.Close();
            tcpServer.Close();
            btnSend.Enabled = false;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chat application" + Environment.NewLine + "steve@mycosmos.gr", "About");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)  // If the key is Enter
            {
                SendMessage();
            }
        }

        // Sends the typed message to the server
        private void SendMessage()
        {
            if (txtMessage.Lines.Length >= 1)
            {
                swSender.WriteLine(txtMessage.Text);
                swSender.Flush();
                txtMessage.Lines = null;
            }
            txtMessage.Text = "";
            txtLog.AppendText(txtMessage.Text);
        }

        // Closes a current connection
        public void CloseConnection(string Reason)
        {
            // Show the reason why the connection is ending
            txtLog.AppendText(Reason + "\r\n");
            // Enable and disable the appropriate controls on the form
            txtIP.Enabled = true;
            txtUser.Enabled = true;
            txtMessage.Enabled = false;
            btnSend.Enabled = false;
            //btnConnect.Text = "Connect";

            // Close the objects
            connected = false;
            swSender.Close();
            srReceiver.Close();
            tcpServer.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (connected == false)
            {
                InitializeConnection();
            }
            else
            {
                CloseConnection("Disconnected at user's request.");
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Chat Application" + Environment.NewLine + "steve@mycosmos.gr", "About");
            new AboutBox().Show();
        }

    }
}
