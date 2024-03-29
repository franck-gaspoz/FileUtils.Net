﻿using System.Collections.ObjectModel;

namespace FileUtils.Net.Services.CmdLine;

class CommandLineArgs
{
    public ReadOnlyCollection<string> Args { get; private set; }

    public CommandLineArgs(List<string> args)
        => Args = new ReadOnlyCollection<string>(args);
}
