using CommandLine;

namespace JB.Id3Editor.Options
{
    public abstract class BaseOptions : IOptions
    {
        [Option(Required = true, HelpText = "Path to target directory containing *.mp3 files or path to a single .mp3 file.")]
        public string TargetPath { get; set; }

        [Option(Required = false, Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option(Required = false, Default = false, HelpText = "If --targetpath is a Directory, enabling --recursive will traverse into sub-directories, too.")]
        public bool Recursive { get; set; }

        [Option(Required = false, Default = "*.mp3", HelpText = "If --targetpath is a Directory, this will be used to find the corresponding file(s) to process.")]
        public string SearchFilter { get; set; }

        [Option(Required = false, Default = 1, HelpText = "Specifies the maximum level of concurrency. If you have fast I/O multiple CPU cores, increasing this value will improve runtime.")]
        public int MaxDegreeOfParallelism { get; set; }
    }
}