using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// A player for "The Game".
    /// </summary>
    internal sealed class Player
    {
        private readonly HashSet<int>                                                        cardsInHand = new();
        private readonly string                                                              name;
        private readonly List<Func<IReadOnlyCollection<int>, PlayerInformation, PlayerMove>> moveRules = new();

        private readonly List<Func<IReadOnlyCollection<int>, PlayerInformation, List<PlayerCommunication>>>
            communicationRules = new();

        private Func<IReadOnlyCollection<int>, int, PlayerStartDecision> startRule;

        private Player(string name)
        {
            this.name = name;
            startRule = AlwaysWantsToStart;
        }

        /// <inheritdoc />
        [Pure]
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// Initializes and returns a new instance of the <see cref="Player" /> class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <returns>The new player.</returns>
        /// <remarks>
        ///     <para>A new player has a standard start rule and communication rule set, but not move rules.</para>
        /// </remarks>
        internal static Player NewPlayer(string name)
        {
            return new(name);
        }

        /// <summary>
        /// Adds an additional move rule at the end of the rules chain.
        /// </summary>
        /// <param name="additionalMoveRule">The additional move rule.</param>
        /// <returns>The player.</returns>
        /// <remarks>
        ///     <para>Move rules are processed in order until a decision is reached.</para>
        /// </remarks>
        internal Player AddMoveRule(Func<IReadOnlyCollection<int>, PlayerInformation, PlayerMove> additionalMoveRule)
        {
            moveRules.Add(additionalMoveRule);
            return this;
        }

        /// <summary>
        /// Adds an additional communication rule.
        /// </summary>
        /// <param name="additionalCommunicationRule">The communication rule.</param>
        /// <returns>The player.</returns>
        internal Player AddCommunicationRule(
            Func<IReadOnlyCollection<int>, PlayerInformation, List<PlayerCommunication>> additionalCommunicationRule)
        {
            communicationRules.Add(additionalCommunicationRule);
            return this;
        }

        /// <summary>
        /// Sets the new start rule. The old rule is overwritten.
        /// </summary>
        /// <param name="newStartRule">The start rule.</param>
        /// <returns>The player.</returns>
        internal Player SetStartRule(Func<IReadOnlyCollection<int>, int, PlayerStartDecision> newStartRule)
        {
            startRule = newStartRule;
            return this;
        }

        /// <summary>
        /// Converts the player to a string representation.
        /// </summary>
        /// <param name="outputWithCards">If set to true, add the cards in hand to the string representation.</param>
        /// <returns>Player representation as string.</returns>
        [Pure]
        internal string ToString(bool outputWithCards)
        {
            return (cardsInHand.Count, outputWithCards) switch
            {
                (_, false) => name,
                (0, true)  => $"{name} has no cards",
                (_, _)     => $"{name} has the cards {string.Join(", ", cardsInHand.OrderByDescending(x => x))}"
            };
        }

        /// <summary>
        /// Receive the card and add it to the cards in hand.
        /// </summary>
        /// <param name="card">The card to receive.</param>
        internal void ReceiveCard(int card)
        {
            cardsInHand.Add(card);
        }

        /// <summary>
        /// Hand over the card.
        /// </summary>
        /// <param name="card">The card to hand over.</param>
        /// <exception cref="ArgumentException">The card was not in the hand.</exception>
        internal void HandOverCard(int card)
        {
            if (cardsInHand.Contains(card))
            {
                cardsInHand.Remove(card);
            }
            else
            {
                throw new ArgumentException($"Card {card} was not in the hand.", nameof(card));
            }
        }

        /// <summary>
        /// Get the player's next move.
        /// </summary>
        /// <param name="info">The available information for the player.</param>
        /// <returns>The next move of the player.</returns>
        [Pure]
        internal PlayerMove GetNextMove(PlayerInformation info)
        {
            foreach (var moveRule in moveRules)
            {
                var move = moveRule.Invoke(cardsInHand, info);
                if (move.Decision is not PlayerMoveDecision.Undecided)
                {
                    return move;
                }
            }

            return new PlayerMove(PlayerMoveDecision.CannotPlay);
        }

        /// <summary>
        /// Get the player's communication.
        /// </summary>
        /// <param name="info">The available information for the player.</param>
        /// <returns>The communication from the player.</returns>
        [Pure]
        internal IEnumerable<PlayerCommunication> GetCommunication(PlayerInformation info)
        {
            var communication = new List<PlayerCommunication>();

            foreach (var communicationRule in communicationRules)
            {
                communication.AddRange(communicationRule.Invoke(cardsInHand, info));
            }

            return communication;
        }

        /// <summary>
        /// Gets the player's start decision.
        /// </summary>
        /// <param name="round">The round.</param>
        /// <returns>The start decision from the player.</returns>
        [Pure]
        internal PlayerStartDecision GetStartDecision(int round)
        {
            return startRule.Invoke(cardsInHand, round);
        }

        /// <summary>
        /// Standard implementation for the decision of the player to start the game; the player always wants to play.
        /// </summary>
        /// <param name="cards">Not used.</param>
        /// <param name="startingRound">Not used.</param>
        /// <returns>Player wants to start.</returns>
        [Pure]
        private static PlayerStartDecision AlwaysWantsToStart(IReadOnlyCollection<int> cards, int startingRound)
        {
            return PlayerStartDecision.WantToStart;
        }
    }
}