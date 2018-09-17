using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which contains <see cref="Byte"/> value
    /// </summary>
    public class ParameterByte : Parameter<byte>
    {
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterByte()
        { }

        /// <summary>
        /// Initialize a new instance of this class with the given name
        /// </summary>
        /// <param name="name"></param>
        public ParameterByte(string name)
            : base(name)
        { }

        /// <summary>
        /// Initialize a new instance of this class with the given name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ParameterByte(string name, byte value)
            : base(name, value)
        { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes parameter to the given binary stream
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            ServerBinaryWriter.WriteByte(writer, Value);
        }

        /// <summary>
        /// Reads parameter from the given binary stream
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = ServerBinaryReader.ReadByte(reader);
        }

        #endregion
    }
}