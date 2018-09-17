using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which contains <see cref="NetCommand"/> value
    /// </summary>
	public class ParameterCommand : Parameter<NetCommand>
	{
        #region Members

        private const byte NULL = (byte)0;
        
        #endregion

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterCommand()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the name
        /// </summary>
        /// <param name="name"></param>
		public ParameterCommand(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the given name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterCommand(string name, NetCommand value)
			: base(name, value)
		{ }

        #region Public Methods

        /// <summary>
        /// Writes parameter to the binary stream
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
            if (Value == null)
            {
                ServerBinaryWriter.WriteByte(writer, NULL);
            }
            else
            {
                ServerBinaryWriter.WriteByte(writer, (byte)1);
                Value.Write(writer);
            }
		}

        /// <summary>
        /// Reads parameter from the given binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			byte first = ServerBinaryReader.ReadByte(reader);
            if (first == NULL)
            {
                Value = null;
            }
            else
            {
                Value = NetCommand.Parse(reader);
            }
		}

        #endregion
    }
}
