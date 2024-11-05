namespace SpreadsheetEngine;

public interface ICommand
{
    /// <summary>
    /// Function that executes the command.
    /// </summary>
    public void Execute();

    /// <summary>
    /// Function that undoes the command.
    /// </summary>
    public void Unexecute();

    /// <summary>
    /// Message that represents what the command does.
    /// </summary>
    /// <returns>Message representing this command.</returns>
    public string Message();
}