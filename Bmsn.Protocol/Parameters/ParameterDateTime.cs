using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which has the <see cref="DateTime"/> value
    /// </summary>
	public class ParameterDateTime : Parameter<DateTime>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterDateTime()
		{
			Value = DateTime.Now;
		}

        /// <summary>
        /// Initialize a new instance of this class with the name
        /// </summary>
        /// <param name="name"></param>
		public ParameterDateTime(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterDateTime(string name, DateTime value)
			: base(name, value)
		{ }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads from the given binary stream
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
			ServerBinaryWriter.WriteDateTime(writer, Value);
		}

        /// <summary>
        /// Writes to the given binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadDateTime(reader);
		}

        #endregion
    }
}
