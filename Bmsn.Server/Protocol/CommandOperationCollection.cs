using System;
using System.Collections;
using System.Collections.Generic;

namespace Bmsn.Server
{
    /// <summary>
    /// A collection of <see cref="CommandOperation"/> instances
    /// </summary>
	public class CommandOperationCollection : IEnumerable<CommandOperation>
    {
        #region Events

        public event CommandOperationEventHandler CommandAdded;
        public event CommandOperationEventHandler CommandRemoved;
        public event CommandOperationEventHandler CommandFinished;
        public event CommandOperationEventHandler CommandCanceled;

        #endregion

        #region Members

        private readonly List<CommandOperation> _Operations = new List<CommandOperation>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ServerClient"/> object that owns this collection
        /// </summary>
        public ServerClient Parent { get; private set; }

        /// <summary>
        /// Gets the items at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CommandOperation this[int index]
        {
            get
            {
                return _Operations[index] as CommandOperation;
            }
        }

        /// <summary>
        /// Gets the number of items in the collection
        /// </summary>
        public int Count
        {
            get { return _Operations.Count; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="parent"></param>
        public CommandOperationCollection(ServerClient parent)
		{
			Parent = parent;
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds given item to the collection
        /// </summary>
        /// <param name="value"></param>
        public void Add(CommandOperation value)
		{
			value.Parent = this;

			value.Finished += new CommandOperationEventHandler(OperationFinished);
			value.Canceled += new CommandOperationEventHandler(OperationCanceled);

			_Operations.Add(value);
			OnCommandAdded(value);
		}

        /// <summary>
        /// Removes given item from the collection
        /// </summary>
        /// <param name="value"></param>
		public void Remove(CommandOperation value)
		{
			_Operations.Remove(value);
			OnCommandRemoved(value);
		}

        /// <summary>
        /// Removes item at the specified index
        /// </summary>
        /// <param name="index"></param>
		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

        /// <summary>
        /// Clears item from the collection
        /// </summary>
		public void Clear()
		{
            _Operations.Clear();
		}

        /// <summary>
        /// Checks that is given command operation exists in the collection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public bool Contains(CommandOperation value)
		{
			return _Operations.Contains(value);
		}

        /// <summary>
        /// Returns the index of the given item in the collection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public int IndexOf(CommandOperation value)
		{
			return _Operations.IndexOf(value);
		}

        /// <summary>
        /// Returns an instance of <see cref="IEnumerator"/> which provides enumerating through this collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CommandOperation> GetEnumerator()
        {
            return _Operations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Protected Methods

        protected virtual void OnCommandRemoved(CommandOperation value)
		{
			if (CommandRemoved != null)
				CommandRemoved(this, new CommandOperationEventArgs(value));
		}

		protected virtual void OnCommandAdded(CommandOperation value)
		{
			if (CommandAdded != null)
				CommandAdded(this, new CommandOperationEventArgs(value));
		}

		protected virtual void OnCommandFinished(CommandOperation value)
		{
			if (CommandFinished != null)
				CommandFinished(this, new CommandOperationEventArgs(value));

			Remove(value);
			value.Dispose();
		}

		protected virtual void OnCommandCanceled(CommandOperation value)
		{
			if (CommandCanceled != null)
				CommandCanceled(this, new CommandOperationEventArgs(value));
		}

        #endregion

        #region Private Methods

        /// <summary>
        /// Operation finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperationFinished(object sender, CommandOperationEventArgs e)
		{
			OnCommandFinished(e.Operation);
		}

        /// <summary>
        /// Operation canceled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OperationCanceled(object sender, CommandOperationEventArgs e)
		{
			OnCommandCanceled(e.Operation);
		}

        #endregion
    }
}
