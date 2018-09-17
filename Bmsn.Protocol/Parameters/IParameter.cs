using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Provides methods and properties for a system parameter
    /// </summary>
    public interface IParameter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent collection
        /// </summary>
        ParameterCollection Parent { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Reads parameter from given binary stream
        /// </summary>
        /// <param name="reader"></param>
        void Read(BinaryReader reader);

        /// <summary>
        /// Writes parameter to the given binary stream
        /// </summary>
        /// <param name="writer"></param>
        void Write(BinaryWriter writer);

        #endregion
    }
}
