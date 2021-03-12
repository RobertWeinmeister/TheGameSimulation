using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheGameApp
{
    /// <summary>
    /// The app.
    /// </summary>
    public static class App
    {
        /// <summary>
        /// Run the application.
        /// </summary>
        /// <param name="args">Arguments to be used for the configuration of the simulation.</param>
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            var configuration = GetConfiguration(args);

            try
            {
                RunSimulation(configuration);
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
        }

        /// <summary>
        /// Runs the simulation with the given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="AggregateException">One or more exceptions occurred during the running of the simulation.</exception>
        private static void RunSimulation(Configuration configuration)
        {
            Console.WriteLine();
            Console.WriteLine("Running the simulation...");
            Console.WriteLine();
            var runner = new SimulationRunner(configuration.Parallelism);

            foreach (var availablePlayerType in configuration.AvailablePlayers)
            {
                var players = new List<AvailablePlayer>();
                for (var i = 1; i < 6; i++)
                {
                    players.Add(availablePlayerType);
                    try
                    {
                        var result = runner.Run(players, configuration.Runs);
                        Console.WriteLine($"{availablePlayerType}, {i} players: avg: {result.AverageNumberOfCardsLeft.ToString("N1", NumberFormatInfo.InvariantInfo)} (super: {result.PercentageExcellentResult.ToString("P0", NumberFormatInfo.InvariantInfo)}, perfect: {result.PercentagePerfectResult.ToString("P0", NumberFormatInfo.InvariantInfo)}, min: {result.MinNumberOfCardsLeft} - max: {result.MaxNumberOfCardsLeft})");
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Run was cancelled!");
                    }
                }
            }
        }

        private static Configuration GetConfiguration(IEnumerable<string> args)
        {
            var reader = new ApplicationArgumentReader();
            reader.DisplayAvailableArguments();
            Console.WriteLine();
            Console.WriteLine("Getting current configuration...");
            var (isValid, configuration) = reader.GetConfigurationFromArguments(args);
            if (!isValid || configuration is null)
            {
                Console.WriteLine("Arguments were not valid.");
                Environment.Exit(-2);
            }

            Console.WriteLine();
            Console.WriteLine("Current simulation is running with:");
            Console.WriteLine(configuration);
            return configuration;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("An unhandled exception occurred.");
            Console.WriteLine(e.ExceptionObject.ToString());
            Environment.Exit(-1);
        }
    }
}