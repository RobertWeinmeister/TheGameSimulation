using System.Collections.Generic;
using System.Linq;

namespace TheGameApp
{
    /// <summary>
    /// A row of cards for "The Game".
    /// </summary>
    internal class RowOfCards
    {
        private const    int       StartCardUp   = GameRules.LowestNumberInGame;
        private const    int       StartCardDown = GameRules.HighestNumberInGame;
        private readonly List<int> cards         = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RowOfCards" /> class.
        /// </summary>
        /// <param name="type">The type of the row.</param>
        internal RowOfCards(RowOfCardsType type)
        {
            Type = type;
            cards.Add(type == RowOfCardsType.Up ? StartCardUp : StartCardDown);
        }

        /// <summary>
        /// The type of the row.
        /// </summary>
        internal RowOfCardsType Type { get; }

        /// <summary>
        /// Gets the number of cards played on this row.
        /// </summary>
        /// <value>
        /// The number of cards.
        /// </value>
        internal int NumberOfPlayedCards => cards.Count - 1;

        /// <summary>
        /// Gets the card on top of the row.
        /// </summary>
        /// <value>
        /// The card on top of the row.
        /// </value>
        internal int CardOnTop => cards.Last();

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Type}: {cards.Last()}";
        }

        /// <summary>
        /// Tries to add the card if allowed.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if the card can be added; otherwise false.</returns>
        internal bool TryAddCard(int card)
        {
            if (!CanAddCard(card))
            {
                return false;
            }

            cards.Add(card);
            return true;
        }

        private bool CanAddCard(int card)
        {
            switch (Type)
            {
                case RowOfCardsType.Up
                    when card > cards.Last() || card + GameRules.AllowedBackwardsTrick == cards.Last():
                case RowOfCardsType.Down
                    when card < cards.Last() || card - GameRules.AllowedBackwardsTrick == cards.Last():
                    return true;
                default:
                    return false;
            }
        }
    }
}