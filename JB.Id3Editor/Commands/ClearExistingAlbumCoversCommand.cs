using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JB.Id3Editor.Options;
using TagLib;

namespace JB.Id3Editor.Commands
{
    public class ClearExistingAlbumCoversCommand : ICommand<ClearExistingAlbumCoversOptions>
    {
        #region Implementation of ICommand<in ClearExistingAlbumCoversOptions>

        /// <summary>
        /// Runs the command and returns an exit code.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public int RunAndReturnExitCode(ClearExistingAlbumCoversOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.TargetPath)) throw new ArgumentOutOfRangeException(nameof(options), "TargetPath may not be empty.");
            if (string.IsNullOrWhiteSpace(options.SearchFilter)) throw new ArgumentOutOfRangeException(nameof(options), "SearchFilter may not be empty.");

            IEnumerable<string> filesToProcess = Helpers.GetFilesToProcess(options.TargetPath,
                    options.SearchFilter,
                    options.Recursive);
            
            var errorOccured = false;
            
            try
            {
                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = options.MaxDegreeOfParallelism <= 0
                    ? 1
                    : options.MaxDegreeOfParallelism,
                    CancellationToken = cancellationToken
                };

                Parallel.ForEach(filesToProcess, parallelOptions, (fileToProcess, ls) =>
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

                            Console.WriteLine("Error Cleaning '{0}': {1}", fileToProcess, exception.Message);
                        }
                        finally
                        {
                            Console.ForegroundColor = currentForeColor;
                        }
                    }
                });
            }
            catch (OperationCanceledException)
            {
                var currentForeColor = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine("Cancellation requested - stopping process.");
                }
                finally
                {
                    Console.ForegroundColor = currentForeColor;
                }
            }
            
            return errorOccured ? 1 : 0;
        }

        #endregion 

        /// <summary>
        /// Clears the album cover(s) for the given file.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <exception cref="IOException">File is not writeable</exception>
        private void ClearAlbumCovers(string pathToFile)
        {
            if (string.IsNullOrWhiteSpace(pathToFile))
                throw new ArgumentException("Argument is null or whitespace", nameof(pathToFile));

            using (var tagLibFile = TagLib.File.Create(pathToFile))
            {
                if (!tagLibFile.Writeable)
                    throw new IOException("File is not writeable");

                if (tagLibFile.Tag != null && tagLibFile.Tag.Pictures != null && tagLibFile.Tag.Pictures.Length > 0)
                {
                    tagLibFile.Tag.Pictures = tagLibFile.Tag.Pictures.Where(picture => picture.Type != PictureType.BackCover && picture.Type != PictureType.FrontCover).ToArray();
                    tagLibFile.Save();
                }
            }
        }
    }
}