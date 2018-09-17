using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Bmsn.Protocol
{
    /// <summary>
    /// A collection of <see cref="IParameter"/> objects
    /// </summary>
    public class ParameterCollection : IEnumerable<IParameter>
    {
        #region Members

        private Dictionary<string, IParameter> _Items = new Dictionary<string, IParameter>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of items in this collection
        /// </summary>
        public int Count { get { return _Items.Count; } }

        /// <summary>
        /// Gets the parameter by its name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IParameter this[string key]
        {
            get { return _Items[key]; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ParameterCollection()
        { }

        #endregion

        #region Public Methods

        public void Add(IParameter parameter)
        {
            if (ContainsKey(parameter.Name))
                throw new Exception("There is already a parameter with this name");

            parameter.Parent = this;
            if (parameter.Name == string.Empty)
                parameter.Name = Count.ToString();
            _Items.Add(parameter.Name, parameter);
        }

        /// <summary>
        /// Adds a range of parameter items to this collection
        /// </summary>
        /// <param name="parameters"></param>
        public void AddRange(IParameter[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Add(parameters[i]);
            }
        }

        public void Write(BinaryWriter writer)
        {
            ServerBinaryWriter.WriteByte(writer, (byte)Count);
            foreach (var entry in this)
            {
                entry.Write(writer);
            }
        }

        public void Read(BinaryReader reader)
        {
            Clear();

            int count = (int)reader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                Add(ParameterFactory.Parse(reader));
            }
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            _Items.Clear();
        }

        /// <summary>
        /// Returns that is this collection contains aparameter with the given name or not?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _Items.ContainsKey(key);
        }

        public IEnumerator<IParameter> GetEnumerator()
        {
            return _Items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
