using System;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter with the <see cref="ushort"/> value
    /// </summary>
    public class ParameterUInt16 : Parameter<ushort>
    {
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterUInt16()
        { }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
        public ParameterUInt16(string name)
            : base(name)
        { }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ParameterUInt16(string name, ushort value)
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
            ServerBinaryWriter.WriteUInt16(writer, Value);
        }

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = ServerBinaryReader.ReadUInt16(reader);
        }

        #endregion
    }
}