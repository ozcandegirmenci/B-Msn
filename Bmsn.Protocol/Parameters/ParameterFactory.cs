using System.Collections.Generic;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Creates parameters from given binary stream
    /// </summary>
    public static class ParameterFactory
    {
        #region Members

        private static readonly Dictionary<int, DynamicObjectCreator> ParameterTypeMap;

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        static ParameterFactory()
        {
            ParameterTypeMap = new Dictionary<int, DynamicObjectCreator>();
            var types = typeof(IParameter).Assembly.GetTypes();
            foreach (var type in types)
            {
                if (typeof(IParameter).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    var valueType = type.BaseType.GetGenericArguments()[0];
                    ParameterTypeMap.Add(valueType.GUID.GetHashCode(), new DynamicObjectCreator(type.GetConstructor(System.Type.EmptyTypes)));
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a new IParameter from the given binary reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IParameter Parse(BinaryReader reader)
        {
            var name = ServerBinaryReader.ReadTinyString(reader);
            var type = ServerBinaryReader.ReadInt32(reader);

            var cinfo = ParameterTypeMap[type]
                    as DynamicObjectCreator;
            var param = cinfo.Invoke() as IParameter;
            param.Name = name;
            param.Read(reader);
            return param;
        }

        #endregion
    }
}
