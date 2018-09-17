namespace Bmsn.Protocol
{
    /// <summary>
    /// Command names
    /// </summary>
	public static class CommandNames
	{
        /// <summary>
        /// No operation command
        /// </summary>
		public const string NOOP = "N";
        /// <summary>
        /// Quit command
        /// </summary>
		public const string QUIT = "Q";
        /// <summary>
        /// Login command
        /// </summary>
		public const string LOGIN = "L";
        /// <summary>
        /// User log-in information command
        /// </summary>
		public const string U_LOGIN = "UL";
        /// <summary>
        /// User disconnected information command
        /// </summary>
		public const string U_DISCONNECT = "UD";
        /// <summary>
        /// User status changed command
        /// </summary>
		public const string U_STATUS_CHANGED = "USC";
        /// <summary>
        /// User send message command
        /// </summary>
		public const string SEND_MESSAGE = "SM";
        /// <summary>
        /// Get user picture command
        /// </summary>
		public const string GET_PICTURE = "GP";
        /// <summary>
        /// Client picture changed information command
        /// </summary>
		public const string CLIENT_UPDATE_PICTURE = "CUP";
        /// <summary>
        /// User writing message status changed information command
        /// </summary>
		public const string INFORM_WRITING_STATE = "IWS";
	}
}
