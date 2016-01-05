using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        /// <summary>
        /// Gets the files to process.
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        /// <param name="searchFilter">The search filter.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException"></exception>
        public static IEnumerable<string> GetFilesToProcess(string targetPath, string searchFilter, bool recursive)
        {
            if (IsDirectory(targetPath))
            {
                return Directory.EnumerateFiles(
                    targetPath,
                    searchFilter,
                    recursive
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly);
            }
            else
            {
                if (File.Exists(targetPath))
                {
                    return new List<string>()
                    {
                        targetPath
                    };
                }
                else
                {
                    throw new IOException(string.Format("File or Directory '{0}' not found!", targetPath ?? "n.a."));
                }
            }
        }

        /// <summary>
        /// Gets the assembly attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T GetAssemblyAttribute<T>(this System.Reflection.Assembly assembly) where T : Attribute
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            object[] attributes = assembly.GetCustomAttributes(typeof(T), false);

            return (attributes == null || attributes.Length == 0)
                ? null
                : attributes.OfType<T>().SingleOrDefault();
        }
    }
}