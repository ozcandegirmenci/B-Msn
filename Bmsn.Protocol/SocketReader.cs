using System;
using System.Net.Sockets;
using System.Threading;

namespace Bmsn.Protocol
{
    /// <summary>
    ///  Socket reader, Reads available data from socket
    /// </summary>
    /// <remarks>
    ///  This method reads from a socket and returns readed data and reading operation status ...
    /// </remarks>
    public class SocketReader
    {
        #region Types

        /// <summary>
        /// Represents reading status
        /// </summary>
        public enum ReadStatus
        {
            None = 0,
            Success,
            Timeout,
            Exception
        }

        /// <summary>
        /// Read operation type
        /// </summary>
        public enum ReadType
        {
            Command,
            Response
        }

        /// <summary>
        /// Read position
        /// </summary>
        public enum ReadPosition
        {
            Type,
            CommandId,
            Length,
            Data
        }

        #endregion

        #region Events

        public event EventHandler BeginReading = null;
        public event EventHandler LengthTaken = null;
        public event EventHandler DataReaded = null;
        public event EventHandler TypeReaded = null;
        public event EventHandler CommandIdReaded = null;

        #endregion

        #region Members

        private int _SleepTime = 10;
        private Socket _Socket = null;
        private byte[] _Data = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the last read status
        /// </summary>
        public ReadStatus LastStatus { get; private set; } = ReadStatus.None;

        /// <summary>
        /// Gets the type of the read
        /// </summary>
        public ReadType Type { get; private set; } = ReadType.Command;

        /// <summary>
        /// Gets the command if
        /// </summary>
        public int CommandId { get; private set; }

        /// <summary>
        /// Gets that is reading?
        /// </summary>
        public bool Reading { get; private set; }

        /// <summary>
        /// Gets the readed data
        /// </summary>
        public byte[] Data
        {
            get
            {
                if (_Data == null)
                    return new byte[0];
                return _Data;
            }
        }

