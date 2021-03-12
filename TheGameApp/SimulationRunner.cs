using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// Runs the simulation of the "The Game".
    /// </summary>
    internal class SimulationRunner
    {
        private readonly int degreeOfParallelism;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationRunner" /> class.
        /// </summary>
        /// <param name="degreeOfParallelism">Degree of parallelism.</param>
        internal SimulationRunner(int degreeOfParallelism)
        {
            this.degreeOfParallelism = degreeOfParallelism;
        }

        /// <summary>
        /// Runs the simulation.
        /// </summary>
        /// <param name="availablePlayers">The available players.</param>
        /// <param name="numberOfRuns">The number of runs.</param>
        /// <returns>The result of the simulation.</returns>
        /// <exception cref="AggregateException">One or more exceptions occurred during the running of the simulation.</exception>
        /// <exception cref="OperationCanceledException">The simulation query was canceled.</exception>
        internal SimulationResult Run(IReadOnlyList<AvailablePlayer> availablePlayers, int numberOfRuns)
        {
            var random      = new Random();
            var randomSeeds = new List<int>();
            for (var i = 0; i < numberOfRuns; i++)
            {
                randomSeeds.Add(random.Next());
            }

            var numberOfCardsLeft = Enumerable.Range(0, numberOfRuns).AsParallel()
                                              .WithDegreeOfParallelism(degreeOfParallelism).Select(x =>
                                               {
                                                   var game =
                                                       new
                                                           Game(PlayerFactory.GetPlayers(availablePlayers).AsReadOnly(),
                                                                new Random(randomSeeds[x]),
                                                                numberOfRuns == 1);
                                                   return game.Run();
                                               }).ToList();

            return new SimulationResult(numberOfCardsLeft.Min(), numberOfCardsLeft.Max(), numberOfCardsLeft.Average(),
                                        numberOfCardsLeft.Count(x => x < 10) / (double) numberOfCardsLeft.Count,
                                        numberOfCardsLeft.Count(x => x == 0) / (double) numberOfCardsLeft.Count);
        }
    }
}