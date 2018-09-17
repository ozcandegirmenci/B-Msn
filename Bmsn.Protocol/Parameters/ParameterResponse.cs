using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which contains the <see cref="NetResponse"/> value
    /// </summary>
	public class ParameterResponse : Parameter<NetResponse>
	{
        #region Members

        private const byte NULL = (byte)0;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterResponse()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
		public ParameterResponse(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterResponse(string name, NetResponse value)
			: base(name, value)
		{ }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes to the binary stream
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
			if (Value == null)
				ServerBinaryWriter.WriteByte(writer, NULL);
			else
			{
				ServerBinaryWriter.WriteByte(writer, (byte)1);
				Value.Write(writer);
			}
		}

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			byte first = ServerBinaryReader.ReadByte(reader);
			if (first == NULL)
				Value = null;
			else
				Value = NetResponse.Parse(reader);
		}

        #endregion
    }
}
