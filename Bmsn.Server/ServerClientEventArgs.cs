using System;

namespace Bmsn.Server
{
    /// <summary>
    /// Event handler for <see cref="ServerClient"/> events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	public delegate void ServerClientEventHandler(object sender, ServerClientEventArgs e);

    /// <summary>
    /// Event argument class for <see cref="ServerClient"/> events
    /// </summary>
    public class ServerClientEventArgs : EventArgs
	{
        #region Properties

        /// <summary>
        /// Gets the server client
        /// </summary>
        public ServerClient Client { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="client"></param>
        public ServerClientEventArgs(ServerClient client)
		{
			Client = client;
		}

        #endregion
    }
}