        /// <summary>
        /// Gets or sets the position
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets or sets the readed target count
        /// </summary>
        public int TargetLength { get; private set; }

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public int Timeout { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the data length
        /// </summary>
        public int Count
        {
            get
            {
                if (_Data == null)
                    return 0;
                return Data.Length;
            }
        }

        /// <summary>
        /// Gets or sets the socket
        /// </summary>
        public Socket Socket
        {
            get { return _Socket; }
            set
            {
                if (Reading)
                    throw new InvalidOperationException("You can not set this property while socket is in reading state");
                _Socket = value;
            }
        }

        /// <summary>
        /// Gets or sets the sleep time
        /// </summary>
        public int SleepTime
        {
            get { return _SleepTime; }
            set
            {
                if (Reading)
                    throw new InvalidOperationException("You can not set this property while socket is in reading state");
                _SleepTime = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public SocketReader()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears reading operation
        /// </summary>
        public void Clear()
        {
            _Data = null;
            TargetLength = 0;
            Reading = false;
            Position = 0;
            Type = ReadType.Command;
            CommandId = 0;
            LastStatus = ReadStatus.None;
        }

        /// <summary>
        /// Reads from the socket
        /// </summary>
        /// <returns></returns>
        public ReadStatus Read()
        {
            LastStatus = ReadStatus.Success;

            try
            {
                Reading = true;
                OnBeginReading();

                ReadPosition position = ReadPosition.Type;

                _Data = null;
                Position = 0;
                double time = 0;
                TargetLength = 0;
                DateTime lastReadTime = DateTime.Now;

                while (true)
                {
                    if (Socket.Available > 1)
                    {
                        byte[] type = new byte[1];
                        Socket.Receive(type, 0, 1, SocketFlags.None);
                        if (type[0] == 0)
                            Type = ReadType.Command;
                        else
                            Type = ReadType.Response;
                        OnTypeReaded();
                        position = ReadPosition.CommandId;
                        lastReadTime = DateTime.Now;
                        break;
                    }
                    else
                    {
                        time = DateTime.Now.Subtract(lastReadTime).TotalMilliseconds;
                        if (time > Timeout)
                        {
                            LastStatus = ReadStatus.Timeout;
                            break;
                        }
                        Thread.Sleep(SleepTime);
                    }
                }

                if (position == ReadPosition.CommandId)
                {
                    while (true)
                    {
                        if (Socket.Available > 3)
                        {
                            byte[] type = new byte[4];
                            Socket.Receive(type, 0, 4, SocketFlags.None);
                            CommandId = (int)((((type[0] & 0xff) | (type[1] << 8))
                                | (type[2] << 0x10)) | (type[3] << 0x18));
                            OnCommandIdReaded();
                            position = ReadPosition.Length;
                            lastReadTime = DateTime.Now;
                            break;
                        }
                        else
                        {
                            time = DateTime.Now.Subtract(lastReadTime).TotalMilliseconds;
                            if (time > Timeout)
                            {
                                LastStatus = ReadStatus.Timeout;
                                break;
                            }
                            Thread.Sleep(SleepTime);
                        }
                    }
                }

                if (position == ReadPosition.Length)
                {
                    while (true)
                    {
                        if (Socket.Available > 3)
                        {
                            byte[] temp = new byte[4];
                            Socket.Receive(temp, 0, 4, SocketFlags.None);
                            TargetLength = (int)((((temp[0] & 0xff) | (temp[1] << 8))
                                | (temp[2] << 0x10)) | (temp[3] << 0x18));
                            OnLengthTaken();
                            position = ReadPosition.Data;
                            lastReadTime = DateTime.Now;
                            break;
                        }
                        else
                        {
                            time = DateTime.Now.Subtract(lastReadTime).TotalMilliseconds;
                            if (time > Timeout)
                            {
                                LastStatus = ReadStatus.Timeout;
                                break;
                            }
                            Thread.Sleep(SleepTime);
                        }
                    }
                }

                if (position == ReadPosition.Data)
                {
                    _Data = new byte[TargetLength];
                    Position = 0;

                    if (TargetLength > 0)
                    {
                        while (true)
                        {
                            if (Socket.Available > 0)
                            {
                                int l = Socket.Available;
                                if (l > _Data.Length - Position)
                                    l = _Data.Length - Position;
                                Position += Socket.Receive(_Data, Position,
                                    l, SocketFlags.None);
                                OnDataReaded();
                                lastReadTime = DateTime.Now;
                                if (Position == _Data.Length)
                                {
                                    LastStatus = ReadStatus.Success;
                                    break;
                                }
                            }
                            else
                            {
                                time = DateTime.Now.Subtract(lastReadTime).TotalMilliseconds;
                                if (time > Timeout)
                                {
                                    LastStatus = ReadStatus.Timeout;
                                    break;
                                }
                                Thread.Sleep(SleepTime);
                            }
                        }
                    }
                }
            }
            catch /*(Exception ex)*/
            {
                LastStatus = ReadStatus.Exception;
            }
            finally
            {
                Reading = false;
            }

            return LastStatus;
        }

        #endregion

        #region Protected Methods

        protected void OnBeginReading()
        {
            if (BeginReading != null)
                BeginReading(this, EventArgs.Empty);
        }

        protected void OnLengthTaken()
        {
            if (LengthTaken != null)
                LengthTaken(this, EventArgs.Empty);
        }

        protected void OnDataReaded()
        {
            if (DataReaded != null)
                DataReaded(this, EventArgs.Empty);
        }

        protected void OnCommandIdReaded()
        {
            if (CommandIdReaded != null)
                CommandIdReaded(this, EventArgs.Empty);
        }

        protected void OnTypeReaded()
        {
            if (TypeReaded != null)
                TypeReaded(this, EventArgs.Empty);
        }

        #endregion
    }
}
