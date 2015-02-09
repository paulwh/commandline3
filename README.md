Command Line Parser Library 3 pre-release for .Net.
===
The Command Line Parser Library offers .Net applications a clean and concise API for handling command line arguments, detecting errors in the command line, and displaying help screens. Parsing command line arguments should be as simple as defining a data structure that contains the required data.
__This library provides _hassle free_ command line parsing.__

This library was inspired by the [CommandLine](https://github.com/gsscoder/commandline) and [CommandLine2](https://github.com/cosmo0/commandline) projects. Unfortunately the former hasn't seen much development and the latter has a number of egregious bugs. This library is a complete rewrite, preserving a very similar API to [CommandLine2](https://github.com/cosmo0/commandline).

Compatibility:
---
  - .NET Framework 3.5+

Current Release:
---
  - This code is currently __pre-release__, a significant amount of documentation, testing, and packaging still needs to be done.

At glance:
---
  - One line parsing using default singleton: ``CommandLine.Parser.Default.ParseArguments(...)``.
  - One line help screen generator: ``HelpText.AutoBuild(...)``.
  - Map command line arguments to sequences (``IEnumerable<T>``), enum or standard scalar types.
  - Extensible argument deserialization and validation.
  - Define verb based commands (i.e. ``git commit`` or ``git push``) (see ``Parser.ParseMultiVerbArguments``)
  - Mutually exclusive parameter sets
  - Flexible command line argument style (i.e. ``/F <file>`` or ``--file=<file>`` or ``-File <file>``) (see ``ParserSettings``)

To install:
---
  - Currently the only way to install this package is to build from source and manually copy binary. Nuget package comming soon.
  
Sample:
---
Define a class to receive command line values:

```csharp
class Options {
  [Option('r', "read", Required = true,
    HelpText = "Input files to be processed.")]
  public IEnumerable<string> InputFiles { get; set; }
    
  // omitting long name, default --verbose
  [Option(DefaultValue = true,
    HelpText = "Prints all messages to standard output.")]
  public bool Verbose { get; set; }

  // a position parameter may be referenced by name, or used in the appropriate position
  [Option('o', 0)]
  public int Offset { get; set;}
  }
}
```

Parse the command line and consume the resulting obejct:

```csharp
static void Main(string[] args) {
  var result = CommandLine.Parser.Default.ParseArguments<Options>(args);
  if (!result.Errors.Any()) {
    // Values are available here
    if (result.Value.Verbose) Console.WriteLine("Filenames: {0}", string.Join(",", result.Value.InputFiles.ToArray()));
  }
}
```

Known Issues:
---

  - OptionsAttribute needs a position only constructor and a parameterless constructor
  - VerbAttribute shouldn't be strictly required
  - Parameter validation is Not Yet Implemented
  - Default value assignment is Not Yet Implemented

Acknowledgements:
---

Thanks to Giacomo Stelluti Scala (@gsscoder) for prior work on [CommandLine](https://github.com/gsscoder/commandline) which was the inspiration for this library.
Thanks to Thomas (@cosmo0) for prior work on [CommandLine2](https://github.com/cosmo0/commandline) which was also inspirational for this library.

Main Contributors (alphabetical order):
- Paul Wheeler (@paulwh)

Resources for newcomers:
---
  - Quickstart - TODO
  - Wiki - TODO

Latest Changes:
---
  - Minor changes to improve consistency with existing CommandLine package. (@paulwh)

Contacts:
---
Paul Wheeler
  - paul.wheeler AT appature DOT com
