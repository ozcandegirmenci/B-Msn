using System;
using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// A delegate for the command operation events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	public delegate void CommandOperationEventHandler(object sender, CommandOperationEventArgs e);
    
    /// <summary>
    /// Event argument class for the command operation events
    /// </summary>
	public class CommandOperationEventArgs : EventArgs
	{
		private CommandOperation _Operation;

        #region Properties

        /// <summary>
        /// Gets the command
        /// </summary>
        public NetCommand Command { get; private set; }

        /// <summary>
        /// Gets or sets that is command is handled and an operation is assigned to it or not?
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CommandOperation"/> associated with the given command
        /// </summary>
        public CommandOperation Operation
        {
            get { return _Operation; }
            set
            {
                if (value != null)
                    Handled = true;
                else
                    Handled = false;
                _Operation = value;
                _Operation.Command = Command;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class with the provided operation
        /// </summary>
        /// <param name="operation"></param>
		public CommandOperationEventArgs(CommandOperation operation)
		{
			Operation = operation;
			Command = operation.Command;
		}

        /// <summary>
        /// Initialize a new instance of this class with the provided command
        /// </summary>
        /// <param name="command"></param>
		public CommandOperationEventArgs(NetCommand command)
		{
			Command = command;
		}

        /// <summary>
        /// Initialize a new instance of this class with the provided command and operation
        /// </summary>
        /// <param name="command"></param>
        /// <param name="operation"></param>
		public CommandOperationEventArgs(NetCommand command, CommandOperation operation)
		{
			Command = command;
			Operation = operation;
		}

        #endregion
    }
}
