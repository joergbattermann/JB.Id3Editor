using CommandLine;

namespace JB.Id3Editor.Options
{
    [Verb("writecovers", HelpText = "Writes album cover(s) to target file(s) as specified in .ini(s).")]
    public class WriteAlbumCoversOptions : BaseOptions
    {
        [Option(Required = false, Default = "", HelpText = "Full path to custom Mappings.ini file.")]
        public string CustomMappingsFile { get; set; }

        [Option(Required = false, Default = false, HelpText = "If enabled, existing cover art in .mp3(s) will be overwritten instead of skipping these file(s).")]
        public bool Force { get; set; }
    }
}