using CommandLine;

namespace JB.Id3Editor.Options
{
    public abstract class BaseOptions
    {
        [Option(Required = true, HelpText = "Path to target directory containing *.mp3 files or path to a single .mp3 file.")]
        public string TargetPath { get; set; }

        [Option(Required = false, Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option(Required = false, Default = false, HelpText = "If --targetpath is a Directory, enabling --recursive will traverse into sub-directories, too.")]
        public bool Recursive { get; set; }
    }
}