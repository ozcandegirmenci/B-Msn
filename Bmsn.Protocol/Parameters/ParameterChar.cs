using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter with the <see cref="char"/> value
    /// </summary>
	public class ParameterChar : Parameter<char>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterChar()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the given name
        /// </summary>
        /// <param name="name"></param>
		public ParameterChar(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the given name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterChar(string name, char value)
			: base(name, value)
		{ }

        #endregion

        #region Public Methods

        public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
			ServerBinaryWriter.WriteChar(writer, Value);
		}

		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadChar(reader);
		}

        #endregion
    }
}
