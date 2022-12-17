﻿using AnsiVtConsole.NetCore;

using FileDuplicateAnalyzer.Services.Text;

using Microsoft.Extensions.Configuration;

namespace FileDuplicateAnalyzer.Commands.MakeDb;

/// <summary>
/// build a file db
/// </summary>
internal sealed class MakeDbCommand : Command
{
    public MakeDbCommand(
        IConfiguration config,
        IAnsiVtConsole output,
        Texts texts) :
            base(config, output, texts, 0, 0)
    {
    }

    protected override int Execute(string[] args)
        => throw new NotImplementedException();
}
