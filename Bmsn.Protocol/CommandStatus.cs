namespace Bmsn.Protocol
{
    /// <summary>
    /// Status of a <see cref="NetCommand"/> object
    /// </summary>
    public enum CommandStatus
    {
        /// <summary>
        /// Status is unknown
        /// </summary>
        Unknown = 0x00,
        /// <summary>
        /// Command is waiting to send
        /// </summary>
        Waiting,
        /// <summary>
        /// Command is sending to the target
        /// </summary>
        SendingToTarget,
        /// <summary>
        /// Command is waiting for response
        /// </summary>
        WaitingForResponse,
        /// <summary>
        /// Command is taking response
        /// </summary>
        TakingResponse,
        /// <summary>
        /// Command is executed
        /// </summary>
        Executed
    }
}
