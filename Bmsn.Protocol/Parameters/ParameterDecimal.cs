using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which contains <see cref="decimal"/> parameter
    /// </summary>
	public class ParameterDecimal : Parameter<decimal>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterDecimal()
		{
		}

        /// <summary>
        /// Initialize a new instance of this class with the name
        /// </summary>
        /// <param name="name"></param>
		public ParameterDecimal(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterDecimal(string name, decimal value)
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
			ServerBinaryWriter.WriteDecimal(writer, Value);
		}

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadDecimal(reader);
		}

        #endregion
    }
}
