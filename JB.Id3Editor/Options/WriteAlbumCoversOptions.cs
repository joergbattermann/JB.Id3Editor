using CommandLine;

namespace JB.Id3Editor.Options
{
    [Verb("write", HelpText = "Writes album covers to .mp3 file(s).")]
    public class WriteAlbumCoversOptions : BaseOptions
    {
        [Option(Required = false, Default = "", HelpText = "Full path to custom genre Mappings.ini file.")]
        public string CustomMappings { get; set; }

        [Option(Required = false, Default = false, HelpText = "If enabled, existing cover art in .mp3(s) will be overwritten instead of skipping these file(s).")]
        public bool Force { get; set; }
    }
}