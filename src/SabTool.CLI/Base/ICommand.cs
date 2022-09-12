﻿using System.Text;

namespace SabTool.CLI.Base;

public interface ICommand
{
    string Key { get; }
    string Shortcut { get; }
    string Usage { get; }

    void Setup();
    bool Execute(IEnumerable<string> arguments);
    void BuildUsage(StringBuilder builder, IEnumerable<string> arguments);
}
