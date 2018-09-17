using System;
using System.IO;
using System.Drawing;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a parameter which contains <see cref="Image"/> value
    /// </summary>
	public class ParameterImage : Parameter<Image>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterImage()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
		public ParameterImage(string name)
			: base(name)
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		public ParameterImage(string name, Image value)
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
			ServerBinaryWriter.WriteImage(writer, Value);
		}

        /// <summary>
        /// Reads from the binary stream
        /// </summary>
        /// <param name="reader"></param>
		public override void Read(BinaryReader reader)
		{
			Value = ServerBinaryReader.ReadImage(reader);
		}

        #endregion
    }
}
