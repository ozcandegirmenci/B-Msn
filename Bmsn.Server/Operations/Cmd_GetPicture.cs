using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Get client picture command operation
    /// </summary>
    public class Cmd_GetPicture : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_GetPicture()
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
            
			for (int i = 0; i < Server.Clients.Count; i++)
			{
				if (Server.Clients[i].Username == un.Value)
				{
					var command = new NetCommand(CommandNames.GET_PICTURE);
					Server.Clients[i].Commands.Add(command);

					if (command.WaitForStatus(CommandStatus.Executed, 10, 10000))
					{
						Response = new NetResponse(true);
						Response.Parameters.Add(new ParameterResponse("rp", command.Response));
					}
					else {
						Response = new NetResponse(false, "Server could not read picture from target client in the given time");
					}

					break;
				}
			}

            if (Response == null)
            {
                Response = new NetResponse(false, "Could not find client");
            }
		}

        #endregion
    }
}
