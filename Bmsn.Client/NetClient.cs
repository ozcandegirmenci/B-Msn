using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Bmsn.Protocol;

namespace Bmsn.Client
{
    /// <summary>
    /// A component for connecting, executing and managing commands against the server
    /// </summary>
    public class NetClient : Component
	{
        #region Events

        public event EventHandler SocketStatusChanged = null;
		public event EventHandler Disconnecting = null;
		public event EventHandler Disconnected = null;
		public event EventHandler Connecting = null;
		public event EventHandler Connected = null;
		public event EventHandler BeginReceiveData = null;
		public event EventHandler EndReceiveData = null;
		public event EventHandler BeginSendingData = null;
		public event EventHandler EndSendingData = null;
		public event EventHandler UnknownServerData = null;
		public event SystemErrorEventHandler SystemError = null;

        #endregion

        #region Members
        
        private SocketReader _SocketReader;
		private Socket _Socket;
		private Thread _Thread;
		private bool _ProcessFlag;
		private Dictionary<int, NetCommand> _SendCommands;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the client command execution manager
        /// </summary>
        [Browsable(false)]
        public CommandSubscriptionManager CommandExecutionManager { get; private set; }

        /// <summary>
        /// Gets the list of server command execution manager
        /// </summary>
        [Browsable(false)]
        public CommandSubscriptionManager ServerCommandsManager { get; private set; }

        /// <summary>
        /// Gets the status of the client
        /// </summary>
        [Browsable(false)]
        public SocketStatus SocketStatus { get; private set; } = SocketStatus.None;

