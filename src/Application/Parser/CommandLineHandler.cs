namespace Application.Parser;

public abstract class CommandLineHandler
{
    protected CommandLineHandler? NextHandler { get; private set; }

    public void SetNext(CommandLineHandler handler)
    {
        NextHandler = handler;
    }

    public abstract void Handle(string[] args, CommandLineArgs commandLineArgs, ref int position);
}
