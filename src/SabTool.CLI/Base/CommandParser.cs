﻿using System.Diagnostics;
using System.Reflection;
using System.Text;

using SabTool.Utils.Extensions;

namespace SabTool.CLI.Base;
public static class CommandParser
{
    private const string CommandNamespace = "SabTool.CLI.Commands";

    private static readonly Dictionary<string, ICommand> _commands = new();

    static CommandParser()
    {
        static bool filter(Type a) => a.Namespace == CommandNamespace && a.IsClass && !a.IsNested && a.GetInterfaces().Contains(typeof(ICommand));

        foreach (Type? type in Assembly.GetExecutingAssembly().GetTypes().Where(filter))
        {
            if (Activator.CreateInstance(type) is not ICommand newInstance)
            {
                Console.WriteLine($"ERROR: Command class \"{type.Name}\" cannot be instantiated! Skipping command...");
                return;
            }

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

        string? commandKey = commandParts.FirstOrDefault();
        if (commandKey == null || !_commands.ContainsKey(commandKey))
        {
            Console.WriteLine("ERROR: Unknown command!");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.Write("<");

            bool first = true;

            foreach (KeyValuePair<string, ICommand> command in _commands)
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

        bool res = false;

        // If the application is being debugged, let the exception fall through to the debugger to check it
        if (Debugger.IsAttached)
        {
            res = _commands[commandKey].Execute(commandParts.Skip(1));
        }
        else
        {
            try
            {
                res = _commands[commandKey].Execute(commandParts.Skip(1));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured while running the command! Exception: {0}", e);
            }
        }

        if (!res)
        {
            Console.WriteLine();
            Console.WriteLine("Command could not be ran! Usage:");

            StringBuilder sb = new();
            _commands[commandKey].BuildUsage(sb, commandParts.Skip(1));

            Console.WriteLine(sb.ToString().Trim());
        }

        return res;
    }
}
