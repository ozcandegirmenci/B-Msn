using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Response of a command
    /// </summary>
    public class NetResponse
    {
        #region Members

        private int _CommandId;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the command Id of this response
        /// </summary>
        public int CommandId { get; set; }

        /// <summary>
        /// Gets that is command response is ok?
        /// </summary>
        public bool Okey { get; private set; }

        /// <summary>
        /// Sets the response ok property
        /// </summary>
        /// <param name="value"></param>
        internal void SetOkey(bool value)
        {
            Okey = value;
        }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the parameters of the response
        /// </summary>
        public ParameterCollection Parameters { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public NetResponse()
        {
            Parameters = new ParameterCollection();
        }

        /// <summary>
        /// Initialize a new instance of this class with the ok parameter
        /// </summary>
        /// <param name="okey"></param>
        public NetResponse(bool okey)
            : this()
        {
            Okey = okey;
        }

        /// <summary>
        /// Initialize a new instance of this class with the ok and msg parameters
        /// </summary>
        /// <param name="okey"></param>
        /// <param name="msg"></param>
        public NetResponse(bool okey, string msg)
            : this(okey)
        {
            Message = msg;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set net response to error with the provided error message
        /// </summary>
        /// <param name="msg"></param>
        public void SetError(string msg)
        {
            Message = msg;
            Okey = false;
        }

        /// <summary>
        /// Writes to binary stream
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            ServerBinaryWriter.WriteBoolean(writer, Okey);
            ServerBinaryWriter.WriteShortString(writer, Message);
            Parameters.Write(writer);
        }

        /// <summary>
        /// Reads from binary stream
        /// </summary>
        /// <param name="reader"></param>
        public void Read(BinaryReader reader)
        {
            Okey = ServerBinaryReader.ReadBoolean(reader);
            Message = ServerBinaryReader.ReadShortString(reader);
            Parameters.Read(reader);
        }

        /// <summary>
        /// Parses a new net response from given byte data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NetResponse Parse(byte[] data)
        {
            using (MemoryStream str = new MemoryStream(data))
            {
                return Parse(str);
            }
        }

        /// <summary>
        /// Parses a new net response from memory stream
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static NetResponse Parse(MemoryStream str)
        {
            using (BinaryReader reader = new BinaryReader(str))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses a new net response from binary reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static NetResponse Parse(BinaryReader reader)
        {
            NetResponse response = new NetResponse();
            response.Read(reader);
            return response;
        }

        /// <summary>
        /// Implicit bool operator
        /// </summary>
        /// <param name="response"></param>
        public static implicit operator bool(NetResponse response)
        {
            return response.Okey;
        }

        /// <summary>
        /// Creates a new not allowed net response
        /// </summary>
        /// <returns></returns>
        public static NetResponse NotAllowed()
        {
            return new NetResponse(false, "You dont have permission for this command");
        }

        /// <summary>
        /// Creates a system error net response
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static NetResponse SystemError(string err)
        {
            return new NetResponse(false, "System Exception: " + err);
        }

        #endregion
    }
}
