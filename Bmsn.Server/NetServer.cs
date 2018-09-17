using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Server component which manages clients connections
    /// </summary>
	public class NetServer : Component
	{
        #region Events

        /// <summary>
        /// Occurs when server is started
        /// </summary>
        [Description("Occurs when server is started")]
		public event EventHandler Started;
        /// <summary>
        /// Occurs when server is stopped
        /// </summary>
		[Description("Occurs when server is started")]
		public event EventHandler Stopped;
        /// <summary>
        /// Occurs when a system error occured on server
        /// </summary>
		[Description("Occurs when a system error occured on server")]
		public event SystemErrorEventHandler SystemErrorOccured;
        /// <summary>
        /// Occurs when a new client is connected to the server
        /// </summary>
		[Description("Occurs when a new client is connected to the server")]
		public event ServerClientEventHandler ClientConnected;
        /// <summary>
        /// Occurs when a client is disconnected from server
        /// </summary>
		[Description("Occurs when a client is disconnected from server")]
		public event ServerClientEventHandler ClientDisconnected;
        /// <summary>
        /// Occurs when a new command received to server
        /// </summary>
		[Description("Occurs when a new command received to server")]
		public event CommandOperationEventHandler NewCommand;
        /// <summary>
        /// Occurs when a clients status is changed
        /// </summary>
		[Description("Occurs when a clients status is changed")]
		public event ServerClientEventHandler ClientStatusChanged;

        #endregion

        #region Members

        private Socket _Listener;
		private Thread _Thread;

		private Dictionary<string, DynamicObjectCreator> _Commands;

		internal bool stopFlag = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets list of <see cref="ServerClient"/> objects which are connected at the server
        /// </summary>
        [Browsable(false)]
        public List<ServerClient> Clients { get; private set; }

        /// <summary>
        /// Gets that is server enabled at the moment or not?
        /// </summary>
        [Browsable(false)]
        public bool Enabled { get; private set; }

        /// <summary>
        /// Gets or sets the Ip address that this server is listening for
        /// </summary>
        [DefaultValue(""),
        Category("Behaviour"),
        Description("Gets or sets the Ip address that this server is listening for")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the port number that this server is listening for
        /// </summary>
        [DefaultValue(11223),
        Category("Behaviour"),
        Description("Gets or sets the port number that this server is listening for")]
        public int Port { get; set; } = 11223;

        /// <summary>
        /// Gets or sets the timeout in ms for a single command
        /// </summary>
        [DefaultValue(10000),
        Category("Behaviour"),
        Description("Gets or sets the timeout in ms for a single command")]
        public int CommandTimeout { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the timeout in ms for a connection
        /// </summary>
        [DefaultValue(30000),
        Category("Behaviour"),
        Description("Gets or sets the timeout in ms for a connection")]
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary>
        /// Gets or sets the number of bad commands that are allowed
        /// </summary>
        /// <remarks>
        /// After it reaches the given number of bad commands connection will be terminated automatically with the client
        /// </remarks>
        [DefaultValue(10),
        Category("Behaviour"),
        Description("Gets or sets the number of bad commands that are allowed")]
        public int BadCommandCount { get; set; } = 10;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public NetServer()
		{
			Clients = new List<ServerClient>(0);
			_Commands = new Dictionary<string, DynamicObjectCreator>();

			SubscribeCommandOperation(CommandNames.NOOP, typeof(Cmd_Noop));
			SubscribeCommandOperation(CommandNames.SEND_MESSAGE, typeof(Cmd_SendMessage));
			SubscribeCommandOperation(CommandNames.LOGIN, typeof(Cmd_Login));
			SubscribeCommandOperation(CommandNames.QUIT, typeof(Cmd_Quit));
			SubscribeCommandOperation(CommandNames.U_STATUS_CHANGED, typeof(Cmd_UserStatusChanged));
			SubscribeCommandOperation(CommandNames.GET_PICTURE, typeof(Cmd_GetPicture));
			SubscribeCommandOperation(CommandNames.CLIENT_UPDATE_PICTURE, typeof(Cmd_ClientUpdatePicture));
			SubscribeCommandOperation(CommandNames.INFORM_WRITING_STATE, typeof(Cmd_InformWritingState));
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribes a command with an operation
        /// </summary>
        /// <param name="command"></param>
        /// <param name="type"></param>
        public void SubscribeCommandOperation(string command, Type type)
		{
			if (_Commands.ContainsKey(command))
				throw new InvalidOperationException("Already contains a command with this name");
			_Commands.Add(command, new DynamicObjectCreator(type.GetConstructor(Type.EmptyTypes)));
		}

        /// <summary>
        /// Unsubscribe a command operation
        /// </summary>
        /// <param name="command"></param>
		public void UnsubscribeCommandOperation(string command)
		{
			if (!_Commands.ContainsKey(command))
				return;
			_Commands.Remove(command);
		}

        /// <summary>
        /// Returns the <see cref="CommandOperation"/> for the given command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
		public CommandOperation GetOperation(NetCommand command)
		{
			if (_Commands.ContainsKey(command.Name))
			{
				return _Commands[command.Name].Invoke() as CommandOperation;
			}

			return null;
		}

        /// <summary>
        /// Starts server component
        /// </summary>
		public void Start()
		{
			if (Enabled)
				throw new Exception("Server already listening");

			if (_Listener != null)
				throw new Exception("Server already listening, " 
					+ "you can not create more than one listener on the same port");

			_Thread = new Thread(new ThreadStart(Process));
			_Thread.IsBackground = true;
			_Thread.Name = "Server Thread";
			_Thread.Priority = ThreadPriority.AboveNormal;
			_Thread.Start();
			Enabled = true;
			OnStarted();
		}

        /// <summary>
        /// Stops server component
        /// </summary>
		public void Stop()
		{
			if (!Enabled)
				return;

			stopFlag = true;

			Enabled = false;
			if (_Listener != null)
				_Listener.Close();
			_Listener = null;

			for (int i = 0; i < Clients.Count;)
			{
				Clients[i].Dispose();
			}

			OnStopped();
		}

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Stop();
			}
			base.Dispose(disposing);
		}

		protected internal virtual void OnClientStatusChanged(ServerClient client)
		{
			if (ClientStatusChanged != null)
				ClientStatusChanged(this, new ServerClientEventArgs(client));
		}

		protected internal virtual void OnNewCommand(CommandOperationEventArgs e)
		{
			if (NewCommand != null)
				NewCommand(this, e);
		}

		protected internal virtual void OnClientConnected(ServerClient client)
		{
			if (ClientConnected != null)
				ClientConnected(this, new ServerClientEventArgs(client));
		}

		protected internal virtual void OnClientDisconnected(ServerClient client)
		{
			if (!stopFlag)
			{
				for (int i = 0; i < Clients.Count; i++)
				{
					if (Clients[i] == client)
						continue;
					NetCommand command = new NetCommand(CommandNames.U_DISCONNECT);
					command.Parameters.Add(new ParameterInt32("hash", client.Guid.GetHashCode()));
					command.Parameters.Add(new ParameterString("un", client.Username));

					Clients[i].Commands.Add(command);
				}
			}

			if (ClientDisconnected != null)
				ClientDisconnected(this, new ServerClientEventArgs(client));
		}

		protected internal virtual void OnSystemError(Exception ex)
		{
			if (SystemErrorOccured != null)
				SystemErrorOccured(this, new SystemErrorEventArgs(ex));
		}

		protected virtual void OnStarted()
		{
			if (Started != null)
				Started(this, EventArgs.Empty);
		}

		protected virtual void OnStopped()
		{
			if (Stopped != null)
				Stopped(this, EventArgs.Empty);
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Listens for incoming connections and creates <see cref="ServerClient"/> objects for new connections
        /// </summary>
        private void Process()
        {
            try
            {
                _Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (string.IsNullOrEmpty(IpAddress))
                    _Listener.Bind(new IPEndPoint(IPAddress.Any, Port));
                else
                    _Listener.Bind(new IPEndPoint(IPAddress.Parse(IpAddress), Port));

                _Listener.Listen(10);


                Socket socket = null;

                while (Enabled)
                {
                    socket = _Listener.Accept();

                    ServerClient client = new ServerClient(socket, this);
                    Clients.Add(client);
                    client.Start();
                    Thread.Sleep(10);
                }
            }
            catch (SocketException ex)
            {
                // 0x2714 WSACancelBlock ignore this error
                if (ex.NativeErrorCode != 0x2714)
                {
                    OnSystemError(ex);
                }
            }
            catch (Exception ex)
            {
                OnSystemError(ex);
            }
            finally
            {
                Stop();
            }
        }

        #endregion
    }
}
