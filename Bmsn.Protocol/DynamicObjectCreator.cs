using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Dynamic object creator
    /// </summary>
    public class DynamicObjectCreator
    {
        /// <summary>
        /// Object creation delegate
        /// </summary>
        /// <returns></returns>
        private delegate object InvokeMethodHandler();
        
        private readonly InvokeMethodHandler handler = null;

        /// <summary>
        /// Initialize a new instance of this class for the target constructor
        /// </summary>
        /// <param name="targetMethod"></param>
        public DynamicObjectCreator(ConstructorInfo targetMethod)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty,
                typeof(object),
                new Type[0],
                targetMethod.DeclaringType);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

            ilGenerator.DeclareLocal(targetMethod.DeclaringType);

            ilGenerator.Emit(OpCodes.Newobj, targetMethod);
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            handler = (InvokeMethodHandler)dynamicMethod.CreateDelegate(typeof(InvokeMethodHandler));
        }

        /// <summary>
        /// Invokes the handler and created the object
        /// </summary>
        /// <returns></returns>
        public object Invoke()
        {
            return handler();
        }
    }
}
