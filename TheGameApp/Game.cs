using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// The actual game.
    /// </summary>
    internal class Game
    {
        private readonly CardDeck      cardDeck;
        private readonly ConsoleOutput output;

        private readonly Dictionary<RowOfCardsIdentifier, RowOfCards> rowsOfCards = new()
            {
                {RowOfCardsIdentifier.FirstRowUp, new RowOfCards(RowOfCardsType.Up)},
                {RowOfCardsIdentifier.SecondRowUp, new RowOfCards(RowOfCardsType.Up)},
                {RowOfCardsIdentifier.FirstRowDown, new RowOfCards(RowOfCardsType.Down)},
                {RowOfCardsIdentifier.SecondRowDown, new RowOfCards(RowOfCardsType.Down)}
            };

        private readonly IReadOnlyList<Player> players;

        private int numberOfCardsToPlayPerTurn = GameRules.NumberOfCardsToPlayBeforeDeckIsEmpty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game" /> class.
        /// </summary>
        /// <param name="players">The players for this game.</param>
        /// <param name="random">The random generator.</param>
        /// <param name="withOutput">Set to true, if the game should show its output.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The number of <paramref name="players" /> is not within the range from 1
        /// to 5.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="players" /> is null</exception>
        internal Game(IReadOnlyList<Player> players, Random random, bool withOutput = false)
        {
            if (players is null)
            {
                throw new ArgumentNullException(nameof(players));
            }

            if (players.Count is < GameRules.MinNumberOfPlayersAllowed or > GameRules.MaxNumberOfPlayersAllowed)
            {
                throw new ArgumentOutOfRangeException(nameof(players),
                                                      $"The number of players has to be from {GameRules.MinNumberOfPlayersAllowed} to {GameRules.MaxNumberOfPlayersAllowed}.");
            }

            this.players = players;
            cardDeck     = new CardDeck(random);
            output       = new ConsoleOutput(withOutput);
        }

        /// <summary>
        /// Runs the game until it ends.
        /// </summary>
        /// <returns>The number of cards left at the end of the game.</returns>
        internal int Run()
        {
            DrawStartingCards();
            output.GameStarts(players);

            var currentCommunication = new List<(int relativePosition, PlayerCommunication playerCommunication)>();
            var playStarted = false;
            var numberOfStartDecisionsAskedFor = 0;

            while (true)
            {
                foreach (var player in players)
                {
                    if (!playStarted)
                    {
                        var startDecision = player.GetStartDecision(numberOfStartDecisionsAskedFor / players.Count + 1);
                        output.PlayerStartDecision(player, startDecision);
                        numberOfStartDecisionsAskedFor++;
                        if (startDecision != PlayerStartDecision.WantToStart)
                        {
                            continue;
                        }

                        playStarted = true;
                    }

                    output.PlayersTurn(player, rowsOfCards);
                    var cardsPlayedThisTurn = 0;

                    do
                    {
                        NewCommunication(player, cardsPlayedThisTurn, currentCommunication);
                    } while (PerformNextMove(player, ref cardsPlayedThisTurn, currentCommunication).Decision is not (
                                 PlayerMoveDecision.DoNotWantToPlay or PlayerMoveDecision.CannotPlay or
                                 PlayerMoveDecision.HaveNoCards));

                    if (cardsPlayedThisTurn < numberOfCardsToPlayPerTurn)
                    {
                        // the game ends because a player did not play but had to
                        var numberOfRemainingCards = GetNumberOfRemainingCards();
                        output.GameEnds(numberOfRemainingCards);
                        return numberOfRemainingCards;
                    }

                    DrawCards(player, cardsPlayedThisTurn);
                    if (cardDeck.IsEmpty)
                    {
                        numberOfCardsToPlayPerTurn = GameRules.NumberOfCardsToPlayAfterDeckIsEmpty;
                    }
                }
            }
        }

        private PlayerMove PerformNextMove(Player                           player, ref int cardsPlayedThisTurn,
                                           List<(int, PlayerCommunication)> currentCommunication)
        {
            var move = player.GetNextMove(new PlayerInformation(cardsPlayedThisTurn, numberOfCardsToPlayPerTurn,
                                                                new ReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards
                                                                >(rowsOfCards), currentCommunication));
            output.PlayerMove(move);
            if (!IsDecisionThatCardWillBePlayed(move.Decision))
            {
                return move;
            }

            if (move.CardPlacement is null || !PerformPlayerMove(player, move.CardPlacement))
            {
                output.IllegalMove();
                move = move with {Decision = PlayerMoveDecision.CannotPlay};
            }
            else
            {
                cardsPlayedThisTurn++;
            }

            return move;
        }

        private void NewCommunication(Player                           currentPlayer, int cardsPlayedThisTurn,
                                      List<(int, PlayerCommunication)> currentCommunication)
        {
            currentCommunication.Clear();
            foreach (var playerForCommunication in players.Where(x => x != currentPlayer))
            {
                foreach (var communication in
                    playerForCommunication.GetCommunication(new PlayerInformation(cardsPlayedThisTurn,
                                                                numberOfCardsToPlayPerTurn,
                                                                new ReadOnlyDictionary<RowOfCardsIdentifier,
                                                                    RowOfCards>(rowsOfCards),
                                                                currentCommunication)))
                {
                    // this is a little hack as IReadOnlyList has no method IndexAt
                    var playersAsList = players.ToList();
                    var relativePosition = (playersAsList.IndexOf(playerForCommunication) -
                                            playersAsList.IndexOf(currentPlayer) + players.Count) % players.Count;
                    currentCommunication.Add((relativePosition, communication));
                    output.PlayerCommunication(relativePosition, communication);
                }
            }
        }

        /// <summary>
        /// Evaluates the player decision to see if a card will be played.
        /// </summary>
        /// <param name="decision">The decision of the player.</param>
        /// <returns>True, if a card will be played; otherwise false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The decision is not implemented.</exception>
        private static bool IsDecisionThatCardWillBePlayed(PlayerMoveDecision decision)
        {
            return decision switch
            {
                PlayerMoveDecision.WantToPlay => true,
                PlayerMoveDecision.DoNotWantToPlay or PlayerMoveDecision.CannotPlay or PlayerMoveDecision.HaveNoCards =>
                    false,
                _ => throw new ArgumentOutOfRangeException(nameof(decision), $"{decision} is not supported.")
            };
        }

        /// <summary>
        /// Performs the player move if allowed; otherwise does nothing.
        /// </summary>
        /// <param name="player">The player that does the move.</param>
        /// <param name="placement">The chosen placement for this move.</param>
        /// <returns>True, if the move is allowed, otherwise false.</returns>
        private bool PerformPlayerMove(Player player, CardPlacement placement)
        {
            if (rowsOfCards[placement.RowOfCards].TryAddCard(placement.Card))
            {
                player.HandOverCard(placement.Card);
                return true;
            }

            return false;
        }

        private void DrawCards(Player player, int cardsPlayedThisTurn)
        {
            for (var j = 0; j < cardsPlayedThisTurn; j++)
            {
                if (cardDeck.IsEmpty)
                {
                    output.CardDeckIsEmpty();
                    break;
                }

                var cardDrawn = cardDeck.DrawCard();
                output.PlayerDrawsCard(player, cardDrawn);
                player.ReceiveCard(cardDrawn);
            }
        }

        private int GetNumberOfRemainingCards()
        {
            return GameRules.NumberOfCardsInDeck - rowsOfCards.Values.Sum(x => x.NumberOfPlayedCards);
        }

        private void DrawStartingCards()
        {
            var cardsToDraw = GameRules.GetNumberOfCardsToDraw(players.Count);

            foreach (var player in players)
            {
                for (var i = 0; i < cardsToDraw; i++)
                {
                    player.ReceiveCard(cardDeck.DrawCard());
                }
            }
        }
    }
}