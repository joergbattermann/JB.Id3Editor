using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IniParser;
using JB.Id3Editor.Options;
using TagLib;
using File = System.IO.File;

namespace JB.Id3Editor.Commands
{
    public class WriteAlbumCoversCommand : ICommand<WriteAlbumCoversOptions>
    {
        /// <summary>
        /// The default mappings filename
        /// </summary>
        private const string DefaultMappingsFilename = "DefaultMappings.ini";

        private const string DefaultCoverKey = "DefaultCover";

        private const string DefaultsSection = "Defaults";
        private const string GenresSection = "Genres";

        #region Implementation of ICommand<in WriteAlbumCoversOptions>

        /// <summary>
        /// Runs the command and returns an exit code.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public int RunAndReturnExitCode(WriteAlbumCoversOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.TargetPath)) throw new ArgumentOutOfRangeException(nameof(options), "TargetPath may not be empty.");
            if (string.IsNullOrWhiteSpace(options.SearchFilter)) throw new ArgumentOutOfRangeException(nameof(options), "SearchFilter may not be empty.");

            //Create an instance of a ini file parser
            FileIniDataParser fileIniData = new FileIniDataParser();
            fileIniData.Parser.Configuration.CommentString = "#";
            fileIniData.Parser.Configuration.AllowDuplicateKeys = false;
            fileIniData.Parser.Configuration.AllowDuplicateSections = false;
            fileIniData.Parser.Configuration.AllowKeysWithoutSection = false;
            fileIniData.Parser.Configuration.CaseInsensitive = true;

            var mappingsIniFile = fileIniData.ReadFile(Helpers.NormalizePotentialRelativeToFullPath(DefaultMappingsFilename));

            // merge with custom / user mappings file, if applicable
            if (!string.IsNullOrWhiteSpace(options.CustomMappingsFile) && File.Exists(Helpers.NormalizePotentialRelativeToFullPath(options.CustomMappingsFile)))
            {
                var customMappingsIniFile = fileIniData.ReadFile(Helpers.NormalizePotentialRelativeToFullPath(options.CustomMappingsFile));

                mappingsIniFile.Merge(customMappingsIniFile);
            }

            if(!mappingsIniFile.Sections.ContainsSection(DefaultsSection))
                throw new ArgumentOutOfRangeException(nameof(options), $"No '[{DefaultsSection}]' Section found in the configuration file(s).");

            var defaultsSection = mappingsIniFile.Sections[DefaultsSection];

            if (!defaultsSection.ContainsKey(DefaultCoverKey))
                throw new ArgumentOutOfRangeException(nameof(options), $"'[{DefaultsSection}]' Section in the configuration file(s) does not contain a '{DefaultCoverKey}' entry.");

            string defaultCoverFile = defaultsSection[DefaultCoverKey];

            if(string.IsNullOrWhiteSpace(defaultCoverFile))
                throw new ArgumentOutOfRangeException(nameof(options), $"'{DefaultCoverKey}' entry in '[{DefaultsSection}]' in the configuration file(s) may not be empty.");

            // normalize cover path
            defaultCoverFile = Helpers.NormalizePotentialRelativeToFullPath(defaultCoverFile);

            if (!File.Exists(defaultCoverFile))
                throw new ArgumentOutOfRangeException(nameof(options), $"File '{defaultCoverFile}' configured in '[{DefaultsSection}]' > '{DefaultCoverKey}' entry in the configuration file(s) does not exist.");

            var genresToCoverFilesMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (mappingsIniFile.Sections.ContainsSection(GenresSection))
            {
                foreach (var keyData in mappingsIniFile.Sections[GenresSection].Where(keyData => !string.IsNullOrWhiteSpace(keyData.KeyName) && !string.IsNullOrWhiteSpace(keyData.Value)))
                {
                    var key = keyData.KeyName.Trim();
                    var value = Helpers.NormalizePotentialRelativeToFullPath(keyData.Value.Trim());

                    if (!File.Exists(value))
                        continue;

                    if (genresToCoverFilesMapping.ContainsKey(key))
                    {
                        genresToCoverFilesMapping.Remove(key);
                    }

                    genresToCoverFilesMapping.Add(key, value);
                }
            }

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
                        if (!File.Exists(fileToProcess))
                        {
                            var currentForeColor = Console.ForegroundColor;
                            try
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;

                                Console.WriteLine($"File '{fileToProcess}' does not / no longer exist - Skipping");
                            }
                            finally
                            {
                                Console.ForegroundColor = currentForeColor;
                            }
                        }
                        else
                        {
                            WriteAlbumCover(fileToProcess, options.Force, defaultCoverFile, genresToCoverFilesMapping);
                        }
                    }
                    catch (Exception exception)
                    {
                        errorOccured = true;

                        var currentForeColor = Console.ForegroundColor;
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Red;

                            Console.WriteLine("Error Writing Cover to '{0}': {1}", fileToProcess, exception.Message);
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
        /// <param name="overwriteExistingCovers">if set to <c>true</c> [overwrite existing covers].</param>
        /// <param name="defaultCover">The default cover.</param>
        /// <param name="genresToCoverFileMappings">The genres to cover file mappings.</param>
        /// <exception cref="ArgumentException">Argument is null or whitespace</exception>
        /// <exception cref="IOException">File is not writeable</exception>
        private void WriteAlbumCover(string pathToFile, bool overwriteExistingCovers, string defaultCover, IDictionary<string, string> genresToCoverFileMappings)
        {
            if (genresToCoverFileMappings == null) throw new ArgumentNullException(nameof(genresToCoverFileMappings));
            if (string.IsNullOrWhiteSpace(pathToFile)) throw new ArgumentException("Argument is null or whitespace", nameof(pathToFile));
            if (string.IsNullOrWhiteSpace(defaultCover)) throw new ArgumentException("Argument is null or whitespace", nameof(defaultCover));

            using (var tagLibFile = TagLib.File.Create(pathToFile))
            {
                if (!tagLibFile.Writeable)
                    throw new IOException("File is not writeable");

                // skip file if force overwrite is set to false
                if (overwriteExistingCovers == false
                    && tagLibFile.Tag != null
                    && tagLibFile.Tag.Pictures != null
                    && tagLibFile.Tag.Pictures.Any(picture => picture.Type == PictureType.FrontCover))
                {
                    Console.WriteLine("NO Cover written to '{0}' - Skipped ({1} existing cover(s) found)", pathToFile, tagLibFile.Tag.Pictures.Length);
                    return;
                }
                
                // else check for genre match or fall back to defaultcover
                string coverToUse = defaultCover;
                if (tagLibFile.Tag != null && !string.IsNullOrWhiteSpace(tagLibFile.Tag.FirstGenre) &&
                    genresToCoverFileMappings.ContainsKey(tagLibFile.Tag.FirstGenre.Trim()))
                    coverToUse = genresToCoverFileMappings[tagLibFile.Tag.FirstGenre.Trim()];

                if (tagLibFile.Tag != null)
                {
                    tagLibFile.Tag.Pictures = tagLibFile.Tag.Pictures.Where(picture => picture.Type != PictureType.FrontCover)
                        .Concat(new[]
                        {
                            new Picture(coverToUse) {Type = PictureType.FrontCover}
                        }).ToArray();
                    tagLibFile.Save();

                    Console.WriteLine("'{0}' Cover written to '{1}' - Success", string.Equals(coverToUse, defaultCover) ? "Default" : tagLibFile.Tag.FirstGenre.Trim(), pathToFile);
                }
            }
        }
    }
}