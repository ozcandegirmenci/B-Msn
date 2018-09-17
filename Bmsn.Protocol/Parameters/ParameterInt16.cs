using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which contains the <see cref="short"/> value
    /// </summary>
	public class ParameterInt16 : Parameter<short>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterInt16()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
		public ParameterInt16(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instnace of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterInt16(string name, short value)
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
			ServerBinaryWriter.WriteInt16(writer, Value);
		}

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadInt16(reader);
		}

        #endregion
    }
}
