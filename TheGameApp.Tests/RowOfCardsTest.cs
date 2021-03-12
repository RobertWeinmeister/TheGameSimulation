using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class RowOfCardsTest
    {
        [Test]
        public void TryAddCard_NewRowDownAdd98_Allowed()
        {
            var row = new RowOfCards(RowOfCardsType.Down);

            Assert.That(row.TryAddCard(98), Is.True);
        }

        [Test]
        public void TryAddCard_NewRowUpAdd2_Allowed()
        {
            var row = new RowOfCards(RowOfCardsType.Up);

            Assert.That(row.TryAddCard(2), Is.True);
        }

        [Test]
        public void TryAddCard_RowDownAddHigherCard_NotAllowed()
        {
            var row = new RowOfCards(RowOfCardsType.Down);
            row.TryAddCard(55);

            Assert.That(row.TryAddCard(56), Is.False);
        }

        [Test]
        public void TryAddCard_RowDownAddHigherCardBy10_Allowed()
        {
            var row = new RowOfCards(RowOfCardsType.Down);
            row.TryAddCard(55);

            Assert.That(row.TryAddCard(65), Is.True);
        }

        [Test]
        public void TryAddCard_RowUpAddLowerCard_NotAllowed()
        {
            var row = new RowOfCards(RowOfCardsType.Up);
            row.TryAddCard(55);

            Assert.That(row.TryAddCard(54), Is.False);
        }

        [Test]
        public void TryAddCard_RowUpAddLowerCardBy10_Allowed()
        {
            var row = new RowOfCards(RowOfCardsType.Up);
            row.TryAddCard(55);

            Assert.That(row.TryAddCard(45), Is.True);
        }
    }
}