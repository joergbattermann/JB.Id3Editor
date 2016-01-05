using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using CommandLine;
using CommandLine.Text;
using JB.Id3Editor.Commands;
using JB.Id3Editor.Options;

namespace JB.Id3Editor
{
    class Program
    {
        /// <summary>
        /// The cancellation token source used for ctrl+c handling
        /// </summary>
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        internal const int DefaultErrorCode = 1;

        public static int Main(string[] args)
        {
            try
            {
                Console.CancelKeyPress += OnConsoleCancelKeyPress;

                var parser = new Parser(settings =>
                {
                    settings.IgnoreUnknownArguments = true;
                });
                
                var parserResult = parser.ParseArguments<ClearExistingAlbumCoversOptions, WriteAlbumCoversOptions>(args);
                if (parserResult.Tag == ParserResultType.Parsed)
                {
                    return parserResult.MapResult(
                        (ClearExistingAlbumCoversOptions options) =>
                        {
                            var command = new ClearExistingAlbumCoversCommand();

                            return command.RunAndReturnExitCode(options, CancellationTokenSource.Token);
                        },
                    (WriteAlbumCoversOptions options) =>
                    {
                        var command = new WriteAlbumCoversCommand();

                        return command.RunAndReturnExitCode(options, CancellationTokenSource.Token);
                    },
                    errors => DefaultErrorCode);
                }
                else
                {
                    var helpText = HelpText.AutoBuild(parserResult);
                    helpText.Heading = GetHelpTextHeading();

                    Console.Error.Write(helpText);

                    return DefaultErrorCode;
                }
            }
            catch (AggregateException aggregateException)
            {
                var currentConsoleForegroundColor = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Something bad happened:");

                    foreach (var exception in aggregateException.Flatten().InnerExceptions)
                    {
                        Console.WriteLine("- {0}", exception.Message);
                    }

                    return 1;
                }
                finally
                {
                    Console.ForegroundColor = currentConsoleForegroundColor;
                }
            }
            catch (Exception exception)
            {
                var currentConsoleForegroundColor = Console.ForegroundColor;
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Something bad happened: {0}", exception);

                    return 1;
                }
                finally
                {
                    Console.ForegroundColor = currentConsoleForegroundColor;
                }
            }
        }

        /// <summary>
        /// Gets the help text heading.
        /// </summary>
        /// <returns></returns>
        private static string GetHelpTextHeading()
        {
            var title = typeof (Program).Assembly.GetAssemblyAttribute<AssemblyTitleAttribute>();
            var version = typeof(Program).Assembly.GetName().Version;

            return $"{title.Title} v{version}";
        }

        /// <summary>
        /// Called when [CTRL+C] is pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="consoleCancelEventArgs">The <see cref="ConsoleCancelEventArgs"/> instance containing the event data.</param>
        private static void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            CancellationTokenSource.Cancel(); // signal cancellation to command(s)

            consoleCancelEventArgs.Cancel = true; // cancel process termination
        }
    }
}
