using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Update profile picture command operation for clients
    /// </summary>
    public class Cmd_ClientUpdatePicture : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_ClientUpdatePicture()
		{
			SecurityLevel = CommandSecurityLevel.Authenticate;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Applies innner operation
        /// </summary>
        protected override void DoOperation()
		{
			var un = new ParameterString("un", Client.Username);

			for (int i = 0; i < Server.Clients.Count; i++)
			{
				if (Server.Clients[i] != Client)
				{
					var command = new NetCommand(CommandNames.CLIENT_UPDATE_PICTURE);
					command.Parameters.Add(un);

					Server.Clients[i].Commands.Add(command);
				}
			}

			Response = new NetResponse(true);
		}

        #endregion
    }
}
