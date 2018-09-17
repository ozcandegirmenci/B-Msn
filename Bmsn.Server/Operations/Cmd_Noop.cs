using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// No operation command operation
    /// </summary>
	internal class Cmd_Noop : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_Noop()
		{
			SecurityLevel = CommandSecurityLevel.All;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Applies operation
        /// </summary>
        protected override void DoOperation()
		{
			Response = new NetResponse(true, string.Empty);
		}

        #endregion
    }
}
