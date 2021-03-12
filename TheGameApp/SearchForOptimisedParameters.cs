using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// The search for optimised parameters.
    /// </summary>
    public static class SearchForOptimisedParameters
    {
        /// <summary>
        /// Search for optimised parameters.
        /// </summary>
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            var configuration =
                new Configuration(2000, 5,
                                  new List<AvailablePlayer>
                                  {
                                      AvailablePlayer.PlaysBackwardsAndSmallestDistanceWithCommunication
                                  });
            try
            {
                for (var i = 0; i < 10; i++)
                {
                    //PlayerMoveRules.MaxDistanceForSingleMoveWithCommunication2 = i;
                    var averagedResult = RunSimulation(configuration);
                    Console.WriteLine($"{i} - avg: {averagedResult.AverageNumberOfCardsLeft.ToString("N1", NumberFormatInfo.InvariantInfo)} (excellent: {averagedResult.PercentageExcellentResult.ToString("N0", NumberFormatInfo.InvariantInfo)}, perfect: {averagedResult.PercentagePerfectResult.ToString("N0", NumberFormatInfo.InvariantInfo)}, min: {averagedResult.MinNumberOfCardsLeft} - max: {averagedResult.MaxNumberOfCardsLeft})");
                }
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    Console.WriteLine(exception.ToString());
                }
            }

            Console.WriteLine(new string('=', 20));
            Console.WriteLine("Search finished.");
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("An unhandled exception occurred.");
            Console.WriteLine(e.ExceptionObject.ToString());
            Environment.Exit(-1);
        }

        /// <summary>
        /// Runs the simulation with the given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="AggregateException">One or more exceptions occurred during the running of the simulation.</exception>
        private static SimulationResult RunSimulation(Configuration configuration)
        {
            var results = new List<SimulationResult>();
            var runner  = new SimulationRunner(configuration.Parallelism);

            foreach (var availablePlayerType in configuration.AvailablePlayers)
            {
                var players = new List<AvailablePlayer> {availablePlayerType};
                for (var i = 1; i < 5; i++)
                {
                    players.Add(availablePlayerType);
                    try
                    {
                        var result = runner.Run(players, configuration.Runs);
                        results.Add(result);
                        //Console.WriteLine($"{availablePlayerType}, {i} players: avg: {result.AverageNumberOfCardsLeft.ToString("N1", NumberFormatInfo.InvariantInfo)} (super: {result.PercentageExcellentResult.ToString("N0", NumberFormatInfo.InvariantInfo)}, perfect: {result.PercentagePerfectResult.ToString("N0", NumberFormatInfo.InvariantInfo)}, min: {result.MinNumberOfCardsLeft} - max: {result.MaxNumberOfCardsLeft})");
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Run was cancelled!");
                    }
                }
            }

            return new SimulationResult((int) results.Average(x => x.MinNumberOfCardsLeft),
                                        (int) results.Average(x => x.MaxNumberOfCardsLeft),
                                        results.Average(x => x.AverageNumberOfCardsLeft),
                                        results.Average(x => x.PercentageExcellentResult),
                                        results.Average(x => x.PercentagePerfectResult));
        }
    }
}