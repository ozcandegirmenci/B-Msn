using System.Collections.Generic;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Manages command execution subscriptions and publishes
    /// </summary>
    public class CommandSubscriptionManager
    {
        #region Members

        public const string ALL_COMMAND_NAME_PREFIX = "*";

        private Dictionary<string, List<CommandExecuteHandler>> _Subscriptions = new Dictionary<string, List<CommandExecuteHandler>>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the subscriptions list for the given command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public List<CommandExecuteHandler> this[string command]
        {
            get
            {
                if (!ContainsCommand(command))
                {
                    return null;
                }
                return _Subscriptions[command] as List<CommandExecuteHandler>;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public CommandSubscriptionManager()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribe given method handler to all command executions
        /// </summary>
        /// <param name="methodHandler"></param>
        public void SubscribeGlobal(CommandExecuteHandler methodHandler)
        {
            Subscribe(ALL_COMMAND_NAME_PREFIX, methodHandler);
        }

        /// <summary>
        /// Subscribe given method handler to the given command name
        /// </summary>
        /// <param name="command"></param>
        /// <param name="methodHandler"></param>
        public void Subscribe(NetCommand command, CommandExecuteHandler methodHandler)
        {
            Subscribe(command.Name, methodHandler);
        }

        /// <summary>
        /// Subscribe given method handler to the given command name
        /// </summary>
        /// <param name="command"></param>
        /// <param name="methodHandler"></param>
        public void Subscribe(string command, CommandExecuteHandler methodHandler)
        {
            if (ContainsCommand(command))
            {
                List<CommandExecuteHandler> list = this[command];
                list.Add(methodHandler);
            }
            else
            {
                List<CommandExecuteHandler> list = new List<CommandExecuteHandler>();
                list.Add(methodHandler);
                _Subscriptions.Add(command, list);
            }
        }

        /// <summary>
        /// Unsubscribe method handler from given command name
        /// </summary>
        /// <param name="command"></param>
        /// <param name="methodHandler"></param>
        public void Unsubscribe(string command, CommandExecuteHandler methodHandler)
        {
            if (!ContainsCommand(command))
                return;

            this[command].Remove(methodHandler);
        }

        /// <summary>
        /// Unsubscribe method handler from given command name
        /// </summary>
        /// <param name="command"></param>
        /// <param name="methodHandler"></param>
        public void Unsubscribe(NetCommand command, CommandExecuteHandler methodHandler)
        {
            Unsubscribe(command.Name, methodHandler);
        }

        /// <summary>
        /// Unsubscribe given method handler from all command executed subscribtions
        /// </summary>
        /// <param name="methodHandler"></param>
        public void UnsubscribeGlobal(CommandExecuteHandler methodHandler)
        {
            List<CommandExecuteHandler> items = this[ALL_COMMAND_NAME_PREFIX];
            if (items != null)
            {
                items.Remove(methodHandler);
            }
        }

        /// <summary>
        /// Checks that is there any subscription for the given command name
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool ContainsCommand(string command)
        {
            return _Subscriptions.ContainsKey(command);
        }

        /// <summary>
        /// Clear all subscriptions to the given command
        /// </summary>
        /// <param name="command"></param>
        public void ClearSubscriptions(NetCommand command)
        {
            ClearSubscriptions(command.Name);
        }

        /// <summary>
        /// Clears all subscriptions of the given command anme
        /// </summary>
        /// <param name="command"></param>
        public void ClearSubscriptions(string command)
        {
            if (ContainsCommand(command))
            {
                _Subscriptions.Remove(command);
            }
        }

        /// <summary>
        /// Clear all subscriptions
        /// </summary>
        public void ClearAll()
        {
            _Subscriptions.Clear();
        }

        /// <summary>
        /// Publish a command executed to the subscriptions
        /// </summary>
        /// <param name="command"></param>
        public void PublishCommandExecute(NetCommand command)
        {
            Publish(command.Name, command);
            Publish(ALL_COMMAND_NAME_PREFIX, command);
            command.Status = CommandStatus.Executed;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Publishes given command executed for the given command key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        private void Publish(string name, NetCommand command)
        {
            List<CommandExecuteHandler> list = this[name];
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
                ((CommandExecuteHandler)list[i])(command);
        }

        #endregion
    }
}
