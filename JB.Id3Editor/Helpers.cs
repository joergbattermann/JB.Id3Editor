using System;

namespace JB.Id3Editor
{
    public static class Helpers
    {
        /// <summary>
        /// Determines whether the specified path is a directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Argument is null or whitespace</exception>
        public static bool IsDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Argument is null or whitespace", nameof(path));

            // see http://stackoverflow.com/questions/439447/net-how-to-check-if-path-is-a-file-and-not-a-directory
            return (System.IO.File.GetAttributes(path) & System.IO.FileAttributes.Directory)
                == System.IO.FileAttributes.Directory;
        }
    }
}