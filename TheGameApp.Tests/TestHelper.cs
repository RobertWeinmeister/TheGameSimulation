using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheGameApp.Tests
{
    internal static class TestHelper
    {
        internal static Player PlayerThatCanNeverPlay =>
            Player.NewPlayer("CannotPlay").AddMoveRule((_, _) => new PlayerMove(PlayerMoveDecision.CannotPlay));

        internal static Player PlayerThatNeverWantsToPlay =>
            Player.NewPlayer("DoesNotWantToPlay")
                  .AddMoveRule((_, _) => new PlayerMove(PlayerMoveDecision.DoNotWantToPlay));

        internal static PlayerInformation EmptyPlayerInformation =>
            new(0, 0,
                new ReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards>(new Dictionary<RowOfCardsIdentifier, RowOfCards
                                                                         >()), new List<(int, PlayerCommunication)>());

        internal static CardPlacement GetBestCardPlacementFromScoredPlays(
            IEnumerable<(List<CardPlacement> placements, int score)> scoredPlays)
        {
            return scoredPlays.Aggregate((a, b) => a.score < b.score ? a : b).placements[0];
        }

        /// <summary>
        /// Gets the prepared rows.
        /// </summary>
        /// <param name="firstUp"></param>
        /// <param name="secondUp"></param>
        /// <param name="firstDown"></param>
        /// <param name="secondDown"></param>
        /// <returns>The prepared rows with only the given cards added.</returns>
        internal static ReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards> GetPreparedRows(
            int firstUp, int secondUp, int firstDown, int secondDown)
        {
            return new(new Dictionary<RowOfCardsIdentifier, RowOfCards>
                       {
                           {RowOfCardsIdentifier.FirstRowUp, RowUpWithHighestCard(firstUp)},
                           {RowOfCardsIdentifier.SecondRowUp, RowUpWithHighestCard(secondUp)},
                           {RowOfCardsIdentifier.FirstRowDown, RowDownWithLowestCard(firstDown)},
                           {RowOfCardsIdentifier.SecondRowDown, RowDownWithLowestCard(secondDown)}
                       });
        }

        internal static IEnumerable<Player> AllImplementedPlayers()
        {
            return from AvailablePlayer player in Enum.GetValues(typeof(AvailablePlayer))
                   select PlayerFactory.GetPlayers(new List<AvailablePlayer> {player})[0];
        }

        private static RowOfCards RowUpWithHighestCard(int card)
        {
            var row = new RowOfCards(RowOfCardsType.Up);
            row.TryAddCard(card);
            return row;
        }

        private static RowOfCards RowDownWithLowestCard(int card)
        {
            var row = new RowOfCards(RowOfCardsType.Down);
            row.TryAddCard(card);
            return row;
        }
    }
}