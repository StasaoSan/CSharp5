namespace Application.Parser;

public class PasswordHandler : CommandLineHandler
{
    public override void Handle(string[] args, CommandLineArgs commandLineArgs, ref int position)
    {
        if (args[position] == "-p" && position + 1 < args.Length)
            commandLineArgs.Password = args[++position].Trim('"');
        else
            NextHandler?.Handle(args, commandLineArgs, ref position);
    }
}
