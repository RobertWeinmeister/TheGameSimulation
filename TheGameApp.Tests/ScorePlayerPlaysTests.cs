using System.Collections.Generic;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    [Timeout(100)]
    public class ScorePlayerPlaysTests
    {
        private readonly ScorePlayerPlays spm = new();

        [Test]
        [Timeout(200)]
        public void GetScoredPlays_EightCardsAtTheStart_FinishesInTime()
        {
            var cards = new HashSet<int>
                        {
                            11,
                            22,
                            33,
                            44,
                            55,
                            66,
                            77,
                            88
                        };
            var rows = TestHelper.GetPreparedRows(1, 1, 99, 99);

            spm.GetScoredPlays(cards, rows);
        }

        [Test]
        public void GetScoredPlays_SingleCardForDown_IsBestPlay()
        {
            var cards             = new HashSet<int> {12};
            var rows              = TestHelper.GetPreparedRows(1, 2, 19, 25);
            var expectedPlacement = new CardPlacement(RowOfCardsIdentifier.FirstRowDown, 12);

            var bestPlacement = TestHelper.GetBestCardPlacementFromScoredPlays(spm.GetScoredPlays(cards, rows));

            Assert.That(bestPlacement, Is.EqualTo(expectedPlacement));
        }

        [Test]
        public void GetScoredPlays_SingleCardForUp_IsBestPlay()
        {
            var cards             = new HashSet<int> {6};
            var rows              = TestHelper.GetPreparedRows(1, 2, 19, 25);
            var expectedPlacement = new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 6);

            var bestPlacement = TestHelper.GetBestCardPlacementFromScoredPlays(spm.GetScoredPlays(cards, rows));

            Assert.That(bestPlacement, Is.EqualTo(expectedPlacement));
        }

        [Test]
        public void GetScoredPlays_ThreeCards_PlayForBackwardsTrickIsBest()
        {
            var cards             = new HashSet<int> {6, 45, 35};
            var rows              = TestHelper.GetPreparedRows(1, 5, 44, 99);
            var expectedPlacement = new CardPlacement(RowOfCardsIdentifier.FirstRowDown, 35);

            var bestPlacement = TestHelper.GetBestCardPlacementFromScoredPlays(spm.GetScoredPlays(cards, rows));

            Assert.That(bestPlacement, Is.EqualTo(expectedPlacement));
        }

        [Test]
        public void GetScoredPlays_TwoCards_BackwardsTrickOnDownIsBestPlay()
        {
            var cards             = new HashSet<int> {6, 35};
            var rows              = TestHelper.GetPreparedRows(1, 5, 34, 25);
            var expectedPlacement = new CardPlacement(RowOfCardsIdentifier.SecondRowDown, 35);

            var bestPlacement = TestHelper.GetBestCardPlacementFromScoredPlays(spm.GetScoredPlays(cards, rows));

            Assert.That(bestPlacement, Is.EqualTo(expectedPlacement));
        }

        [Test]
        public void GetScoredPlays_TwoCards_BackwardsTrickOnUpIsBestPlay()
        {
            var cards             = new HashSet<int> {6, 45};
            var rows              = TestHelper.GetPreparedRows(1, 55, 34, 25);
            var expectedPlacement = new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 45);

            var bestPlacement = TestHelper.GetBestCardPlacementFromScoredPlays(spm.GetScoredPlays(cards, rows));

            Assert.That(bestPlacement, Is.EqualTo(expectedPlacement));
        }
    }
}