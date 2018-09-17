using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Represents a collection of commands which belongs to server
    /// </summary>
    public class ServerCommandCollection : CommandCollection
	{
        #region Properties

        /// <summary>
        /// Gets that is this command collection belongs to a server
        /// </summary>
        public override bool IsServer
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ServerCommandCollection()
		{

        }

        #endregion
    }
}
