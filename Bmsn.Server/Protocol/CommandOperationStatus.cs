namespace Bmsn.Server
{
    /// <summary>
    /// Represents the status of a command operation
    /// </summary>
    public enum CommandOperationStatus
    {
        /// <summary>
        /// Operation status is unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Operation is in waiting state
        /// </summary>
        Waiting,
        /// <summary>
        /// Operation is current processing
        /// </summary>
        OnOperation,
        /// <summary>
        /// Operation is finished
        /// </summary>
        Finished,
        /// <summary>
        /// Operation is canceled
        /// </summary>
        Canceled
    }
}
