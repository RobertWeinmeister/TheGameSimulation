using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class PlayerStartDecisionRulesTests
    {
        [Test]
        public void MakeStartDecision_BadCardsInFirstRound_DoNotWantToStart()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        35,
                                                        30,
                                                        41,
                                                        52,
                                                        63,
                                                        97
                                                    });
            var decision = PlayerStartDecisionRules.MakeStartDecision(cards, 1);

            Assert.That(decision, Is.EqualTo(PlayerStartDecision.DoNotWantToStart));
        }

        [Test]
        public void MakeStartDecision_OkayCardsInSecondRound_WantToStart()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        10,
                                                        13,
                                                        30,
                                                        41,
                                                        52,
                                                        63
                                                    });
            var decision = PlayerStartDecisionRules.MakeStartDecision(cards, 2);

            Assert.That(decision, Is.EqualTo(PlayerStartDecision.WantToStart));
        }

        [Test]
        public void MakeStartDecision_OnePerfectCardRestBad_DoNotWantToStartToWantToStart()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        2,
                                                        44,
                                                        45,
                                                        46,
                                                        47,
                                                        48
                                                    });
            var decision = PlayerStartDecisionRules.MakeStartDecision(cards, 1);

            Assert.That(decision, Is.EqualTo(PlayerStartDecision.DoNotWantToStart));

            decision = PlayerStartDecisionRules.MakeStartDecision(cards, 3);

            Assert.That(decision, Is.EqualTo(PlayerStartDecision.WantToStart));
        }

        [Test]
        public void MakeStartDecision_ReallyGoodCardsInFirstRound_WantToStart()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        2,
                                                        30,
                                                        41,
                                                        52,
                                                        63,
                                                        97
                                                    });
            var decision = PlayerStartDecisionRules.MakeStartDecision(cards, 1);

            Assert.That(decision, Is.EqualTo(PlayerStartDecision.WantToStart));
        }

        [Test]
        public void MakeStartDecision_StartScoreOfLessThan13_WantToStart()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        22,
                                                        12,
                                                        40,
                                                        41,
                                                        42,
                                                        43,
                                                        44
                                                    });
            var startDecision = PlayerStartDecisionRules.MakeStartDecision(cards, 1);

            Assert.That(startDecision, Is.EqualTo(PlayerStartDecision.WantToStart));
        }

        [Test]
        public void MakeStartDecision_StartScoreOfLessThan20_CouldStartToWantToStart()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        8,
                                                        93,
                                                        40,
                                                        41,
                                                        42,
                                                        43,
                                                        44
                                                    });
            var startDecision = PlayerStartDecisionRules.MakeStartDecision(cards, 1);

            Assert.That(startDecision, Is.EqualTo(PlayerStartDecision.CouldStart));

            startDecision = PlayerStartDecisionRules.MakeStartDecision(cards, 2);

            Assert.That(startDecision, Is.EqualTo(PlayerStartDecision.WantToStart));
        }

        [Test]
        public void MakeStartDecision_StartScoreOfMoreThan20_DoNotWantToStartToWantToStart()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        40,
                                                        41,
                                                        42,
                                                        43,
                                                        44
                                                    });
            var startDecision = PlayerStartDecisionRules.MakeStartDecision(cards, 1);

            Assert.That(startDecision, Is.EqualTo(PlayerStartDecision.DoNotWantToStart));

            startDecision = PlayerStartDecisionRules.MakeStartDecision(cards, 2);

            Assert.That(startDecision, Is.EqualTo(PlayerStartDecision.DoNotWantToStart));

            startDecision = PlayerStartDecisionRules.MakeStartDecision(cards, 3);

            Assert.That(startDecision, Is.EqualTo(PlayerStartDecision.WantToStart));
        }
    }
}