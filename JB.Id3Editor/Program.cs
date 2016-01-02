using System;
using System.Collections.Generic;
using CommandLine;
using JB.Id3Editor.Options;

namespace JB.Id3Editor
{
    class Program
    {
        public static int Main(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<ClearExistingAlbumCoversOptions, WriteAlbumCoversOptions>(args)
                .MapResult(
                    (ClearExistingAlbumCoversOptions opts) => ClearExistingAlbumCoversAndReturnExitCode(opts),
                    (WriteAlbumCoversOptions opts) => WriteAlbumCoversAndReturnExitCode(opts),
                    errs => 1);
        }

        private static int ClearExistingAlbumCoversAndReturnExitCode(ClearExistingAlbumCoversOptions opts)
        {
            IEnumerable<string> filesToProcess = null;
            if (IsDirectory(opts.TargetPath))
            {
                filesToProcess = System.IO.Directory.EnumerateFiles(
                    opts.TargetPath,
                    "*.mp3",
                    opts.Recursive
                    ? System.IO.SearchOption.AllDirectories
                    : System.IO.SearchOption.TopDirectoryOnly);
            }
            else
            {
                if (System.IO.File.Exists(opts.TargetPath))
                {
                    filesToProcess = new List<string>()
                    {
                        opts.TargetPath
                    };
                }
                else
                {
                    Console.WriteLine("File or Directory '{0}' not found!", opts.TargetPath ?? "n.a.");
                    return 1;
                }
            }

            var errorOccured = false;
            foreach (var fileToProcess in filesToProcess)
            {
                try
                {
                    ClearAlbumCovers(fileToProcess);
                    Console.WriteLine("Cleaning '{0}' - Success", fileToProcess);
                }
                catch (Exception exception)
                {
                    errorOccured = true;

                    var currentForeColor = Console.ForegroundColor;
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine("Cleaning '{0}' - Error: {1}", fileToProcess, exception.Message);
                    }
                    finally
                    {
                        Console.ForegroundColor = currentForeColor;
                    }
                }
            }

            return errorOccured ? 1 : 0;
        }

        private static int WriteAlbumCoversAndReturnExitCode(WriteAlbumCoversOptions opts)
        {
            throw new NotImplementedException();
        }

        private static bool IsDirectory(string path)
        {
            // see http://stackoverflow.com/questions/439447/net-how-to-check-if-path-is-a-file-and-not-a-directory
            return (System.IO.File.GetAttributes(path) & System.IO.FileAttributes.Directory)
                == System.IO.FileAttributes.Directory;
        }

        private static void ClearAlbumCovers(string pathToFile)
        {
            using (var tagLibFile = TagLib.File.Create(pathToFile))
            {
                if (tagLibFile.Tag != null && tagLibFile.Tag.Pictures != null && tagLibFile.Tag.Pictures.Length > 0)
                {
                    tagLibFile.Tag.Pictures = new TagLib.IPicture[0];
                    tagLibFile.Save();
                }
            }
        }
    }
}
