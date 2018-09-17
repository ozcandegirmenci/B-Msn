using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Informs writing state of a user
    /// </summary>
	internal class Cmd_InformWritingState : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_InformWritingState()
		{
			SecurityLevel = CommandSecurityLevel.Authenticate;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Applies operation
        /// </summary>
        protected override void DoOperation()
		{
			var un = Command.Parameters["un"] as ParameterString;
			var st = Command.Parameters["st"] as ParameterBoolean;

			for (int i = 0; i < Server.Clients.Count; i++)
			{
				if (Server.Clients[i].Username == un.Value)
				{
					var command = new NetCommand(CommandNames.INFORM_WRITING_STATE);
					command.Parameters.Add(new ParameterString("un", Client.Username));
					command.Parameters.Add(st);

					Server.Clients[i].Commands.Add(command);
					Response = new NetResponse(true);
					return;
				}
			}

			Response = new NetResponse(false, "User could not found");
		}

        #endregion
    }
}
