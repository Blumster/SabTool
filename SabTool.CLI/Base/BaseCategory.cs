using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabTool.CLI.Base
{
    public abstract class BaseCategory : ICommand
    {
        protected readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        public abstract string Key { get; }
        public abstract string Usage { get; }

        public virtual void Setup()
        {
            _commands.Clear();

            foreach (var type in GetType().GetNestedTypes())
            {
                if (!type.IsClass)
                    continue;

                if (type.GetInterfaces().Contains(typeof(ICommand)))
                {
                    var newInstance = Activator.CreateInstance(type) as ICommand;

                    _commands.Add(newInstance.Key, newInstance);

                    newInstance.Setup();
                }
            }
        }

        public virtual bool Execute(IEnumerable<string> arguments)
        {
            if (arguments.Count() == 0)
            {
                Console.WriteLine("ERROR: No subcommand specified command!");
                return false;
            }

            var nextKey = arguments.First();
            if (!_commands.ContainsKey(nextKey))
            {
                Console.WriteLine("ERROR: Unknown command!");
                return false;
            }

            return _commands[nextKey].Execute(arguments.Skip(1));
        }

        public virtual void BuildUsage(StringBuilder builder, IEnumerable<string> arguments)
        {
            builder.AppendFormat(" {0}", Key);

            if (arguments.Count() > 0)
            {
                var nextKey = arguments.First();
                if (!_commands.ContainsKey(nextKey))
                {
                    builder.Append(" <non-existant sub command specified>!");
                    return;
                }

                _commands[nextKey].BuildUsage(builder, arguments.Skip(1));
                return;
            }

            builder.Append(" <");
            var first = true;

            foreach (var command in _commands)
            {
                if (!first)
                {
                    builder.Append(" | ");
                }
                else
                    first = false;

                builder.Append(command.Key);
            }

            builder.Append(">");
        }

        protected void AddInstance(ICommand command)
        {
            _commands.Add(command.Key, command);

            command.Setup();
        }

        protected void AddInstance<T>()
            where T : ICommand, new()
        {
            AddInstance(new T());
        }
    }
}
