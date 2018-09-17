namespace Bmsn.Protocol
{
    /// <summary>
    /// Represents a method signature which handles command executions
    /// </summary>
    /// <param name="command"></param>
    public delegate void CommandExecuteHandler (NetCommand command);
}
