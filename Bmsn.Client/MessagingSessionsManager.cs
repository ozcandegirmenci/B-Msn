using System;
using System.Collections.Generic;
using System.Text;
using Bmsn.Protocol;

namespace Bmsn.Client
{
    /// <summary>
    /// Manage messaging sessions
    /// </summary>
	internal class MessagingSessionsManager
	{
        #region Members

        private readonly List<FormMsgScreen> _Forms = new List<FormMsgScreen>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="FormMsgScreen"/> accordign to the given Username
        /// </summary>
        /// <param name="un"></param>
        /// <returns></returns>
        public FormMsgScreen this[string un]
        {
            get
            {
                for (int i = 0; i < _Forms.Count; i++)
                {
                    if (_Forms[i].TargetUser == un)
                        return _Forms[i];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the main form
        /// </summary>
        public FormMain MainForm { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="mainform"></param>
        public MessagingSessionsManager(FormMain mainform)
		{
			MainForm = mainform;

			mainform.client1.ServerCommandsManager.Subscribe(
				CommandNames.SEND_MESSAGE,
				new CommandExecuteHandler(OnSendMessage));
			mainform.client1.ServerCommandsManager.Subscribe(
				CommandNames.INFORM_WRITING_STATE,
				new CommandExecuteHandler(OnInformState));
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// User disconnected
        /// </summary>
        /// <param name="un"></param>
        internal void Disconnected(string un)
		{
			FormMsgScreen frm = this[un];
			if (frm == null)
				return;

			frm.UserConnectionStateChanged(false);
		}

        /// <summary>
        /// User connected
        /// </summary>
        /// <param name="un"></param>
		internal void Connected(string un)
		{
			FormMsgScreen frm = this[un];
			if (frm == null)
				return;

			frm.UserConnectionStateChanged(true);
		}

        /// <summary>
        /// Begin messaging with the given user
        /// </summary>
        /// <param name="un"></param>
		internal void Begin(string un)
		{
			FormMsgScreen form = this[un];
			if (form == null)
			{
                form = new FormMsgScreen(this, un);
                form.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MessagingForm_FormClosing);
				_Forms.Add(form);
			}

			form.Show();
		}

        /// <summary>
        /// Adds command to the <see cref="NetClient"/>
        /// </summary>
        /// <param name="command"></param>
		internal void AddCommand(NetCommand command)
		{
			MainForm.client1.Commands.Add(command);
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Target user state is changed
        /// </summary>
        /// <param name="command"></param>
        private void OnInformState(NetCommand command)
		{
			MainForm.Invoke(new EventHandler(InvokeInformState), new object[] { command, null });
		}

        private void InvokeInformState(object sender, EventArgs e)
		{
			var command = sender as NetCommand;
			if (command == null)
				return;

			var un = command.Parameters["un"] as ParameterString;
			var st = command.Parameters["st"] as ParameterBoolean;


			var frm = this[un.Value];
			if (frm != null)
			{
				frm.InformState(st.Value);	
			}
		}

        /// <summary>
        /// Message received
        /// </summary>
        /// <param name="command"></param>
        private void OnSendMessage(NetCommand command)
		{
			MainForm.Invoke(new EventHandler(InvokeSendMessage), new object[] { command, null});
		}

        /// <summary>
        /// Append received message into the target user form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvokeSendMessage(object sender, EventArgs e)
		{
			var command = sender as NetCommand;
			if (command == null)
				return;

			var fr = command.Parameters["fr"] as ParameterString;
			var msg = command.Parameters["msg"] as ParameterString;
            
			var form = this[fr.Value];
			if (form == null)
			{
				form = new FormMsgScreen(this, fr.Value);
				form.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MessagingForm_FormClosing);
				_Forms.Add(form);
			}
			form.Show();
			form.AddMessage(msg.Value);
		}
        
		private void MessagingForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			FormMsgScreen frm = sender as FormMsgScreen;
            if (frm != null)
            {
                frm.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(MessagingForm_FormClosing);
                _Forms.Remove(frm);
            }
		}

        #endregion
    }
}
