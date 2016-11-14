using CommandLineParser.Arguments;

namespace Resxar
{
    public class CommandLineOptions
    {
        [ValueArgument(typeof(string), 'i', "in", Optional = false)]
        public string InputDirectory { get; set; }

        [ValueArgument(typeof(string), 'o', "out", Optional = false)]
        public string OutputDirectory { get; set; }
    }
}
