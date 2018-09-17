namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents status of a client
    /// </summary>
	public enum ClientStatus
    {
        /// <summary>
        /// Client status is unknown
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Client is in active status
        /// </summary>
        Active = 0x01,
        /// <summary>
        /// Client is out to lunch
        /// </summary>
        OutToLunch = 0x02,
        /// <summary>
        /// Client is in stand by mode
        /// </summary>
        StandBy = 0x03,
        /// <summary>
        /// Client is in waiting status
        /// </summary>
        Waiting = 0x04
    }
}
