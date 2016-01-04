using System;
using System.Threading;
using CommandLine;
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

        public static int Main(string[] args)
        {
            try
            {
                Console.CancelKeyPress += OnConsoleCancelKeyPress;

                return Parser.Default.ParseArguments<ClearExistingAlbumCoversOptions, WriteAlbumCoversOptions>(args)
                .MapResult(
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
                    errors => 1);
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
