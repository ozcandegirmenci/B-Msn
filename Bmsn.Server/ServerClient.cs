using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Manages a single client server operations
    /// </summary>
	public class ServerClient : IDisposable
	{
        #region Events

        /// <summary>
        /// Occurs when this client disconnected from server
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region Members
        
		private ClientStatus _Status = ClientStatus.None;
		private SocketReader _SocketReader;
		private Thread _ClientThread;
		private bool _IsOnProcess;
		private int _BadCommandCount;
		private Dictionary<int, NetCommand> _SendedCommands;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the status of a client
        /// </summary>
        public ClientStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value)
                    return;
                _Status = value;
                Server.OnClientStatusChanged(this);
            }
        }

        /// <summary>
        /// Gets the active command operations of this client
        /// </summary>
        public CommandOperationCollection Operations { get; private set; }

        /// <summary>
        /// Gets the connection date of this client
        /// </summary>
        public DateTime ConnectionTime { get; private set; }

        /// <summary>
        /// Gets the hashcode of this client
        /// </summary>
        public int HashCode { get; private set; }

        /// <summary>
        /// Gets the <see cref="NetServer"/> component of this client
        /// </summary>
        public NetServer Server { get; private set; }

        /// <summary>
        /// Gets the unique quid of this client
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// Gets the <see cref="Socket"/> instance of this client
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// Gets or sets the username of this client
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets the list of active commands of this client
        /// </summary>
        public CommandCollection Commands { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class with the provided values
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="server"></param>
        public ServerClient(Socket socket, NetServer server)
		{
			Socket = socket;
			Server = server;
			Guid = Guid.NewGuid();
			HashCode = Guid.GetHashCode();
			Commands = new ServerCommandCollection();
			_SendedCommands = new Dictionary<int, NetCommand>();
			Operations = new CommandOperationCollection(this);

			_SocketReader = new SocketReader();
			_SocketReader.CommandIdReaded += new EventHandler(OnCommandIdTaken);
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts listening for this client
        /// </summary>
        internal void Start()
        {
            _ClientThread = new Thread(new ThreadStart(ClientProcess));
            _ClientThread.IsBackground = true;
            _ClientThread.Name = "Client Thread - " + Guid.ToString("N");
            _ClientThread.Priority = ThreadPriority.Normal;

            _IsOnProcess = true;
            _ClientThread.Start();
        }

        /// <summary>
        /// Gets the hash code of this client
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        /// <summary>
        /// Disconnects from target
        /// </summary>
        public void Disconnect()
        {
            if (!_IsOnProcess)
                return;

            Commands.Clear();
            _SendedCommands.Clear();
            _IsOnProcess = false;

            if (Socket != null)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            Socket = null;

            Server.Clients.Remove(this);
            Server.OnClientDisconnected(this);

            OnDisconnected();
        }

        /// <summary>
        /// Sends given net command to the server
        /// </summary>
        /// <param name="command"></param>
        public void Send(NetCommand command)
        {
            using (MemoryStream str = new MemoryStream(0))
            {
                BinaryWriter writer = new BinaryWriter(str, System.Text.Encoding.Unicode);
                command.Write(writer);
                writer.Flush();

                Send(str.ToArray(), 0, command.Id);
            }
        }

        /// <summary>
        /// Sends given net response to the given target
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
        /// Sends given data to the target
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isResponse"></param>
        /// <param name="commandId"></param>
        public void Send(byte[] data, byte isResponse, int commandId)
        {
            if (Socket == null)
                return;
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

            Socket.Send(temp);
            Socket.Send(data);
        }

        /// <summary>
        /// Releases all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Occurs when a new command received
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewCommand(CommandOperationEventArgs e)
        {
            Server.OnNewCommand(e);

            if (!e.Handled)
            {
                e.Operation = Server.GetOperation(e.Command);
            }

            if (e.Operation != null)
            {
                e.Operation.Command = e.Command;
                Operations.Add(e.Operation);
                e.Operation.StartOperation();
            }
        }

        protected virtual void OnDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
            GC.SuppressFinalize(this);
        }

        ~ServerClient()
        {
            Dispose(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Command id is taken from stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandIdTaken(object sender, EventArgs e)
		{
			if (!_SocketReader.Reading)
				return;
			if (_SocketReader.Type == SocketReader.ReadType.Response)
			{
                if (_SendedCommands.ContainsKey(_SocketReader.CommandId))
                {
                    var command = _SendedCommands[_SocketReader.CommandId]
                            as NetCommand;
                    if (command != null)
                        command.Status = CommandStatus.TakingResponse;
                }
			}
		}

        /// <summary>
        /// Process client operations
        /// </summary>
        private void ClientProcess()
		{
			DateTime lastCommandDate = DateTime.Now;

			_SocketReader.Socket = Socket;
			_SocketReader.SleepTime = 5; // 5 ms
			_SocketReader.Timeout = Server.CommandTimeout;

			_BadCommandCount = 0;

			try {
				// send welcome message to the client
				var response = new NetResponse(true, "Server is ready at " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
				response.CommandId = -1;
				response.Parameters.Add(new ParameterInt32("hash", Guid.GetHashCode()));

				Send(response);

				Server.OnClientConnected(this);

				double time = 0;
				response = null;
				bool badcommand = false;
				NetCommand command = null;

				while (_IsOnProcess)
				{
					response = null;
					badcommand = false;
					command = null;

					if (Socket.Available > 0)
					{
						_SocketReader.Clear();
						_SocketReader.Read();

						lastCommandDate = DateTime.Now;

						if (_SocketReader.LastStatus == SocketReader.ReadStatus.Success)
						{
							try {
								if (_SocketReader.Type == SocketReader.ReadType.Command)
								{
									command = NetCommand.Parse(_SocketReader.Data);
									command.Id = _SocketReader.CommandId;

									CommandOperationEventArgs e = new CommandOperationEventArgs(command);
									OnNewCommand(e);

									if (!e.Handled)
										badcommand = true;
								}
								else {
									command = _SendedCommands[_SocketReader.CommandId]
											as NetCommand;
									if (command != null)
									{
										response = NetResponse.Parse(_SocketReader.Data);
										response.CommandId = _SocketReader.CommandId;

										command.Response = response;
										command.Status = CommandStatus.Executed;
										_SendedCommands.Remove(command.Id);
									}
								}
							}
							catch (Exception exception){
								Server.OnSystemError(exception);
								badcommand = true;
							}

							if (badcommand)
							{
								_BadCommandCount++;
								if (_SocketReader.Type == SocketReader.ReadType.Command)
								{
									if (command != null)
									{
										response = new NetResponse(false, "Bad command");
										command.Response = response;
										Send(response);
										command.Status = CommandStatus.Executed;
									}
								}
								else
								{
									command = _SendedCommands[response.CommandId] 
											as NetCommand;
									if (command != null)
									{
										command.Response = new NetResponse(false, "Bad command");
										command.Status = CommandStatus.Executed;
										_SendedCommands.Remove(command.Id);
									}
								}

								if (_BadCommandCount > Server.BadCommandCount)
								{
									response = new NetResponse(false, "Because of too much bad commands your connection will be terminated");
									Send(response);
									break;
								}
							}
						}
						else
						{
							switch (_SocketReader.LastStatus)
							{ 
								case SocketReader.ReadStatus.Exception:
									response = new NetResponse(false, "Socket reading exception");
									Send(response);
									break;
								case SocketReader.ReadStatus.Timeout:
									response = new NetResponse(false, "Timeout");
									Send(response);
									break;
								default:
									response = new NetResponse(false, "Unknow socket reading exception");
									Send(response);
									break;
							}
						}
					}
					else {
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
							active.Status = CommandStatus.SendingToTarget;
							Send(active);
							_SendedCommands.Add(active.Id, active);
							active.Status = CommandStatus.WaitingForResponse;
						}
						else
						{
							time = DateTime.Now.Subtract(lastCommandDate).TotalMilliseconds;
							if (time > Server.ConnectionTimeout)
							{
								response = new NetResponse(false, "Connection is timeout");
								Send(response);
								Disconnect();
								break;
							}

							Thread.Sleep(10);
						}
					}
				}
			}
			catch (SocketException ex)
			{
				if (ex.NativeErrorCode == 0x2745)
				{ 
					// connection close by remote host is not an exception (client might be closed)
				}
				else if (ex.NativeErrorCode != 0x2714) // WSACancelBlockCall (We might close)
				{ 
					Server.OnSystemError(new Exception(string.Format("Socket exception in Client {0}", Guid.ToString("N")), ex));
				}
			}
			catch (Exception ex){
				Server.OnSystemError(new Exception(string.Format("Exception in Client {0}", Guid.ToString("N")), ex));
			}
			finally{
				Disconnect();
			}
		}

        #endregion
    }
}
