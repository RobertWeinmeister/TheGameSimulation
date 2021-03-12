using System;

namespace TheGameApp
{
    /// <summary>
    /// Game rules/settings for "The Game".
    /// </summary>
    internal static class GameRules
    {
        /// <summary>
        /// The <b>lowest</b> number in the game and also the start card for the rows that go <b>up</b>.
        /// </summary>
        internal const int LowestNumberInGame = 1;

        /// <summary>
        /// The <b>highest</b> number in the game and also the start card for the rows that go <b>down</b>.
        /// </summary>
        internal const int HighestNumberInGame = 100;

        /// <summary>
        /// The difference allowed by the backwards trick.
        /// </summary>
        internal const int AllowedBackwardsTrick = 10;

        /// <summary>
        /// The <b>minimum</b> number of players allowed.
        /// </summary>
        internal const int MinNumberOfPlayersAllowed = 1;

        /// <summary>
        /// The <b>maximum</b> number of players allowed.
        /// </summary>
        internal const int MaxNumberOfPlayersAllowed = 5;

        /// <summary>
        /// The number of cards that have to be played in each players turn <b>before</b> the card deck is empty.
        /// </summary>
        internal const int NumberOfCardsToPlayBeforeDeckIsEmpty = 2;

        /// <summary>
        /// The number of cards that have to be played in each players turn <b>after</b> the card deck is empty.
        /// </summary>
        internal const int NumberOfCardsToPlayAfterDeckIsEmpty = 1;

        /// <summary>
        /// The number of cards in the card deck.
        /// </summary>
        internal const int NumberOfCardsInDeck = HighestNumberInGame - LowestNumberInGame - 1;

        /// <summary>
        /// Gets the number of cards to be drawn by each player at the start of the game.
        /// </summary>
        /// <param name="numberOfPlayers">The number of players.</param>
        /// <exception cref="NotSupportedException">Thrown, if the number of players is not supported.</exception>
        /// <returns>The number of cards to be drawn by each player</returns>
        internal static int GetNumberOfCardsToDraw(int numberOfPlayers)
        {
            return numberOfPlayers switch
            {
                1           => 8,
                2           => 7,
                > 2 and < 6 => 6,
                _           => throw new NotSupportedException($"{numberOfPlayers} players are not supported.")
            };
        }
    }
}