using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter with the <see cref="ulong"/> value
    /// </summary>
    public class ParameterUInt64 : Parameter<ulong>
    {
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class 
        /// </summary>
        public ParameterUInt64()
        { }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        public ParameterUInt64(string name)
            : base(name)
        { }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ParameterUInt64(string name, ulong value)
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
            ServerBinaryWriter.WriteUInt64(writer, Value);
        }

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = ServerBinaryReader.ReadUInt64(reader);
        }

        #endregion
    }
}