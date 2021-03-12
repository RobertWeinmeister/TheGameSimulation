using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// Collection of <see cref="Player" /> rules that can be added to a player.
    /// </summary>
    internal static class PlayerMoveRules
    {
        private const int MaxDistanceForSingleMove                                         = 3;
        private const int MaxDistanceForSingleMoveWithCommunication                        = 5;
        private const int MaxDistanceForSingleScoredMove                                   = 4;
        private const int MaxDistanceForSingleScoredMoveWithCommunicationAndTwoCardsToPlay = 5;
        private const int MaxDistanceForSingleScoredMoveWithCommunicationAndOneCardToPlay  = 8;
        private const int ScorePenaltyDoNotPlayHere                                        = 21;
        private const int ScorePenaltyTryNotToPlayHere                                     = 4;
        private const int ScorePenaltyOnlyBadMoveHere                                      = 13;

        [Pure]
        internal static PlayerMove NoCardsInHand(IReadOnlyCollection<int> cardsInHand, PlayerInformation _)
        {
            return cardsInHand.Count == 0
                       ? new PlayerMove(PlayerMoveDecision.HaveNoCards)
                       : new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove PlayBackwardsTrick(IReadOnlyCollection<int> cardsInHand, PlayerInformation info)
        {
            var backwardsPlays = PlayerRulesUtils.FindPlaysForBackwardTrick(cardsInHand, info.RowsOfCards);
            if (backwardsPlays.Count > 0)
            {
                return new PlayerMove(PlayerMoveDecision.WantToPlay,
                                      new CardPlacement(backwardsPlays.First().Key, backwardsPlays.First().Value));
            }

            return new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove OnlyKeepPlayingIfYouHaveTo(IReadOnlyCollection<int> _, PlayerInformation info)
        {
            return info.CardsPlayedThisTurn >= info.CardsToPlayThisTurn
                       ? new PlayerMove(PlayerMoveDecision.DoNotWantToPlay)
                       : new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay(
            IReadOnlyCollection<int> _, PlayerInformation info)
        {
            return info.CardsPlayedThisTurn >= info.CardsToPlayThisTurn &&
                   !info.Communication.Any(x => x.RelativePosition <= 2 &&
                                                x.PlayerCommunication.CommunicationType ==
                                                PlayerCommunicationType.CanNotPlay)
                       ? new PlayerMove(PlayerMoveDecision.DoNotWantToPlay)
                       : new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove PlayMovesUpToLimitedDistance(IReadOnlyCollection<int> cardsInHand,
                                                                PlayerInformation        info)
        {
            var goodPlays = PlayerRulesUtils.FindPossiblePlaysUpToDistance(cardsInHand, info.RowsOfCards,
                                                                           MaxDistanceForSingleMove);

            if (goodPlays.Count > 0)
            {
                foreach (var play in goodPlays.OrderBy(x => x.distance))
                {
                    return new PlayerMove(PlayerMoveDecision.WantToPlay,
                                          new CardPlacement(play.rowIdentifier, play.card));
                }
            }

            return new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove PlayMovesUpToLimitUnlessBlockedByCommunication(
            IReadOnlyCollection<int> cardsInHand, PlayerInformation info)
        {
            var goodPlays =
                PlayerRulesUtils.FindPossiblePlaysUpToDistance(cardsInHand, info.RowsOfCards,
                                                               MaxDistanceForSingleMoveWithCommunication);

            if (goodPlays.Count > 0)
            {
                foreach (var play in goodPlays.OrderBy(x => x.distance))
                {
                    if (!info.Communication.Exists(x => x.PlayerCommunication.Row == play.rowIdentifier &&
                                                        x.PlayerCommunication.CommunicationType is
                                                            PlayerCommunicationType.DoNotPlayHere))
                    {
                        return new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(play.rowIdentifier, play.card));
                    }
                }
            }

            return new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove PlayLowestGapMove(IReadOnlyCollection<int> cardsInHand, PlayerInformation info)
        {
            var plays = PlayerRulesUtils.FindPossiblePlays(cardsInHand, info.RowsOfCards);
            if (plays.Count > 0)
            {
                var bestPlay = plays.OrderBy(x => x.distance).First();
                return new PlayerMove(PlayerMoveDecision.WantToPlay,
                                      new CardPlacement(bestPlay.rowIdentifier, bestPlay.card));
            }

            return new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove PlayBestScoredMoveUpToLimit(IReadOnlyCollection<int> cardsInHand,
                                                               PlayerInformation        info)
        {
            var spm   = new ScorePlayerPlays();
            var plays = spm.GetScoredPlays(cardsInHand, info.RowsOfCards);

            if (plays.Count == 0)
            {
                return new PlayerMove(PlayerMoveDecision.Undecided);
            }

            var bestMove = PlayerRulesUtils.GetBestMoveFromPlays(plays);

            if (bestMove.score <= MaxDistanceForSingleScoredMove)
            {
                return new PlayerMove(PlayerMoveDecision.WantToPlay, bestMove.placements[0]);
            }

            return new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove PlayBestScoredMove(IReadOnlyCollection<int> cardsInHand, PlayerInformation info)
        {
            var spm   = new ScorePlayerPlays();
            var plays = spm.GetScoredPlays(cardsInHand, info.RowsOfCards);

            if (plays.Count == 0)
            {
                return new PlayerMove(PlayerMoveDecision.Undecided);
            }

            var bestMove = PlayerRulesUtils.GetBestMoveFromPlays(plays);

            return new PlayerMove(PlayerMoveDecision.WantToPlay, bestMove.placements[0]);
        }

        [Pure]
        internal static PlayerMove PlayBestScoredMoveUpToLimitConsideringCommunication(
            IReadOnlyCollection<int> cardsInHand, PlayerInformation info)
        {
            var spm   = new ScorePlayerPlays();
            var plays = spm.GetScoredPlays(cardsInHand, info.RowsOfCards);

            if (plays.Count == 0)
            {
                return new PlayerMove(PlayerMoveDecision.Undecided);
            }

            ConsiderCommunication(info, plays);

            var bestMove = PlayerRulesUtils.GetBestMoveFromPlays(plays);

            if (bestMove.score           <= MaxDistanceForSingleScoredMoveWithCommunicationAndTwoCardsToPlay &&
                info.CardsToPlayThisTurn == 2 ||
                bestMove.score           <= MaxDistanceForSingleScoredMoveWithCommunicationAndOneCardToPlay &&
                info.CardsToPlayThisTurn == 1)
            {
                return new PlayerMove(PlayerMoveDecision.WantToPlay, bestMove.placements[0]);
            }

            return new PlayerMove(PlayerMoveDecision.Undecided);
        }

        [Pure]
        internal static PlayerMove PlayBestScoredMoveConsideringCommunication(
            IReadOnlyCollection<int> cardsInHand, PlayerInformation info)
        {
            var spm   = new ScorePlayerPlays();
            var plays = spm.GetScoredPlays(cardsInHand, info.RowsOfCards);

            if (plays.Count == 0)
            {
                return new PlayerMove(PlayerMoveDecision.Undecided);
            }

            ConsiderCommunication(info, plays);

            var bestMove = PlayerRulesUtils.GetBestMoveFromPlays(plays);

            return new PlayerMove(PlayerMoveDecision.WantToPlay, bestMove.placements[0]);
        }

        private static void ConsiderCommunication(PlayerInformation                                  info,
                                                  IList<(List<CardPlacement> placements, int score)> plays)
        {
            for (var i = 0; i < plays.Count; i++)
            {
                if (info.Communication.Exists(x => x.PlayerCommunication.Row == plays[i].placements[0].RowOfCards &&
                                                   x.PlayerCommunication.CommunicationType is PlayerCommunicationType
                                                      .DoNotPlayHere))
                {
                    plays[i] = (new List<CardPlacement>(plays[i].placements),
                                plays[i].score + ScorePenaltyDoNotPlayHere);
                }
                else if (info.Communication.Exists(x =>
                                                       x.PlayerCommunication.Row == plays[i].placements[0].RowOfCards &&
                                                       x.PlayerCommunication.CommunicationType is
                                                           PlayerCommunicationType.TryNotToPlayHere))
                {
                    plays[i] = (new List<CardPlacement>(plays[i].placements),
                                plays[i].score + ScorePenaltyTryNotToPlayHere);
                }

                if (info.Communication.Exists(x => x.RelativePosition        == 1                                 &&
                                                   x.PlayerCommunication.Row == plays[i].placements[0].RowOfCards &&
                                                   x.PlayerCommunication.CommunicationType is PlayerCommunicationType
                                                      .OnlyBadMoveHere))
                {
                    plays[i] = (new List<CardPlacement>(plays[i].placements),
                                plays[i].score - ScorePenaltyOnlyBadMoveHere);
                }
            }
        }
    }
}