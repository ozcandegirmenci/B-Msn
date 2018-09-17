using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter with the <see cref="byte[]"/> value
    /// </summary>
	public class ParameterByteArray : Parameter<byte[]>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterByteArray()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the name
        /// </summary>
        /// <param name="name"></param>
		public ParameterByteArray(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the given name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterByteArray(string name, byte[] value)
			: base(name, value)
		{ }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes to the given binary stream
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
			ServerBinaryWriter.WriteByteArray(writer, Value);
		}

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadByteArray(reader);
		}

        #endregion
    }
}
