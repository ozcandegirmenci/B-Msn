using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which contains <see cref="long"/> value
    /// </summary>
	public class ParameterInt64 : Parameter<long>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterInt64()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
		public ParameterInt64(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterInt64(string name, long value)
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
			ServerBinaryWriter.WriteInt64(writer, Value);
		}

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadInt64(reader);
		}

        #endregion
    }
}
