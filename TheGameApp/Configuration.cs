using System;
using System.Collections.Generic;

namespace TheGameApp
{
    /// <summary>
    /// Configuration for the application.
    /// </summary>
    /// <remarks>No validation of the arguments.</remarks>
    internal sealed record Configuration(int Runs, int Parallelism, IList<AvailablePlayer> AvailablePlayers)
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return "Runs".PadRight(25) + Runs + Environment.NewLine + "Parallelism".PadRight(25) + Parallelism +
                   Environment.NewLine + "Players to be simulated:" + Environment.NewLine +
                   string.Join(Environment.NewLine, AvailablePlayers);
        }
    }
}