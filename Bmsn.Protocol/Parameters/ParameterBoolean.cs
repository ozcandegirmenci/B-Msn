using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter class which has boolean value
    /// </summary>
    public class ParameterBoolean : Parameter<bool>
    {
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterBoolean()
        {
            Value = false;
        }

        /// <summary>
        /// Initialize a new instance of this class with the name
        /// </summary>
        /// <param name="name"></param>
        public ParameterBoolean(string name)
            : base(name)
        { }

        /// <summary>
        /// Initialize a new instance of this class with the name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ParameterBoolean(string name, bool value)
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
            ServerBinaryWriter.WriteBoolean(writer, Value);
        }

        /// <summary>
        /// Reads parameter from the given binary stream
        /// </summary>
        /// <param name="reader"></param>
        public override void Read(BinaryReader reader)
        {
            Value = ServerBinaryReader.ReadBoolean(reader);
        }

        #endregion
    }
}