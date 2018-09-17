using System;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Command security level
    /// </summary>
    [Flags]
	public enum CommandSecurityLevel
    {
        /// <summary>
        /// Command can be executed by both authenticated and non authenticated users
        /// </summary>
		All = Authenticate | NotAuthenticate,
        /// <summary>
        /// Command can only be executed by authenticated users
        /// </summary>
		Authenticate = 0x01,
        /// <summary>
        /// Command can only be executed by not authenticed users
        /// </summary>
		NotAuthenticate = 0x02
    }
}
