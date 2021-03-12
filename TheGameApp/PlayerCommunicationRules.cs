using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// Collection of <see cref="Player" /> rules to get <see cref="PlayerCommunication" />.
    /// </summary>
    internal static class PlayerCommunicationRules
    {
        private const int GoodMoveDistance = 4;
        private const int BadMoveDistance  = 14;

        internal static List<PlayerCommunication> CommunicateBackwardsTrick(
            IReadOnlyCollection<int> cardsInHand, PlayerInformation info)
        {
            if (cardsInHand.Count == 0)
            {
                return new List<PlayerCommunication>();
            }

            return PlayerRulesUtils.FindPlaysForBackwardTrick(cardsInHand, info.RowsOfCards).Keys.Distinct()
                                   .Select(row => new PlayerCommunication(row, PlayerCommunicationType.DoNotPlayHere))
                                   .ToList();
        }

        [Pure]
        internal static List<PlayerCommunication> CommunicateCanNotPlay(IReadOnlyCollection<int> cardsInHand,
                                                                        PlayerInformation        info)
        {
            if (cardsInHand.Count == 0)
            {
                return new List<PlayerCommunication>();
            }

            if (PlayerRulesUtils.FindPossiblePlaysUpToDistance(cardsInHand, info.RowsOfCards, int.MaxValue).Count <
                info.CardsToPlayThisTurn)
            {
                return new List<PlayerCommunication>
                       {
                           new(RowOfCardsIdentifier.FirstRowUp, PlayerCommunicationType.CanNotPlay)
                       };
            }

            return new List<PlayerCommunication>();
        }

        [Pure]
        internal static List<PlayerCommunication> CommunicateGoodMove(IReadOnlyCollection<int> cardsInHand,
                                                                      PlayerInformation        info)
        {
            if (cardsInHand.Count == 0)
            {
                return new List<PlayerCommunication>();
            }

            return PlayerRulesUtils.FindPossiblePlaysUpToDistance(cardsInHand, info.RowsOfCards, GoodMoveDistance)
                                   .Select(x => x.rowIdentifier).Distinct()
                                   .Select(row =>
                                               new PlayerCommunication(row, PlayerCommunicationType.TryNotToPlayHere))
                                   .ToList();
        }

        [Pure]
        internal static List<PlayerCommunication> CommunicateBadMove(IReadOnlyCollection<int> cardsInHand,
                                                                     PlayerInformation        info)
        {
            if (cardsInHand.Count == 0)
            {
                return new List<PlayerCommunication>();
            }

            if (PlayerRulesUtils.FindPlaysForBackwardTrick(cardsInHand, info.RowsOfCards).Count > 0)
            {
                return new List<PlayerCommunication>();
            }

            var worstDistance = int.MaxValue;
            var worstRow      = RowOfCardsIdentifier.FirstRowUp;

            foreach (var row in info.RowsOfCards)
            {
                var currentDistance = (int) row.Key < 2
                                          ? cardsInHand.Select(x => x - row.Value.CardOnTop).Where(x => x > 0)
                                                       .DefaultIfEmpty(int.MaxValue).Min()
                                          : cardsInHand.Select(x => row.Value.CardOnTop - x).Where(x => x > 0)
                                                       .DefaultIfEmpty(int.MaxValue).Min();

                if (currentDistance < worstDistance)
                {
                    worstDistance = currentDistance;
                    worstRow      = row.Key;
                }
            }

            return worstDistance < BadMoveDistance
                       ? new List<PlayerCommunication>()
                       : new List<PlayerCommunication> {new(worstRow, PlayerCommunicationType.OnlyBadMoveHere)};
        }
    }
}