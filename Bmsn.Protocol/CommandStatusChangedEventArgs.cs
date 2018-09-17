using System;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CommandStatusChangedEventHandler(object sender, CommandStatusChangedEventArgs e);

    /// <summary>
    /// Event argument class which is for the command status changed
    /// </summary>
    public class CommandStatusChangedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the command that is status is changed
        /// </summary>
        public NetCommand Command { get; private set; }

        /// <summary>
        /// Gets the Command's old status
        /// </summary>
        public CommandStatus OldStatus { get; private set; }

        /// <summary>
        /// Gets the date of the command status changed
        /// </summary>
        public DateTime Date { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this command
        /// </summary>
        private CommandStatusChangedEventArgs()
        {
            Date = DateTime.Now;
        }

        /// <summary>
        /// Initialize a new instance of this command with the provided command
        /// </summary>
        /// <param name="command"></param>
        public CommandStatusChangedEventArgs(NetCommand command)
        {
            Command = command;
        }

        /// <summary>
        /// Initialize a new instance of this command with the provided command and old status
        /// </summary>
        /// <param name="command"></param>
        /// <param name="old"></param>
        public CommandStatusChangedEventArgs(NetCommand command, CommandStatus old)
            : this(command)
        {
            OldStatus = old;
        }

        /// <summary>
        /// Initialize a new instance of this command with the provided command, old status and status changed date
        /// </summary>
        /// <param name="command"></param>
        /// <param name="old"></param>
        /// <param name="date"></param>
        public CommandStatusChangedEventArgs(NetCommand command, CommandStatus old,
            DateTime date)
            : this(command, old)
        {
            Date = date;
        }

        #endregion
    }
}
