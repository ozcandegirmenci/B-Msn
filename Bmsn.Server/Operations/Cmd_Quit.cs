using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Quit command operation
    /// </summary>
	internal class Cmd_Quit : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_Quit()
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
			Client.Disconnect();
		}

        /// <summary>
        /// After command operation finished
        /// </summary>
		protected override void OnFinished()
		{
			var fireMe = Command.Parameters["fm"] as ParameterBoolean;
			Response = new NetResponse(true);
			if (fireMe != null && fireMe.Value)
			{
				base.OnFinished();
				Client.Disconnect();
			}
			else
			{
				Client.Disconnect();
				AfterResponseSend();
				Command = null;
			}
		}

        #endregion
    }
}
