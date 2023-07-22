using System.Text;

namespace SabTool.CLI.Base;

public abstract class BaseCategory : ICommand
{
    protected readonly Dictionary<string, ICommand> _commands = new();

    public abstract string Key { get; }
    public abstract string Shortcut { get; }
    public abstract string Usage { get; }

    public virtual void Setup()
    {
        _commands.Clear();

        foreach (Type type in GetType().GetNestedTypes())
            SetupType(type);
    }

    public void SetupWithTypes(params Type[] types)
    {
        _commands.Clear();

        foreach (Type type in GetType().GetNestedTypes())
            SetupType(type);

        foreach (Type type in types)
            SetupType(type);
    }

    private void SetupType(Type type)
    {
        if (!type.IsClass)
            return;

        if (type.GetInterfaces().Contains(typeof(ICommand)))
        {
            if (Activator.CreateInstance(type) is not ICommand newInstance)
            {
                Console.WriteLine($"ERROR: Command {Key} cannot be instantiated! Skipping command...");
                return;
            }

            if (_commands.ContainsKey(newInstance.Key))
            {
                Console.WriteLine($"ERROR: Command {Key} already has subcommand key {newInstance.Key} defined in the commands list! Skipping command...");
                return;
            }

            if (_commands.ContainsKey(newInstance.Shortcut))
            {
                Console.WriteLine($"ERROR: Command {Key} already has subcommand shortcut {newInstance.Shortcut} defined in the commands list! Skipping command...");
                return;
            }

            _commands.Add(newInstance.Key, newInstance);
            _commands.Add(newInstance.Shortcut, newInstance);

            newInstance.Setup();
        }
    }

    public virtual bool Execute(IEnumerable<string> arguments)
    {
        if (!arguments.Any())
        {
            Console.WriteLine("ERROR: No subcommand specified command!");
            return false;
        }

        string nextKey = arguments.First();
        if (!_commands.ContainsKey(nextKey))
        {
            Console.WriteLine("ERROR: Unknown command!");
            return false;
        }

        return _commands[nextKey].Execute(arguments.Skip(1));
    }

    public virtual void BuildUsage(StringBuilder builder, IEnumerable<string> arguments)
    {
        _ = builder.AppendFormat(" {0}", Key);

        if (arguments.Any())
        {
            string nextKey = arguments.First();
            if (!_commands.ContainsKey(nextKey))
            {
                _ = builder.Append(" <non-existant sub command specified>!");
                return;
            }

            _commands[nextKey].BuildUsage(builder, arguments.Skip(1));
            return;
        }

        _ = builder.Append(" <");
        bool first = true;

        foreach (KeyValuePair<string, ICommand> command in _commands)
        {
            // Don't list the shortcuts
            if (command.Key == command.Value.Shortcut)
                continue;

            if (!first)
            {
                _ = builder.Append(" | ");
            }
            else
                first = false;

            _ = builder.Append($"{command.Key}/{command.Value.Shortcut}");
        }

        _ = builder.Append('>');
    }

    protected void AddInstance(ICommand command)
    {
        _commands.Add(command.Key, command);
        _commands.Add(command.Shortcut, command);

        command.Setup();
    }

    protected void AddInstance<T>()
        where T : ICommand, new()
    {
        AddInstance(new T());
    }
}
