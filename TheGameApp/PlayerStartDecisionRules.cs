using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// Collection of <see cref="Player" /> rules to get a <see cref="PlayerStartDecision" />.
    /// </summary>
    internal static class PlayerStartDecisionRules
    {
        [Pure]
        internal static PlayerStartDecision MakeStartDecision(IReadOnlyCollection<int> cardsInHand, int startingRound)
        {
            var spm = new ScorePlayerPlays();
            var rowsAtStart = new Dictionary<RowOfCardsIdentifier, RowOfCards>
                              {
                                  {RowOfCardsIdentifier.FirstRowUp, new RowOfCards(RowOfCardsType.Up)},
                                  {RowOfCardsIdentifier.SecondRowUp, new RowOfCards(RowOfCardsType.Up)},
                                  {RowOfCardsIdentifier.FirstRowDown, new RowOfCards(RowOfCardsType.Down)},
                                  {RowOfCardsIdentifier.SecondRowDown, new RowOfCards(RowOfCardsType.Down)}
                              };

            var possibleStartingPlays = spm.GetScoredPlays(cardsInHand, rowsAtStart).Where(x => x.placements.Count > 1);

            var bestScore = !possibleStartingPlays.Any()
                                ? 999
                                : PlayerRulesUtils.GetBestMoveFromPlays(spm.GetScoredPlays(cardsInHand, rowsAtStart)
                                                                           .Where(x => x.placements.Count > 1)).score;

            return (bestScore, startingRound) switch
            {
                (<13, _) => PlayerStartDecision.WantToStart,
                (<20, 2) => PlayerStartDecision.WantToStart,
                (<20, _) => PlayerStartDecision.CouldStart,
                (_, <3)  => PlayerStartDecision.DoNotWantToStart,
                (_, _)   => PlayerStartDecision.WantToStart
            };
        }
    }
}