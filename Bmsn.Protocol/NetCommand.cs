using System;
using System.IO;
using System.Threading;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a Command which is transfered between server and client
    /// </summary>
    public class NetCommand
    {
        public event CommandStatusChangedEventHandler StatusChanged = null;
        public event EventHandler Executed = null;
        
        private int _Id;
        private CommandStatus _Status = CommandStatus.Unknown;
        private NetResponse _Response;

        #region Properties

        /// <summary>
        /// Gets the id of the command
        /// </summary>
        public int Id
        {
            get { return _Id; }
            set {
                _Id = value;
                if (_Response != null)
                {
                    _Response.CommandId = _Id;
                }
            }
        }

        /// <summary>
        /// Gets or sets the status of the command
        /// </summary>
        public CommandStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value)
                    return;
                CommandStatus oldStatus = _Status;
                _Status = value;
                OnStatusChanged(oldStatus);
            }
        }

        /// <summary>
        /// Gets or sets the parent collection
        /// </summary>
        public CommandCollection Parent { get; set; }

        /// <summary>
        /// Gets or sets the name of the command
        /// </summary>
        public string Name { get; set; }

        public ParameterCollection Parameters { get; private set; }

        /// <summary>
        /// Gets the <see cref="NetResponse"/> of the command
        /// </summary>
        public NetResponse Response
        {
            get
            {
                if (_Response == null)
                {
                    _Response = new NetResponse();
                    _Response.SetOkey(false);
                    _Response.CommandId  = Id;
                }
                return _Response;
            }
            set
            {
                _Response = value;
                if (value != null)
                    value.CommandId = Id;
            }
        }

        /// <summary>
        /// Gets the execution begin date of the command
        /// </summary>
        public DateTime BeginDate { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Gets the execution end date of the command
        /// </summary>
        public DateTime EndDate { get; private set; } = DateTime.MaxValue;

        /// <summary>
        /// Gets the execution time of the command
        /// </summary>
        public int ExecutionTime
        {
            get
            {
                return (int)EndDate.Subtract(BeginDate).TotalMilliseconds;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public NetCommand()
        {
            Parameters = new ParameterCollection();
        }

        /// <summary>
        /// Initializa a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
        public NetCommand(string name)
            : this()
        {
            Name = name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes this command to the given binary stream
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            ServerBinaryWriter.WriteTinyString(writer, Name);
            Parameters.Write(writer);
        }

        /// <summary>
        /// Reads from the given binary stream
        /// </summary>
        /// <param name="reader"></param>
        public void Read(BinaryReader reader)
        {
            Name = ServerBinaryReader.ReadTinyString(reader);
            Parameters.Read(reader);
        }

        /// <summary>
        /// Parses given byte[] and returns its command representation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NetCommand Parse(byte[] data)
        {
            using (MemoryStream str = new MemoryStream(data))
            {
                return Parse(str);
            }
        }

        /// <summary>
        /// Parses given memory stream and creates command from it
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static NetCommand Parse(MemoryStream str)
        {
            using (BinaryReader reader = new BinaryReader(str))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses Command from given binary reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static NetCommand Parse(BinaryReader reader)
        {
            NetCommand command = new NetCommand();
            command.Read(reader);
            return command;
        }

        /// <summary>
        /// Waits for the command status changed to given status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool WaitForStatus(CommandStatus status)
        {
            return WaitForStatus(status, 10, 3000);
        }

        /// <summary>
        /// Waits for the command statuses is changed to the given status in the given time
        /// </summary>
        /// <param name="status"></param>
        /// <param name="sleep"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForStatus(CommandStatus status, int sleep, int timeout)
        {
            int s = Convert.ToInt32(status);
            DateTime begin = DateTime.Now;

            while (true)
            {
                if (Convert.ToInt32(Status) >= s)
                    return true;
                if (DateTime.Now.Subtract(begin).TotalMilliseconds > timeout)
                    return false;

                Thread.Sleep(sleep);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Fires <see cref="StatusChanged"/> event for this command
        /// </summary>
        /// <param name="status"></param>
        protected virtual void OnStatusChanged(CommandStatus status)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new CommandStatusChangedEventArgs(this, status));
            }

            if (Status == CommandStatus.Executed)
            {
                OnExecuted();
            }
        }

        /// <summary>
        /// Fires <see cref="Executed"/> event for this command
        /// </summary>
        protected virtual void OnExecuted()
        {
            EndDate = DateTime.Now;
            if (Executed != null)
            {
                Executed(this, EventArgs.Empty);
            }

            if (Parent != null)
            {
                Parent.Remove(this); ;
                if (Parent.Active == this)
                {
                    Parent.Active = null;
                }
            }
        }

        #endregion
    }
}
