using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SabTool.CLI.Base
{
    using Utils.Extensions;

    public static class CommandParser
    {
        const string CommandNamespace = "SabTool.CLI.Commands";

        private static readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        static CommandParser()
        {
            static bool filter(Type a) => a.Namespace == CommandNamespace && a.IsClass && !a.IsNested && a.GetInterfaces().Contains(typeof(ICommand));

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(filter))
            {
                var newInstance = Activator.CreateInstance(type) as ICommand;

                if (_commands.ContainsKey(newInstance.Key))
                {
                    Console.WriteLine($"ERROR: Command key {newInstance.Key} is already defined in the commands list! Skipping command...");
                    continue;
                }

                if (_commands.ContainsKey(newInstance.Shortcut))
                {
                    Console.WriteLine($"ERROR: Command shortcut {newInstance.Shortcut} is already defined in the commands list! Skipping command...");
                    continue;
                }

                if (newInstance.Key == "exit")
                {
                    Console.WriteLine($"ERROR: Command {newInstance.Key} tried to override the exit command! Skipping command...");
                    continue;
                }

                if (newInstance.Shortcut == "e")
                {
                    Console.WriteLine($"ERROR: Command {newInstance.Key} has the same shortcut as the exit command! Skipping command...");
                    continue;
                }

                _commands.Add(newInstance.Key, newInstance);
                _commands.Add(newInstance.Shortcut, newInstance);

                newInstance.Setup();
            }
        }

        private static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;
            bool splitter(char c)
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            }

            return commandLine.Split(splitter).Select(arg => arg.Trim().TrimMatchingQuotes('\"')).Where(arg => !string.IsNullOrEmpty(arg));
        }

        public static bool ExecuteCommand(string command)
        {
            return ExecuteCommand(SplitCommandLine(command));
        }

        public static bool ExecuteCommand(IEnumerable<string> commandParts)
        {
            Console.WriteLine();

            var commandKey = commandParts.FirstOrDefault();
            if (!_commands.ContainsKey(commandKey))
            {
                Console.WriteLine("ERROR: Unknown command!");
                Console.WriteLine();
                Console.WriteLine("Available commands:");
                Console.Write("<");

                var first = true;

                foreach (var command in _commands)
                {
                    // Don't list the shortcuts
                    if (command.Key == command.Value.Shortcut)
                        continue;

                    if (!first)
                    {
                        Console.Write(" | ");
                    }
                    else
                        first = false;

                    Console.Write($"{command.Key}/{command.Value.Shortcut}");
                }

                Console.WriteLine(" | exit/e>");
                Console.WriteLine();

                return false;
            }

            var res = false;

            try
            {
                res = _commands[commandKey].Execute(commandParts.Skip(1));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured while running the command! Exception: {0}", e);
            }
            
            if (!res)
            {
                Console.WriteLine();
                Console.WriteLine("Command could not be ran! Usage:");

                var sb = new StringBuilder();
                _commands[commandKey].BuildUsage(sb, commandParts.Skip(1));

                Console.WriteLine(sb.ToString().Trim());
            }

            return res;
        }
    }
}
