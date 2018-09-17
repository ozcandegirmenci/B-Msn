using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// User status change command operation
    /// </summary>
	internal class Cmd_UserStatusChanged : CommandOperation
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public Cmd_UserStatusChanged()
		{
			SecurityLevel = CommandSecurityLevel.Authenticate;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Applies command operation
        /// </summary>
        protected override void DoOperation()
		{
			var st = Command.Parameters["st"] as ParameterInt32;

			if (st == null)
			{
				Response = new NetResponse(false, "Parameter null, st");
			}
			else
			{
				ClientStatus status = (ClientStatus)st.Value;
				Client.Status = status;

				for (int i = 0; i < Server.Clients.Count; i++)
				{
					if (Server.Clients[i] == Client)
						continue;

					var command = new NetCommand(CommandNames.U_STATUS_CHANGED);
					command.Parameters.Add(new ParameterInt32("hash", Client.HashCode));
					command.Parameters.Add(st);

					Server.Clients[i].Commands.Add(command);
				}

				Response = new NetResponse(true);
			}
		}

        #endregion
    }
}
