using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable StringLiteralTypo

namespace TheGameApp
{
    /// <summary>
    /// Reads application arguments.
    /// </summary>
    internal class ApplicationArgumentReader
    {
        /// <summary>
        /// All available settings for the configuration. Initially, the current value is set to the default value.
        /// </summary>
        private readonly Dictionary<string, (string displayString, int defaultValue, int minValue, int maxValue)>
            valueSettings = new() {{"RUNS", ("Runs", 100, 1, 10000)}, {"PARALLELISM", ("Parallelism", 5, 1, 10)}};

        private readonly HashSet<AvailablePlayer> availablePlayers = new();
        private          Dictionary<string, int>  value            = new();

        /// <summary>
        /// Display the available arguments in the console.
        /// </summary>
        internal void DisplayAvailableArguments()
        {
            var padding = valueSettings.Keys.Max(x => x.Length) + 5;
            Console.WriteLine("Available arguments are:");
            foreach (var (_, (displayParameter, defaultValue, minValue, maxValue)) in valueSettings)
            {
                Console.WriteLine($"{displayParameter.PadRight(padding)}: default value is {defaultValue}, range is [{minValue}, {maxValue}]");
            }

            Console.WriteLine();
            Console.WriteLine("If the number of runs is set to 1, the details of each game will be output to the console.");
            Console.WriteLine();
            Console.WriteLine("Player types are referenced by number.");
            Console.WriteLine();
            Console.WriteLine("Available player types are:");

            IList players = Enum.GetValues(typeof(AvailablePlayer));
            for (var i = 0; i < players.Count; i++)
            {
                Console.WriteLine($"{i} - {players[i]?.ToString()?.PadRight(padding)}");
            }

            Console.WriteLine();
            Console.WriteLine("Example usage:");
            Console.WriteLine("Runs=100 Parallelism=5 1 2");
        }

        /// <summary>
        /// Get the configuration from the given application arguments.
        /// </summary>
        /// <param name="arguments">The application arguments.</param>
        /// <returns>If a valid configuration is found, returns true and the configuration; otherwise false.</returns>
        internal (bool isValid, Configuration? configuration) GetConfigurationFromArguments(
            IEnumerable<string> arguments)
        {
            value = valueSettings.ToDictionary(x => x.Key, y => y.Value.defaultValue);

            foreach (var argument in arguments)
            {
                var argumentParts = argument.Split('=');

                switch (argumentParts.Length)
                {
                    case 2 when valueSettings.ContainsKey(argumentParts[0].ToUpperInvariant()):

                        if (!int.TryParse(argumentParts[1], out var argumentValue))
                        {
                            Console
                               .WriteLine($"The value {argumentParts[1]} is not a valid number for the argument {argumentParts[0]}.");
                            return (false, null);
                        }

                        if (valueSettings[argumentParts[0].ToUpperInvariant()].minValue > argumentValue ||
                            valueSettings[argumentParts[0].ToUpperInvariant()].maxValue < argumentValue)
                        {
                            Console
                               .WriteLine($"The value {argumentValue} is not within the valid range from {valueSettings[argumentParts[0].ToUpperInvariant()].minValue} to {valueSettings[argumentParts[0].ToUpperInvariant()].maxValue} for the argument {argumentParts[0]}.");
                            return (false, null);
                        }

                        value[argumentParts[0].ToUpperInvariant()] = argumentValue;
                        continue;

                    case 1 when int.TryParse(argumentParts[0], out var number) &&
                                Enum.IsDefined(typeof(AvailablePlayer), number):
                        availablePlayers.Add((AvailablePlayer) number);
                        continue;
                    default:
                        Console.WriteLine($"Unknown argument: {argument}");
                        break;
                }
            }

            if (availablePlayers.Count == 0)
            {
                foreach (AvailablePlayer availablePlayerType in Enum.GetValues(typeof(AvailablePlayer)))
                {
                    availablePlayers.Add(availablePlayerType);
                }
            }

            return (true, new Configuration(value["RUNS"], value["PARALLELISM"], availablePlayers.ToList()));
        }
    }
}