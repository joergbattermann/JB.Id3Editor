using System;
using System.Collections.Generic;
using System.Threading;
using JB.Id3Editor.Options;

namespace JB.Id3Editor.Commands
{
    public class WriteAlbumCoversCommand : ICommand<WriteAlbumCoversOptions>
    {
        /// <summary>
        /// The default mappings filename
        /// </summary>
        private const string DefaultMappingsFilename = "DefaultMappings.ini";
        
        #region Implementation of ICommand<in WriteAlbumCoversOptions>

        /// <summary>
        /// Runs the command and returns an exit code.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public int RunAndReturnExitCode(WriteAlbumCoversOptions options, CancellationToken cancellationToken = new CancellationToken())
        {
            string defaultCoverFile = string.Empty;
            var genresToCoverFilesMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        
            throw new System.NotImplementedException();
        }

        #endregion
    }
}