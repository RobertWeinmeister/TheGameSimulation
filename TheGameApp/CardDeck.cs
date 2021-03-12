using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// The card deck for "The Game".
    /// </summary>
    internal class CardDeck
    {
        private readonly IReadOnlyList<int> shuffledCards;
        private          int                currentCardIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardDeck" /> class.
        /// </summary>
        /// <param name="random">The random generator.</param>
        /// <remarks>
        ///     <para>The cards are automatically shuffled on initialization.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="random" /> is null</exception>
        internal CardDeck(Random random)
        {
            if (random is null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var cards = Enumerable.Range(GameRules.LowestNumberInGame + 1, GameRules.NumberOfCardsInDeck).ToList();

            ShuffleCards(random, cards);

            shuffledCards    = cards.AsReadOnly();
            currentCardIndex = 0;
        }

        /// <summary>
        /// Returns whether the deck is empty.
        /// </summary>
        /// <value>
        /// True if the deck is empty; otherwise false.
        /// </value>
        internal bool IsEmpty => currentCardIndex > shuffledCards.Count - 1;

        /// <summary>
        /// Draws the next card.
        /// </summary>
        /// <returns>The next card.</returns>
        /// <exception cref="InvalidOperationException">The deck is empty.</exception>
        internal int DrawCard()
        {
            if (currentCardIndex > shuffledCards.Count - 1)
            {
                throw new InvalidOperationException("Deck is empty");
            }

            var cardToDeal = shuffledCards[currentCardIndex];
            currentCardIndex++;
            return cardToDeal;
        }

        private static void ShuffleCards(Random random, IList<int> cards)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var j = random.Next(0, i + 1);
                if (j != i)
                {
                    cards[i] = cards[j];
                }

                cards[j] = i + GameRules.LowestNumberInGame + 1;
            }
        }
    }
}