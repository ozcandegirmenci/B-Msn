using System.Collections.Generic;

namespace Bmsn.Protocol
{
    /// <summary>
    /// A collection of <see cref="NetCommand"/> objects
    /// </summary>
    public class CommandCollection
    {
        #region Members

        private List<NetCommand> _Items;
        private int LastCommandId;
        private Dictionary<int, NetCommand> _Ids;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the active command
        /// </summary>
        public NetCommand Active { get; set; }

        /// <summary>
        /// Gets the number of commands
        /// </summary>
        public int Count
        {
            get { return _Items.Count; }
        }

        /// <summary>
        /// Gets the command according to its Id
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public NetCommand this[int commandId]
        {
            get { return _Ids[commandId] as NetCommand; }
        }

        /// <summary>
        /// Gets the command by its id or its index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="useIndex"></param>
        /// <returns></returns>
        public NetCommand this[int index, bool useIndex]
        {
            get
            {
                if (useIndex)
                    return _Items[index] as NetCommand;
                else
                    return this[index];
            }
        }

        /// <summary>
        /// Checks that is there any active command
        /// </summary>
        public bool HasActive
        {
            get { return (Active != null); }
        }

        /// <summary>
        /// Gets that is this server command collection or client command collection
        /// </summary>
        public virtual bool IsServer
        {
            get { return false; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public CommandCollection()
        {
            _Items = new List<NetCommand>();
            _Ids = new Dictionary<int, NetCommand>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds given command to the collection
        /// </summary>
        /// <param name="command"></param>
        public void Add(NetCommand command)
        {
            command.Id = GenerateId();
            command.Status = CommandStatus.Waiting;
            command.Parent = this;
            _Items.Add(command);
            _Ids.Add(command.Id, command);
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            _Ids.Clear();
            _Items.Clear();
        }

        /// <summary>
        /// Removes given item from the command collection
        /// </summary>
        /// <param name="command"></param>
        public void Remove(NetCommand command)
        {
            _Items.Remove(command);
            if (command.Id == 0)
                return;
            _Ids.Remove(command.Id);
        }

        /// <summary>
        /// Checks that is this collection contains a command with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id)
        {
            return (_Ids.ContainsKey(id));
        }

        /// <summary>
        /// Moves to the next command
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (Active != null)
                return true;

            for (int i = 0; i < _Items.Count; i++)
            {
                NetCommand command = this[i, true];
                if (command.Status == CommandStatus.Waiting)
                {
                    Active = command;
                    break;
                }
            }

            return (Active != null);
        }

        /// <summary>
        /// Generates a new command id
        /// </summary>
        /// <returns></returns>
        protected int GenerateId()
        {
            lock (this)
            {
                int max = 0x70000000;
                int min = 0x00000001;
                if (IsServer)
                {
                    max = 0x7ddddddd;
                    min = 0x70000000;
                }

                if (LastCommandId < max && LastCommandId >= min)
                    LastCommandId++;
                else
                    LastCommandId = min;

                while (Contains(LastCommandId))
                {
                    if (LastCommandId < max && LastCommandId >= min)
                        LastCommandId++;
                    else
                        LastCommandId = min;
                }
            }
            return LastCommandId;
        }

        /// <summary>
        /// Checks that is command is a server command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool IsServerCommand(NetCommand command)
        {
            return IsServerCommand(command.Id);
        }

        /// <summary>
        /// Checks that is command is a server command
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsServerCommand(int id)
        {
            if (id >= 0x70000000 && id < 0x7ddddddd)
                return true;
            return false;
        }

        #endregion
    }
}
