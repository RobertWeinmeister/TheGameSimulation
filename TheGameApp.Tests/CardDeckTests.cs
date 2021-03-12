using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class CardDeckTests
    {
        private readonly Random random = new();

        [Test]
        [Retry(3)]
        public void DrawCard_CardsAreDrawnFromDifferentDecks_CardsAreDifferent()
        {
            var deckA = new CardDeck(random);
            var deckB = new CardDeck(random);

            Assert.That(deckA.DrawCard(), Is.Not.EqualTo(deckB.DrawCard()));
        }

        [Test]
        public void DrawCard_CardsAreDrawnUntilDeckIsEmpty_AllCardsAreDifferentAndMatchGameRules()
        {
            var deck = new CardDeck(random);
            var set  = new HashSet<int>();

            while (!deck.IsEmpty)
            {
                set.Add(deck.DrawCard());
            }

            Assert.That(set.Count, Is.EqualTo(98));
            Assert.That(set.Min(), Is.EqualTo(2));
            Assert.That(set.Max(), Is.EqualTo(99));
        }

        [Test]
        public void IsEmpty_98DrawsFromNewCardDeck_IsEmpty()
        {
            var deck = new CardDeck(random);

            for (var i = 1; i <= 98; i++)
            {
                deck.DrawCard();
            }

            Assert.That(deck.IsEmpty, Is.True);
        }

        [Test]
        public void IsEmpty_NewCardDeck_IsNotEmpty()
        {
            var deck = new CardDeck(random);

            Assert.That(deck.IsEmpty, Is.False);
        }
    }
}