using System;
using System.IO;
using System.Windows.Forms;
using Bmsn.Protocol;

namespace Bmsn.Client
{
    /// <summary>
    /// Represents Client main window
    /// </summary>
    public partial class FormMain : Form
	{
        #region Members

        private MessagingSessionsManager _Manager;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public FormMain()
		{
			InitializeComponent();

			client1.ImagePath = Properties.Settings.Default.ImagePath;

			_Manager = new MessagingSessionsManager(this);
			client1.CommandExecutionManager.Subscribe(
				CommandNames.LOGIN,
				new CommandExecuteHandler(OnLoginCommandExecuted));
			client1.ServerCommandsManager.Subscribe(
				CommandNames.U_LOGIN,
				new CommandExecuteHandler(OnUserConnect));
			client1.ServerCommandsManager.Subscribe(
				CommandNames.U_DISCONNECT,
				new CommandExecuteHandler(OnUserDisconnect));
			client1.ServerCommandsManager.Subscribe(
				CommandNames.U_STATUS_CHANGED,
				new CommandExecuteHandler(OnUserStatusChanged));
			client1.ServerCommandsManager.Subscribe(
				CommandNames.GET_PICTURE,
				new CommandExecuteHandler(OnGetPicture));
			client1.ServerCommandsManager.Subscribe(
				CommandNames.CLIENT_UPDATE_PICTURE,
				new CommandExecuteHandler(OnClientUpdatePicture));
		}

        #endregion

        #region Private Methods

        private void client1_SocketStatusChanged(object sender, EventArgs e)
		{
			BeginInvoke(new EventHandler(SetStatusChanges), new object[] { null, null });
		}

        private void SetStatusChanges(object sender, EventArgs e)
		{
			switch (client1.SocketStatus)
			{
				case SocketStatus.Connected:
					NetCommand command = new NetCommand(CommandNames.LOGIN);
					command.Parameters.Add(new ParameterString("un", txtUsername.Text));
					client1.Commands.Add(command);
					break;
				case SocketStatus.Connecting:
					pnlLoginScreen.Enabled = false;
					signOutToolStripMenuItem.Enabled = false;
					durumToolStripMenuItem.Enabled = false;
					break;
				case SocketStatus.Disconnected:
					lvwUsers.Visible = false;
					pnlLoginScreen.Enabled = true;
					pnlLoginScreen.Visible = true;
					signOutToolStripMenuItem.Enabled = false;
					break;
				case SocketStatus.Disconnecting:
					signOutToolStripMenuItem.Enabled = false;
					break;
				case SocketStatus.Error:
					MessageBox.Show("Error in Client ...");

					lvwUsers.Visible = false;
					pnlLoginScreen.Enabled = true;
					pnlLoginScreen.Visible = true;
					signOutToolStripMenuItem.Enabled = false;
					break;
			}
		}

		private void btnLogin_Click(object sender, EventArgs e)
		{
			if (txtUsername.Text == string.Empty)
			{
				MessageBox.Show("Please enter your username");
				txtUsername.Focus();
			}
			else
			{
				if (client1.SocketStatus != SocketStatus.Connected)
				{
					client1.Connect();
				}
				else
				{

					NetCommand command = new NetCommand(CommandNames.LOGIN);
					command.Parameters.Add(new ParameterString("un", txtUsername.Text));
					client1.Commands.Add(command);
				}
			}
		}

		private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			client1.Disconnect();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
			Dispose();
		}

        private void DoLoginOperation(object sender, EventArgs e)
		{
			NetResponse response = sender as NetResponse;
			if (response.Okey)
			{
				lvwUsers.Items.Clear();
				lvwUsers.Visible = true;
				pnlLoginScreen.Visible = false;
				signOutToolStripMenuItem.Enabled = true;
				durumToolStripMenuItem.Enabled = true;

				base.Text = "B-MSN by Ozcan DEGIRMENCI - " + txtUsername.Text;
			}
			else {
				MessageBox.Show(response.Message);
				pnlLoginScreen.Enabled = true;
				base.Text = "B-MSN by Ozcan DEGIRMENCI";
			}
		}

		private void OnLoginCommandExecuted(NetCommand command)
		{
			BeginInvoke(new EventHandler(DoLoginOperation),
				new object[] {command.Response, null});
		}

		private void OnUserConnect(NetCommand command)
		{
			Invoke(new EventHandler(InvokeUserConnect), new object[] { command, null});
		}

		private void InvokeUserConnect(object sender, EventArgs e)
		{
			NetCommand command = sender as NetCommand;
			if (command == null)
				return;

			ParameterInt32 hash = command.Parameters["hash"]
				as ParameterInt32;
			ParameterString un = command.Parameters["un"]
				as ParameterString;
			ParameterInt32 st = command.Parameters["st"]
				as ParameterInt32;

			ClientStatus cst = (ClientStatus)st.Value;

			ListViewItem item = new ListViewItem();
			switch (cst)
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
			item.ImageIndex = 2;
			item.Text = un.Value;
			item.Tag = hash.Value;

			lvwUsers.Items.Add(item);

			command.Response = new NetResponse(true);

			_Manager.Connected(un.Value);
		}

