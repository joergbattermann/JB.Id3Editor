using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JB.Id3Editor
{
    public static class Helpers
    {
        /// <summary>
        /// Normalizes the target path.
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string NormalizeTargetPath(string targetPath)
        {
            if (targetPath == null) throw new ArgumentNullException(nameof(targetPath));
            // see https://github.com/gsscoder/commandline/issues?utf8=%E2%9C%93&q=path

            if(targetPath.EndsWith(":"))
                return targetPath + "\\";

            if(targetPath.EndsWith("\""))
                return targetPath.Replace("\"", "\\");
            
            // else
           return targetPath;
        }

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

            // handle root drive(s) first
            return Directory.Exists(path);
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
            var normalizedTargetPath = Helpers.NormalizeTargetPath(targetPath);
            if (IsDirectory(normalizedTargetPath))
            {
                return Directory.EnumerateFiles(
                    normalizedTargetPath,
                    searchFilter,
                    recursive
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly);
            }
            else
            {
                if (File.Exists(normalizedTargetPath))
                {
                    return new List<string>()
                    {
                        targetPath
                    };
                }
                else
                {
                    throw new IOException(string.Format("File or Directory '{0}' not found!", normalizedTargetPath ?? "n.a."));
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