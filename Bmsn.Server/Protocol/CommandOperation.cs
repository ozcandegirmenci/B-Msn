using System;
using Bmsn.Protocol;

namespace Bmsn.Server
{
    /// <summary>
    /// Base operation class for Server commands
    /// </summary>
	public abstract class CommandOperation
    {
        #region Events

        /// <summary>
        /// Occurs when command operation is finished
        /// </summary>
        public event CommandOperationEventHandler Finished;
        /// <summary>
        /// Occurs when command operation is canceled
        /// </summary>
        public event CommandOperationEventHandler Canceled;

        #endregion

        #region Members

        private CommandOperationStatus _Status = CommandOperationStatus.Unknown;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ServerClient"/> object which this command operation is belongs to
        /// </summary>
        public ServerClient Client
        {
            get
            {
                return Parent.Parent;
            }
        }

        /// <summary>
        /// Gets the <see cref="NetServer"/> object
        /// </summary>
        public NetServer Server
        {
            get
            {
                return Parent.Parent.Server;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="NetCommand"/> object
        /// </summary>
        public NetCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="NetResponse"/> object which will be sended as the response for the command
        /// </summary>
        public NetResponse Response
        {
            get
            {
                if (Command == null)
                    return null;
                return Command.Response;
            }
            set
            {
                Command.Response = value;
            }
        }

        /// <summary>
        /// Gets or sets the status of the command operation
        /// </summary>
        public CommandOperationStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (value == _Status)
                    return;
                _Status = value;
                if (value == CommandOperationStatus.Finished)
                    OnFinished();
                else if (value == CommandOperationStatus.Canceled)
                    OnCanceled();
            }
        }

        /// <summary>
        /// Gets or sets the security level of the command operation
        /// </summary>
        public CommandSecurityLevel SecurityLevel { get; set; } = CommandSecurityLevel.All;

        /// <summary>
        /// Gets or sets the parent collection of this operation
        /// </summary>
        public CommandOperationCollection Parent { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public CommandOperation()
        {

        }

        /// <summary>
        /// Initialize a new instance of this class with the provided parent
        /// </summary>
        /// <param name="parent"></param>
        public CommandOperation(CommandOperationCollection parent)
        {
            Parent = parent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the command operation
        /// </summary>
        public void StartOperation()
        {
            Status = CommandOperationStatus.OnOperation;
            try
            {
                if (CheckSecurity())
                    DoOperation();
                else
                    Response = NetResponse.NotAllowed();
            }
            catch (Exception ex)
            {
                Response = NetResponse.SystemError(ex.Message);
            }
            finally
            {
                Status = CommandOperationStatus.Finished;
            }
        }

        /// <summary>
        /// Checks the command operation security
        /// </summary>
        /// <returns></returns>
        public bool CheckSecurity()
        {
            switch (SecurityLevel)
            {
                case CommandSecurityLevel.All:
                    return true;
                case CommandSecurityLevel.Authenticate:
                    if (Client.Status == ClientStatus.None)
                    {
                        Response.SetError("You can not run this command in this state, please login");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case CommandSecurityLevel.NotAuthenticate:
                    if (Client.Status == ClientStatus.None)
                    {
                        return true;
                    }
                    else
                    {
                        Response.SetError("You can not run this command int his state, please sign out first");
                        return false;
                    }
                default:
                    return true;
            }
        }

        /// <summary>
        /// Cancels the operation
        /// </summary>
        public void Cancel()
        {
            Response.SetError("Operation Canceled");
            Status = CommandOperationStatus.Canceled;
        }

        /// <summary>
        /// Releases all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Internally do the operation
        /// </summary>
        protected virtual void DoOperation()
        { }

        /// <summary>
        /// Occurs on command operation canceled
        /// </summary>
        protected virtual void OnCanceled()
        {
            if (Canceled != null)
                Canceled(this, new CommandOperationEventArgs(this));
        }

        /// <summary>
        /// Occurs when the command operation completed
        /// </summary>
        protected virtual void OnFinished()
        {
            Client.Send(Response);
            AfterResponseSend();

            if (Finished != null)
                Finished(this, new CommandOperationEventArgs(this));


            Command = null;
        }

        /// <summary>
        /// Occurs after command response sended to the target
        /// </summary>
        protected virtual void AfterResponseSend()
        {

        }

        /// <summary>
        /// Release all unmanaged resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
