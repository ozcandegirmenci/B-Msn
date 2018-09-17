using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter with the <see cref="string"/> value
    /// </summary>
	public class ParameterString : Parameter<string>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterString()
		{
			Value = string.Empty;
		}

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
		public ParameterString(string name)
			: base(name)
		{
            Value = string.Empty;
        }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterString(string name, string value)
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
			ServerBinaryWriter.WriteString(writer, Value);
		}

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadString(reader);
		}

        #endregion
    }
}
