using Bmsn.Protocol;
using Bmsn.Server;

namespace Bmsn.Server
{
    /// <summary>
    /// Send message command opeation
    /// </summary>
	internal class Cmd_SendMessage : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_SendMessage()
		{
			SecurityLevel = CommandSecurityLevel.Authenticate;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Aplies operation
        /// </summary>
        protected override void DoOperation()
		{
			var un = Command.Parameters["un"] as ParameterString;
			var msg = Command.Parameters["msg"] as ParameterString;
            
			for (int i = 0; i < Server.Clients.Count; i++)
			{
				if (Server.Clients[i].Username == un.Value)
				{
					var command = new NetCommand(CommandNames.SEND_MESSAGE);
					command.Parameters.Add(un);
					command.Parameters.Add(new ParameterString("fr", Client.Username));
					command.Parameters.Add(msg);

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
