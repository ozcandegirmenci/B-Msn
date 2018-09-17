using System;

namespace Bmsn.Protocol
{
    /// <summary>
    /// System Error event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SystemErrorEventHandler (object sender, SystemErrorEventArgs e);

    /// <summary>
    /// Represents event argument for the system error
    /// </summary>
    public class SystemErrorEventArgs : EventArgs
	{
        #region Properties

        /// <summary>
        /// Gets the exception object
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="ex"></param>
        public SystemErrorEventArgs(Exception ex)
		{
			Exception = ex;
		}

        #endregion
    }
}
