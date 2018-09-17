namespace Bmsn.Protocol
{
    /// <summary>
    /// Socket statuses
    /// </summary>
    public enum SocketStatus
    {
        /// <summary>
        /// Unknown
        /// </summary>
        None = 0,
        /// <summary>
        /// Connecting to the target
        /// </summary>
        Connecting,
        /// <summary>
        /// Connected
        /// </summary>
        Connected,
        /// <summary>
        /// Disconnecting
        /// </summary>
        Disconnecting,
        /// <summary>
        /// Disconnected
        /// </summary>
        Disconnected,
        /// <summary>
        /// Is in error state
        /// </summary>
        Error
    }
}