		private void OnUserDisconnect(NetCommand command)
		{
			Invoke(new EventHandler(InvokeUserDisconnect), new object[] { command, null});
		}

		private void InvokeUserDisconnect(object sender, EventArgs e)
		{
			NetCommand command = sender as NetCommand;
			if (command == null)
				return;

			ParameterInt32 hash = command.Parameters["hash"]
				as ParameterInt32;
			ParameterString un = command.Parameters["un"]
				as ParameterString;

			for (int i = 0; i < lvwUsers.Items.Count; i++)
			{
				if (hash.Value.Equals(lvwUsers.Items[i].Tag))
				{
					lvwUsers.Items.RemoveAt(i);
					break;
				}
			}

			_Manager.Disconnected(un.Value);

			command.Response = new NetResponse(true);
		}

		private void OnUserStatusChanged(NetCommand command)
		{
			Invoke(new EventHandler(InvokeUserStatusChanged), new object[] { command, null});
		}

		private void InvokeUserStatusChanged(object sender, EventArgs e)
		{
			NetCommand command = sender as NetCommand;
			if (command == null)
				return;

			ParameterInt32 hash = command.Parameters["hash"]
				as ParameterInt32;
			ParameterInt32 st = command.Parameters["st"]
				as ParameterInt32;
			ClientStatus cst = (ClientStatus)st.Value;
			for (int i = 0; i < lvwUsers.Items.Count; i++)
			{
				if (hash.Value.Equals(lvwUsers.Items[i].Tag))
				{
					switch (cst)
					{
						case ClientStatus.Active:
							lvwUsers.Items[i].ImageIndex = 2;
							break;
						case ClientStatus.OutToLunch:
							lvwUsers.Items[i].ImageIndex = 1;
							break;
						case ClientStatus.StandBy:
							lvwUsers.Items[i].ImageIndex = 0;
							break;
						case ClientStatus.Waiting:
							lvwUsers.Items[i].ImageIndex = 3;
							break;
					}
					break;
				}
			}

			command.Response = new NetResponse(true);
		}

		private void aktifToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeStatus(ClientStatus.Active);
		}

		private void ChangeStatus(ClientStatus st)
		{
			NetCommand command = new NetCommand(CommandNames.U_STATUS_CHANGED);
			command.Parameters.Add(new ParameterInt32("st", Convert.ToInt32(st)));
			client1.Commands.Add(command);
		}

		private void yemekteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeStatus(ClientStatus.OutToLunch);
		}

		private void disardaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeStatus(ClientStatus.StandBy);
		}

		private void disardaToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			ChangeStatus(ClientStatus.Waiting);
		}

		private void lvwUsers_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (lvwUsers.SelectedItems.Count > 0)
			{
				for (int i = 0; i < lvwUsers.SelectedItems.Count; i++)
				{
					string un = lvwUsers.SelectedItems[0].Text;
					_Manager.Begin(un);
				}
			}
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var command = new NetCommand("TEST");
			client1.Commands.Add(command);

			if (command.WaitForStatus(CommandStatus.Executed))
			{
				MessageBox.Show(command.Response.Message);
			}
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog opf = new OpenFileDialog();
			opf.Filter = "Image Files|*.jpg;*.jpeg;*.gif";
			if (opf.ShowDialog() == DialogResult.OK)
			{
				client1.ImagePath = opf.FileName;
				Properties.Settings.Default.ImagePath = opf.FileName;
				Properties.Settings.Default.Save();

				NetCommand command = new NetCommand();
				command.Name = CommandNames.CLIENT_UPDATE_PICTURE;
				client1.Commands.Add(command);
			}
		}

		private void OnGetPicture(NetCommand command)
		{
			if (client1.ImagePath != string.Empty 
				&& System.IO.File.Exists(client1.ImagePath))
			{
				command.Response = new NetResponse(true);

				FileStream str = File.Open(client1.ImagePath, FileMode.Open);
				byte[] temp = new byte[str.Length];
				str.Read(temp, 0, temp.Length);

				ParameterByteArray a = new ParameterByteArray("img", temp);
				command.Response.Parameters.Add(a);
			}
			else
			{
				command.Response = new NetResponse(false, "No picture");
			}
		}

		private void OnClientUpdatePicture(NetCommand command)
		{
			command.Response = new NetResponse(true);
			ParameterString uname = command.Parameters["un"]
				as ParameterString;
			if (uname == null)
				return;

			FormMsgScreen form = _Manager[uname.Value];
			if (form == null)
				return;

			form.InvokeRefreshImage();
		}

		private void client1_SystemError(object sender, SystemErrorEventArgs e)
		{
			MessageBox.Show(e.Exception.Message);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using (var frm = new FormAbout())
            {
                frm.ShowDialog();
            }
		}

        #endregion
    }
}