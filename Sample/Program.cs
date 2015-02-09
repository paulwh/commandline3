using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Sample {
    class Program {
        static int Main(string[] args) {
            Console.WriteLine(
                "Received the following arguments: \"" +
                String.Join("\", \"", args) + "\""
            );

            var result = Parser.Default.ParseMultiVerbArguments<Options>(args);

            if (!result.Errors.Any()) {
                Console.WriteLine("Parser found verb: " + result.Verb);

                var options = result.Value;
                switch (result.Verb.ToLower()) {
                    case "foo":
                        Console.WriteLine("Switch: {0}", options.Foo.Switch);
                        Console.WriteLine("Required: {0}", options.Foo.Required ?? "<null>");
                        Console.WriteLine("Collection: {0}", options.Foo.Collection == null ? "<null>" : String.Join(", ", options.Foo.Collection));
                        break;
                    case "bar":
                        Console.WriteLine("Ordered: {0}", options.Bar.Ordered);
                        Console.WriteLine("Catch-All: {0}", options.Bar.CatchAll);
                        Console.WriteLine("First: {0}", options.Bar.First ?? "<null>");
                        Console.WriteLine("Second: {0}", options.Bar.Second ?? "<null>");
                        Console.WriteLine("All: {0}", options.Bar.All == null ? "<null>" : String.Join(", ", options.Bar.All));
                        break;
                }

                return 0;
            } else {
                return -1;
            }
        }
    }

    public class Options {
        public FooCommand Foo { get; set; }

        public BarCommand Bar { get; set; }
    }

    [Verb("foo", HelpText = "The foo command")]
    public class FooCommand {
        [Option("switch", 's', HelpText = "An example of a switch")]
        public bool Switch { get; set; }

        [Option("required", 'r', Required = true, HelpText = "An example of a required argument")]
        public string Required { get; set; }

        [Option("collection", 'c', 1, HelpText = "A positional collection parameter can be used to collect all remaining arguments.")]
        public string[] Collection { get; set; }

    }

    [Verb("bar", HelpText = "The bar command")]
    public class BarCommand {
        [Option("ordered", 'o', Required = true, ParameterSetName = "ordered")]
        public bool Ordered { get; set; }

        [Option("catchall", 'c', Required = true, ParameterSetName = "catchall")]
        public bool CatchAll { get; set; }

        [Option("generic", 'g', 1, Required = true)]
        public int Generic { get; set; }

        [Option("first", 'f', 2, ParameterSetName = "ordered")]
        public string First { get; set; }

        [Option("second", 's', 3, ParameterSetName = "ordered")]
        public string Second { get; set; }

        [Option("all", 'a', 2, ParameterSetName = "catchall")]
        public IList<string> All { get; set; }
    }
}
