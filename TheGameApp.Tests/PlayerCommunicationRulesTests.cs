using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class PlayerCommunicationRulesTests
    {
        [Test]
        public void CommunicateBackwardsTrick_BackwardsTrickPossible_CommunicateDoNotPlayHere()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {13, 24});
            var rows  = TestHelper.GetPreparedRows(5, 23, 80, 15);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var expectedCommunication = new List<PlayerCommunication>
                                        {
                                            new(RowOfCardsIdentifier.SecondRowUp, PlayerCommunicationType.DoNotPlayHere)
                                        };

            var communication = PlayerCommunicationRules.CommunicateBackwardsTrick(cards, info);

            Assert.That(communication, Is.EqualTo(expectedCommunication));
        }

        [Test]
        public void CommunicateBadMove_BackwardTrickPossible_CommunicateNothing()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {90});
            var rows  = TestHelper.GetPreparedRows(10, 12, 80, 70);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var expectedCommunication = new List<PlayerCommunication>();

            var communication = PlayerCommunicationRules.CommunicateBadMove(cards, info);

            Assert.That(communication, Is.EqualTo(expectedCommunication));
        }

        [Test]
        public void CommunicateBadMove_GoodMovePossible_CommunicateNothing()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {90, 61});
            var rows  = TestHelper.GetPreparedRows(10, 12, 80, 70);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var expectedCommunication = new List<PlayerCommunication>();

            var communication = PlayerCommunicationRules.CommunicateBadMove(cards, info);

            Assert.That(communication, Is.EqualTo(expectedCommunication));
        }

        [Test]
        public void CommunicateBadMove_OnlyBadMovePossible_CommunicateOnlyBadMove()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {50});
            var rows  = TestHelper.GetPreparedRows(10, 12, 80, 70);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var expectedCommunication = new List<PlayerCommunication>
                                        {
                                            new(RowOfCardsIdentifier.SecondRowDown,
                                                PlayerCommunicationType.OnlyBadMoveHere)
                                        };

            var communication = PlayerCommunicationRules.CommunicateBadMove(cards, info);

            Assert.That(communication, Is.EqualTo(expectedCommunication));
        }

        [Test]
        public void CommunicateCanNotPlay_CanNotPlaySameCardTwiceWithTwoRequiredCards_CommunicateCanNotPlay()
        {
            // 80 can go on 79 up and 90 up, but is the same card
            var cards = new ReadOnlyCollection<int>(new List<int> {76, 77, 78, 80});
            var rows  = TestHelper.GetPreparedRows(79, 90, 75, 10);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var communication = PlayerCommunicationRules.CommunicateCanNotPlay(cards, info);

            Assert.That(communication.Count(x => x.CommunicationType == PlayerCommunicationType.CanNotPlay),
                        Is.AtLeast(1));
        }

        [Test]
        public void CommunicateCanNotPlay_CanNotPlayWithOneRequiredCard_CommunicateCanNotPlay()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {76, 77, 78});
            var rows  = TestHelper.GetPreparedRows(80, 90, 75, 10);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 1, rows, comm);

            var communication = PlayerCommunicationRules.CommunicateCanNotPlay(cards, info);

            Assert.That(communication.Count(x => x.CommunicationType == PlayerCommunicationType.CanNotPlay),
                        Is.AtLeast(1));
        }

        [Test]
        public void CommunicateCanNotPlay_CanPlay_CommunicateNothing()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {70, 76, 77, 78});
            var rows  = TestHelper.GetPreparedRows(80, 90, 20, 71);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 1, rows, comm);

            var communication = PlayerCommunicationRules.CommunicateCanNotPlay(cards, info);

            Assert.That(communication, Is.Empty);
        }

        [Test]
        public void CommunicateCanNotPlay_CanPlayWithTwoRequiredCards_CommunicateNothing()
        {
            // 69 on 79, 21 on 11
            var cards = new ReadOnlyCollection<int>(new List<int> {69, 21});
            var rows  = TestHelper.GetPreparedRows(79, 90, 11, 10);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var communication = PlayerCommunicationRules.CommunicateCanNotPlay(cards, info);

            Assert.That(communication.Count(x => x.CommunicationType == PlayerCommunicationType.CanNotPlay),
                        Is.AtLeast(1));
        }

        [Test]
        public void CommunicateCanNotPlay_HasNoCards_CommunicateNothing()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>());
            var rows  = TestHelper.GetPreparedRows(80, 90, 75, 10);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var communication = PlayerCommunicationRules.CommunicateCanNotPlay(cards, info);

            Assert.That(communication, Is.Empty);
        }

        [Test]
        public void CommunicateGoodMove_MoveWithDistance2Possible_CommunicateTryNotToPlayHere()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {13, 24});
            var rows  = TestHelper.GetPreparedRows(5, 23, 80, 15);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var expectedCommunication = new List<PlayerCommunication>
                                        {
                                            new(RowOfCardsIdentifier.SecondRowUp,
                                                PlayerCommunicationType.TryNotToPlayHere),
                                            new(RowOfCardsIdentifier.SecondRowDown,
                                                PlayerCommunicationType.TryNotToPlayHere)
                                        };

            var communication = PlayerCommunicationRules.CommunicateGoodMove(cards, info);

            Assert.That(communication, Is.EqualTo(expectedCommunication));
        }
    }
}