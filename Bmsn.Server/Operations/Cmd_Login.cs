using System;
using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Login command operation
    /// </summary>
	internal class Cmd_Login : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_Login()
		{
			SecurityLevel = CommandSecurityLevel.NotAuthenticate;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Applies operation
        /// </summary>
        protected override void DoOperation()
		{
			var uname = Command.Parameters["un"] as ParameterString;

			for (int i = 0; i < Server.Clients.Count; i++)
			{
				if (Server.Clients[i].Username == uname.Value)
				{
					Response = new NetResponse(false, "This username is already in use");
					return;
				}
			}

			Client.Username = uname.Value;
			Client.Status = ClientStatus.Active;

			Response = new NetResponse(true, "User logged in");

			// send other users that this user is online now ..
			for (int i = 0; i < Server.Clients.Count; i++)
			{
				if (Server.Clients[i] == Client)
					continue;

				NetCommand command = new NetCommand(CommandNames.U_LOGIN);
				command.Parameters.Add(new ParameterInt32("hash", Client.HashCode));
				command.Parameters.Add(new ParameterInt32("st", Convert.ToInt32(Client.Status)));
				command.Parameters.Add(uname);

				Server.Clients[i].Commands.Add(command);


				if (Server.Clients[i].Status != ClientStatus.None)
				{
					NetCommand command2 = new NetCommand(CommandNames.U_LOGIN);
					command2.Parameters.Add(new ParameterInt32("hash", Server.Clients[i].HashCode));
					command2.Parameters.Add(new ParameterInt32("st", Convert.ToInt32(Server.Clients[i].Status)));
					command2.Parameters.Add(new ParameterString("un", Server.Clients[i].Username));

					Client.Commands.Add(command2);
				}
			}
		}

        #endregion
    }
}
