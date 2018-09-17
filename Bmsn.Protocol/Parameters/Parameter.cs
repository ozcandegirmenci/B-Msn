using System;
using System.Collections.Generic;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents the base type for server parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Parameter<T> : IParameter
    {
        #region Members

        private T _Value = default(T);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the parent parameter collection
        /// </summary>
        public ParameterCollection Parent { get; set; }

        /// <summary>
        /// Gets the Type of the value
        /// </summary>
        public Type ValueType { get { return typeof(T); } }

        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        public T Value
        {
            get { return _Value; }
            set
            {
                OnValueChanging(value);
                _Value = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Parameter()
        {

        }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
        public Parameter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initialize a new instance of this class with the provided name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Parameter(string name, T value)
            : this(name)
        {
            Value = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes parameter to the binary stream
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Write(BinaryWriter writer)
        {
            ServerBinaryWriter.WriteTinyString(writer, Name);
            ServerBinaryWriter.WriteInt32(writer, ValueType.GUID.GetHashCode());
        }

        /// <summary>
        /// Reads parameter from the binary stream
        /// </summary>
        /// <param name="reader"></param>
        public virtual void Read(BinaryReader reader)
        {

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// On before value of the parameter is changed
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnValueChanging(T value)
        {

        }

        #endregion
    }
}
