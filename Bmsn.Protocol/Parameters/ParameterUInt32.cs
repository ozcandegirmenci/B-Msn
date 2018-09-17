using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter with the <see cref="uint"/> value
    /// </summary>
    public class ParameterUInt32 : Parameter<uint>
    {
        #region Intialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterUInt32()
        { }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
        public ParameterUInt32(string name)
            : base(name)
        { }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ParameterUInt32(string name, uint value)
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
            ServerBinaryWriter.WriteUInt32(writer, Value);
        }

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = ServerBinaryReader.ReadUInt32(reader);
        }

        #endregion
    }
}