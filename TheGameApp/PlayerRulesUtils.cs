using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// Collection of <see cref="Player" /> rules that can be added to a player.
    /// </summary>
    internal static class PlayerRulesUtils
    {
        [Pure]
        internal static (List<CardPlacement> placements, int score) GetBestMoveFromPlays(
            IEnumerable<(List<CardPlacement> placements, int score)> plays)
        {
            return plays.Aggregate((a, b) => a.score < b.score ? a : b);
        }

        [Pure]
        internal static Dictionary<RowOfCardsIdentifier, int> FindPlaysForBackwardTrick(
            IReadOnlyCollection<int> cardsInHand, ReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards> rowsOfCards)
        {
            var possiblePlays = new Dictionary<RowOfCardsIdentifier, int>();

            foreach (var (rowId, row) in rowsOfCards)
            {
                foreach (var card in cardsInHand
                                    .Where(x => x == row.CardOnTop +
                                                (row.Type == RowOfCardsType.Up
                                                     ? -GameRules.AllowedBackwardsTrick
                                                     : +GameRules.AllowedBackwardsTrick)).ToList())
                {
                    possiblePlays.Add(rowId, card);
                }
            }

            return possiblePlays;
        }

        [Pure]
        internal static List<(RowOfCardsIdentifier rowIdentifier, int distance, int card)>
            FindPossiblePlaysUpToDistance(IReadOnlyCollection<int>                             cardsInHand,
                                          ReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards> rowsOfCards,
                                          int                                                  maxDistance)
        {
            var plays = FindPossiblePlays(cardsInHand, rowsOfCards);

            return plays.Where(x => x.distance <= maxDistance                                       &&
                                    !cardsInHand.Contains(x.card + GameRules.AllowedBackwardsTrick) &&
                                    !cardsInHand.Contains(x.card - GameRules.AllowedBackwardsTrick)).ToList();
        }

        [Pure]
        internal static List<(RowOfCardsIdentifier rowIdentifier, int distance, int card)> FindPossiblePlays(
            IReadOnlyCollection<int> cardsInHand, ReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards> rowsOfCards)
        {
            var plays = new List<(RowOfCardsIdentifier, int distance, int card)>();

            foreach (var (rowId, row) in rowsOfCards)
            {
                plays.AddRange(cardsInHand
                              .Select(x => (rowId,
                                            row.Type == RowOfCardsType.Up ? x - row.CardOnTop : row.CardOnTop - x, x))
                              .Where(x => x.Item2 > 0));
            }

            return plays;
        }
    }
}