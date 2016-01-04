using System.Threading;
using JB.Id3Editor.Options;

namespace JB.Id3Editor.Commands
{
    public interface ICommand<in TOptions> where TOptions : IOptions
    {
        /// <summary>
        /// Runs the command and returns an exit code.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        int RunAndReturnExitCode(TOptions options, CancellationToken cancellationToken = default(CancellationToken));
    }
}