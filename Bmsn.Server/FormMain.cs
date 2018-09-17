using System;
using System.Windows.Forms;
using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Main application form for server
    /// </summary>
    public partial class FormMain : Form
	{
        #region Types

        /// <summary>
        /// An example operation for custom server command
        /// </summary>
        private class Cmd_Test : CommandOperation
        {
            /// <summary>
            /// Initialize a new instance of this class
            /// </summary>
			public Cmd_Test()
            {
                SecurityLevel = CommandSecurityLevel.All;
            }

            /// <summary>
            /// Applies operation
            /// </summary>
			protected override void DoOperation()
            {
                Response = new NetResponse(true, "Hi hash: " + Client.HashCode.ToString());
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public FormMain()
		{
			InitializeComponent();
		}

        #endregion

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            btnStart.PerformClick();
        }

        #region Private Methods

        /// <summary>
        /// Starts server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
		{
			server.Start();
		}

        /// <summary>
        /// Stops server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnStop_Click(object sender, EventArgs e)
		{
			server.Stop();
			lvwUsers.Items.Clear();
		}

        /// <summary>
        /// Handles server is started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void server_Started(object sender, EventArgs e)
		{
			btnStart.Enabled = false;
			btnStop.Enabled = true;
			SetText("Server started");
		}

        /// <summary>
        /// Handles server is stopped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void server_Stopped(object sender, EventArgs e)
		{
			btnStop.Enabled = false;
			btnStart.Enabled = true;

			SetText("Server stopped");
		}

        /// <summary>
        /// Sets server status label text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void SetLabelText(object sender, EventArgs e)
		{
			lblStatus.Text = sender.ToString();
		}

        /// <summary>
        /// Sets server status label text in a safety manner
        /// </summary>
        /// <param name="text"></param>
		private void SetText(string text)
		{
			Invoke(new EventHandler(SetLabelText), new object[] { text, null});
		}

        /// <summary>
        /// Handles a server client is disconnected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void server_ClientDisconnected(object sender, ServerClientEventArgs e)
		{
			Invoke(new EventHandler(ClientDisconnected), new object[] {e.Client, null });
		}

        /// <summary>
        /// Applies client disconnection operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void ClientDisconnected(object sender, EventArgs e)
		{
			ServerClient client = sender as ServerClient;
			if (client == null)
				return;

			for (int i = 0; i < lvwUsers.Items.Count; i++)
			{
				if (lvwUsers.Items[i].Tag == client)
				{
					lvwUsers.Items.RemoveAt(i);
					return;
				}
			}
		}

        /// <summary>
        /// Handles a client is connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void server_ClientConnected(object sender, ServerClientEventArgs e)
		{

			Invoke(new EventHandler(ClientConnected), new object[] { e.Client, null});
		}

        /// <summary>
        /// Applies client connection operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientConnected(object sender, EventArgs e)
		{
			ServerClient client = sender as ServerClient;
			if (sender == null)
				return;

			ListViewItem item = new ListViewItem();
			
			lvwUsers.Items.Add(item);
			SetItemProperties(item, client);
		}

        /// <summary>
        /// Applies and shown client properties in the client list
        /// </summary>
        /// <param name="item"></param>
        /// <param name="client"></param>
        private void SetItemProperties(ListViewItem item, ServerClient client)
		{
			item.Tag = client;
			item.SubItems.Clear();
			switch (client.Status)
			{ 
				case ClientStatus.Active:
					item.ImageIndex = 2;
					break;
				case ClientStatus.OutToLunch:
					item.ImageIndex = 1;
					break;
				case ClientStatus.StandBy:
					item.ImageIndex = 0;
					break;
				case ClientStatus.Waiting:
					item.ImageIndex = 3;
					break;
			}
			item.Text = client.Socket.RemoteEndPoint.ToString();
			item.SubItems.Add(client.ConnectionTime.ToString());
			item.SubItems.Add(client.Username);
			item.SubItems.Add(client.Status.ToString());
		}

        /// <summary>
        /// Applies client status changed operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientstatusChanged(object sender, EventArgs e)
		{
			ServerClient client = sender as ServerClient;
			if (client == null)
				return;

			for (int i = 0; i < lvwUsers.Items.Count; i++)
			{
				if (lvwUsers.Items[i].Tag == client)
				{
					SetItemProperties(lvwUsers.Items[i], client);
					return;
				}
			}
		}

        /// <summary>
        /// Handles client status changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void server_ClientStatusChanged(object sender, ServerClientEventArgs e)
		{
			Invoke(new EventHandler(ClientstatusChanged), new object[] { e.Client, null});
		}
        
		/// <summary>
		/// Handles server received a new command
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void server_NewCommand(object sender, CommandOperationEventArgs e)
		{
			SetText("Command Received: " + e.Command.Name);
			if (e.Command.Name == "TEST")
			{
				// custom operation by handling New Command event
				e.Operation = new Cmd_Test();
				e.Handled = true;
			}
		}

        /// <summary>
        /// An unhandled error occured on the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void server_SystemErrorOccured(object sender, SystemErrorEventArgs e)
		{
            MessageBox.Show(e.Exception.ToString(), "Error");
		}

        #endregion
    }
}