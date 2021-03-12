using System.Collections.Generic;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// Score all possible plays the player could do.
    /// </summary>
    internal class ScorePlayerPlays
    {
        /// <summary>
        /// Limits the processing of useless plays for performance reasons.
        /// </summary>
        private const int ScoreCutOff = +33;

        private int                                               bestScore;
        private int                                               currentScore;
        private List<(List<CardPlacement> placements, int score)> scoredPlays = new();
        private Queue<QueueStruct>                                queue       = new();

        /// <summary>
        /// Get possible plays and their scores.
        /// </summary>
        /// <param name="cardsInHand">The cards in hand.</param>
        /// <param name="rows">The rows.</param>
        /// <returns>
        /// Possible plays and their scores.
        /// </returns>
        /// <remarks>
        ///     <para>For performance reasons, not all possible plays are scored and returned, only promising ones.</para>
        /// </remarks>
        internal List<(List<CardPlacement> placements, int score)> GetScoredPlays(IReadOnlyCollection<int> cardsInHand,
            IReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards>                                          rows)
        {
            // the lower the score, the better
            currentScore = 0;
            currentScore = Score(rows.Values.Select(x => x.CardOnTop).ToList());
            bestScore    = 999;
            scoredPlays  = new List<(List<CardPlacement> placements, int score)>();
            queue        = new Queue<QueueStruct>();
            queue.Enqueue(new QueueStruct(new List<CardPlacement>(), cardsInHand,
                                          rows.Values.Select(x => x.CardOnTop).ToList()));
            WorkThroughQueue();
            return scoredPlays;
        }

        private void WorkThroughQueue()
        {
            while (queue.TryDequeue(out var next))
            {
                foreach (var card in next.RemainingCardsInHand)
                {
                    for (var i = 0; i < next.TopCards.Count; i++)
                    {
                        // allows treating the up and down rows in the same loop
                        var upDown = i < 2 ? +1 : -1;

                        if ((upDown * card > upDown * next.TopCards[i] || card == next.TopCards[i] - upDown * 10) &&
                            Score(next.TopCards) + upDown * (card - next.TopCards[i]) < bestScore + ScoreCutOff)
                        {
                            var newTopCards             = new List<int>(next.TopCards) {[i] = card};
                            var newRemainingCardsInHand = new HashSet<int>(next.RemainingCardsInHand);
                            newRemainingCardsInHand.Remove(card);
                            var newPlacements =
                                new List<CardPlacement>(next.CardPlacements) {new((RowOfCardsIdentifier) i, card)};
                            var newScore = Score(newTopCards);
                            if (newScore < bestScore)
                            {
                                bestScore = newScore;
                            }

                            scoredPlays.Add((newPlacements, newScore));
                            queue.Enqueue(new QueueStruct(newPlacements, newRemainingCardsInHand, newTopCards));
                        }
                    }
                }
            }
        }

        private int Score(IReadOnlyList<int> rows)
        {
            return rows[0] + rows[1] - rows[2] - rows[3] + 200 - currentScore;
        }

        private record QueueStruct(List<CardPlacement> CardPlacements, IReadOnlyCollection<int> RemainingCardsInHand,
                                   List<int>           TopCards);
    }
}