        /// <summary>
        /// Gets the datetime of the client connection
        /// </summary>
        [Browsable(false)]
        public DateTime ConnectTime { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the image path of the client
        /// </summary>
        [DefaultValue(""),
        Description("Gets or sets the image path of the client"),
        Category("Image")]
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time in ms which this client will send NOOP if there is no action
        /// </summary>
        [DefaultValue(5000),
        Category("Behaviour"),
        Description("Gets or sets the time in ms which this client will send NOOP if there is no action")]
        public int NoopPeriod { get; set; } = 5000;

        /// <summary>
        /// Gets the hashcode of the client
        /// </summary>
        [Browsable(false)]
        public int HashCode { get; private set; } = 0;

        /// <summary>
        /// Gets the collection of client commands which are waiting for execution 
        /// </summary>
        [Browsable(false)]
        public CommandCollection Commands { get; private set; }

        /// <summary>
        /// Gets or sets the timeout in ms for the connection requests
        /// </summary>
        [DefaultValue(10000),
        Category("Behaviour"),
        Description("Gets or sets the timeout in ms for the connection requests")]
        public int ConnectingTimeout { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the timeout int ms for the command
        /// </summary>
        [DefaultValue(10000),
        Category("Behaviour"),
        Description("Gets or sets the timeout int ms for the command")]
        public int CommandTimeout { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the timeout in ms for the connection.
        /// </summary>
        [DefaultValue(30000),
        Category("Behaviour"),
        Description("Gets or sets the timeout in ms for the connection.")]
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary>
        /// Gets or sets that whether the client component will protect the connection by sending NOOP commands or not ?
        /// </summary>
        [DefaultValue(true),
        Category("Behaviour"),
        Description("Gets or sets that whether the client component will protect the connection by sending NOOP commands or not ?")]
        public bool KeepConnection { get; set; } = true;

        /// <summary>
        /// Gets or sets the Server IP Address
        /// </summary>
        [DefaultValue("127.0.0.1"),
        Category("Behaviour"),
        Description("Gets or sets the Server IP Address")]
        public string Server { get; set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets the port number of the server
        /// </summary>
        [DefaultValue(11223),
        Description("Gets or sets the port number of the server"),
        Category("Behaviour")]
        public int Port { get; set; } = 11223;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public NetClient()
		{
			Commands = new CommandCollection();
			_SendCommands = new Dictionary<int, NetCommand>();
			CommandExecutionManager = new CommandSubscriptionManager();
			ServerCommandsManager = new CommandSubscriptionManager();

			_SocketReader = new SocketReader();
			_SocketReader.CommandIdReaded += new EventHandler(SocketReader_CommandIdReaded);
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Begins to connect to the server
        /// </summary>
        public void Connect()
		{
			if (SocketStatus != SocketStatus.Disconnected &&
				SocketStatus != SocketStatus.None
				&& SocketStatus != SocketStatus.Error)
				throw new InvalidOperationException("Already connected to a server");

			SetSocketStatus(SocketStatus.None);
            _Thread = new Thread(new ThreadStart(InternalConnect))
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal,
                Name = "Client Thread - " + DateTime.Now.ToString()
            };
            _Thread.Start();
		}

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        public void Disconnect()
        {
            if (!_ProcessFlag)
            {
                SetSocketStatus(SocketStatus.Disconnected);
                return;
            }

            try
            {
                if (SocketStatus == SocketStatus.Connected)
                {
                    NetCommand command = new NetCommand(CommandNames.QUIT);
                    command.Parameters.Add(new ParameterBoolean("fm", false));
                    Send(command);
                }
            }
            catch (SocketException exception)
            {
                if (exception.NativeErrorCode != 0x2745 && exception.NativeErrorCode != 0x2714)
                    throw exception;
            }

            SetSocketStatus(SocketStatus.Disconnecting);
            _ProcessFlag = false;

            if (_Socket != null)
            {
                _Socket.Shutdown(SocketShutdown.Both);
                _Socket.Close();
            }

            ConnectTime = DateTime.MinValue;
            _Socket = null;

            SetSocketStatus(SocketStatus.Disconnected);
        }

        /// <summary>
        /// Sets given command to the server
        /// </summary>
        /// <param name="command"></param>
		public void Send(NetCommand command)
		{
			if (SocketStatus != SocketStatus.Connected)
				return;
			command.Status = CommandStatus.SendingToTarget;
            using (MemoryStream str = new MemoryStream(0))
            {
                BinaryWriter writer = new BinaryWriter(str, System.Text.Encoding.Unicode);
                command.Write(writer);
                writer.Flush();

                Send(str.ToArray(), 0, command.Id);
            }
			command.Status = CommandStatus.WaitingForResponse;
		}

        /// <summary>
        /// Sends given net response to the server
        /// </summary>
        /// <param name="response"></param>
		public void Send(NetResponse response)
		{
            using (MemoryStream str = new MemoryStream(0))
            {
                BinaryWriter writer = new BinaryWriter(str, System.Text.Encoding.Unicode);
                response.Write(writer);
                writer.Flush();

                Send(str.ToArray(), 1, response.CommandId);
            }
		}

        /// <summary>
        /// Sets given data to the server
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isResponse"></param>
        /// <param name="commandId"></param>
		public void Send(byte[] data, byte isResponse, int commandId)
		{
			
			if (_Socket == null)
				return;
			OnBeginSendingData();
			int len = data.Length;
			byte[] temp = new byte[9];
			temp[0] = isResponse;
			temp[1] = (byte)commandId;
			temp[2] = (byte)(commandId >> 0x8);
			temp[3] = (byte)(commandId >> 0x10);
			temp[4] = (byte)(commandId >> 0x18);
			temp[5] = (byte)len;
			temp[6] = (byte)(len >> 0x8);
			temp[7] = (byte)(len >> 0x10);
			temp[8] = (byte)(len >> 0x18);

            _Socket.Send(temp);
            _Socket.Send(data);
			OnEndSendingData();
		}

        /// <summary>
        /// Sets the socket status
        /// </summary>
        /// <param name="value"></param>
		internal void SetSocketStatus(SocketStatus value)
		{
			if (SocketStatus == value)
				return;
			SocketStatus = value;
			OnSocketStatusChanged();
		}

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
            base.Dispose(disposing);
        }

        protected virtual void OnSystemError(Exception ex)
		{
			if (SystemError != null)
				SystemError(this, new SystemErrorEventArgs(ex));
		}

		protected virtual void OnSocketStatusChanged()
		{
			if (SocketStatusChanged != null)
				SocketStatusChanged(this, EventArgs.Empty);

			switch (SocketStatus)
			{ 
				case SocketStatus.Connecting:
					OnConnecting();
					break;
				case SocketStatus.Connected:
					OnConnected();
					break;
				case SocketStatus.Disconnected:
					OnDisconnected();
					break;
				case SocketStatus.Disconnecting:
					OnDisconnecting();
					break;
			}
		}

		protected virtual void OnBeginSendingData()
		{
			if (BeginSendingData != null)
				BeginSendingData(this, EventArgs.Empty);
		}

		protected virtual void OnEndSendingData()
		{
			if (EndSendingData != null)
				EndSendingData(this, EventArgs.Empty);
		}

		protected virtual void OnBeginReceiveData()
		{
			if (BeginReceiveData != null)
				BeginReceiveData(this, EventArgs.Empty);
		}

		protected virtual void OnEndReceiveData()
		{
			if (EndReceiveData != null)
				EndReceiveData(this, EventArgs.Empty);
		}

		protected virtual void OnDisconnected()
		{
			if (Disconnected != null)
				Disconnected(this, EventArgs.Empty);
		}

		protected virtual void OnDisconnecting()
		{
			if (Disconnecting != null)
				Disconnecting(this, EventArgs.Empty);
		}

		protected virtual void OnConnecting()
		{
			if (Connecting != null)
				Connecting(this, EventArgs.Empty);
		}

		protected virtual void OnConnected()
		{
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}

		protected virtual void OnUnknownServerData()
		{
			if (UnknownServerData != null)
				UnknownServerData(this, EventArgs.Empty);
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Internal connect operation
        /// </summary>
        private void InternalConnect()
        {
            Commands.Clear();
            SetSocketStatus(SocketStatus.Connecting);

            IPEndPoint host = new IPEndPoint(IPAddress.Parse(Server), Port);

            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _Socket.Connect(host);

                if (_Socket.Connected)
                {
                    _SocketReader.Socket = _Socket;
                    _SocketReader.Timeout = ConnectingTimeout;
                    _SocketReader.SleepTime = 10;

                    if (_SocketReader.Read() == SocketReader.ReadStatus.Success
                        && _SocketReader.Type == SocketReader.ReadType.Response &&
                        _SocketReader.CommandId == -1)
                    {
                        var response = NetResponse.Parse(_SocketReader.Data);

                        if (response.Okey)
                        {
                            var hash = response.Parameters["hash"] as ParameterInt32;
                            HashCode = hash.Value;
                            SetSocketStatus(SocketStatus.Connected);
                            _ProcessFlag = true;
                            ConnectTime = DateTime.Now;
                            ProcessOperation();
                        }
                        else
                        {
                            _Socket.Shutdown(SocketShutdown.Both);
                            _Socket.Close();
                            SetSocketStatus(SocketStatus.None);
                        }
                    }
                    else
                    {
                        _Socket.Shutdown(SocketShutdown.Both);
                        _Socket.Close();
                        SetSocketStatus(SocketStatus.None);
                    }
                }
                else
                {
                    SetSocketStatus(SocketStatus.None);
                }
            }
            catch //(Exception exception)
            {
                if (_Socket != null && _Socket.Connected)
                {
                    _Socket.Shutdown(SocketShutdown.Both);
                    _Socket.Close();
                }
                SetSocketStatus(SocketStatus.Error);
            }
        }

        /// <summary>
        /// Listen and process messages
        /// </summary>
        private void ProcessOperation()
        {
            var lastCommandDate = DateTime.Now;
            var time = 0;

            _SocketReader.Clear();
            _SocketReader.SleepTime = 5;
            _SocketReader.Socket = _Socket;
            _SocketReader.Timeout = CommandTimeout;

            NetResponse response = null;

            try
            {
                while (_ProcessFlag)
                {
                    response = null;

                    if (_Socket.Available > 0)
                    {
                        OnBeginReceiveData();
                        _SocketReader.Clear();
                        _SocketReader.Read();
                        OnEndReceiveData();

                        if (_SocketReader.LastStatus == SocketReader.ReadStatus.Success)
                        {
                            if (_SocketReader.Type == SocketReader.ReadType.Command)
                            {
                                NetCommand command = NetCommand.Parse(_SocketReader.Data);
                                command.Id = _SocketReader.CommandId;

                                ServerCommandsManager.PublishCommandExecute(command);
                                command.Status = CommandStatus.Executed;

                                if (command.Response == null)
                                    command.Response = new NetResponse(false, "Server command could not be executed");

                                Send(command.Response);
                            }
                            else
                            {
                                var command = _SendCommands[_SocketReader.CommandId] as NetCommand;
                                if (command == null)
                                {
                                    OnUnknownServerData();
                                }
                                else
                                {
                                    response = NetResponse.Parse(_SocketReader.Data);
                                    command.Response = response;
                                    command.Status = CommandStatus.Executed;
                                    CommandExecutionManager.PublishCommandExecute(command);
                                    _SendCommands.Remove(command.Id);
                                }
                            }
                        }
                        else
                        {
                            Disconnect();
                        }

                        lastCommandDate = DateTime.Now;
                    }
                    else
                    {
                        NetCommand active = null;

                        for (int k = 0; k < Commands.Count; k++)
                        {
                            NetCommand cd = Commands[k, true];
                            if (cd.Status == CommandStatus.Waiting)
                            {
                                active = cd;
                                break;
                            }
                        }

                        if (active != null)
                        {
                            Commands.Active = active;
                            OnBeginSendingData();
                            Send(active);
                            OnEndSendingData();
                            _SendCommands.Add(active.Id, active);
                        }
                        else
                        {
                            time = Convert.ToInt32(DateTime.Now.Subtract(lastCommandDate).TotalMilliseconds);
                            if (KeepConnection && time >= NoopPeriod)
                            {
                                Commands.Add(new NetCommand(CommandNames.NOOP));
                            }
                            else
                            {
                                if (time > ConnectionTimeout)
                                {
                                    break;
                                }
                            }

                            Thread.Sleep(10);
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.NativeErrorCode != 0x2745 && ex.NativeErrorCode != 0x2714)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                OnSystemError(ex);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Occurs when the command id readed from the network stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketReader_CommandIdReaded(object sender, EventArgs e)
		{
			if (!_SocketReader.Reading)
				return;

			if (_SocketReader.Type == SocketReader.ReadType.Response)
			{
                if (_SendCommands.ContainsKey(_SocketReader.CommandId))
                {
                    var command = _SendCommands[_SocketReader.CommandId] as NetCommand;
                    if (command != null)
                    {
                        command.Status = CommandStatus.TakingResponse;
                    }
                }
			}
		}

        /// <summary>
        /// Return int id from the byte data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int GetId(byte[] data)
        {
            return (int)((((data[0] & 0xff) |
                (data[1] << 0x8)) |
                (data[2] << 0x10)) |
                (data[3] << 0x18));
        }

        #endregion
    }
}
