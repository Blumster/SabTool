using System.Text;

namespace SabTool.CLI.Base;

public abstract class BaseCommand : ICommand
{
    public abstract string Key { get; }
    public abstract string Shortcut { get; }
    public abstract string Usage { get; }

    public void Setup() { }

    public abstract bool Execute(IEnumerable<string> arguments);

    public void BuildUsage(StringBuilder builder, IEnumerable<string> arguments)
    {
        _ = builder.AppendFormat(" {0}/{2} {1}", Key, Usage, Shortcut).AppendLine();
    }
}
