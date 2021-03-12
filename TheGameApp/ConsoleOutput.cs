using System;
using System.Collections.Generic;

namespace TheGameApp
{
    /// <summary>
    /// Handles the output to the console.
    /// </summary>
    internal class ConsoleOutput
    {
        private readonly bool outputIsEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleOutput" /> class.
        /// </summary>
        /// <param name="outputIsEnabled">If set to true, then output is enabled.</param>
        internal ConsoleOutput(bool outputIsEnabled)
        {
            this.outputIsEnabled = outputIsEnabled;
        }

        internal void PlayerCommunication(int relativePosition, PlayerCommunication playerCommunication)
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine($"Communication from relative Position {relativePosition} for {playerCommunication.Row}: {playerCommunication.CommunicationType}.");
        }

        internal void PlayerStartDecision(Player player, PlayerStartDecision decision)
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine($"{player.ToString(false)} {decision}.");
        }

        internal void PlayersTurn(Player player, Dictionary<RowOfCardsIdentifier, RowOfCards> rowsOfCards)
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine($"Next turn: {player}.");
            foreach (var (key, value) in rowsOfCards)
            {
                Console.WriteLine($"{key}: {value}");
            }
        }

        internal void PlayerDrawsCard(Player player, int card)
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine($"{player} and has drawn the card {card}.");
        }

        internal void GameStarts(IEnumerable<Player> players)
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine("The Game starts.");
            foreach (var player in players)
            {
                Console.WriteLine(player);
            }
        }

        internal void IllegalMove()
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine("The move is illegal.");
        }

        internal void CardDeckIsEmpty()
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine("The card deck is empty.");
        }

        internal void PlayerMove(PlayerMove move)
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine(move.Decision is PlayerMoveDecision.WantToPlay
                                  ? $"{move.Decision}: Card {move.CardPlacement!.Card} goes on row {move.CardPlacement.RowOfCards}."
                                  : $"{move.Decision}.");
        }

        internal void GameEnds(int numberOfRemainingCards)
        {
            if (!outputIsEnabled)
            {
                return;
            }

            Console.WriteLine($"The game ended with {numberOfRemainingCards} remaining cards.");
        }
    }
}