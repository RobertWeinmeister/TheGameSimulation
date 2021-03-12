using System.Collections.Generic;
using NUnit.Framework;

namespace TheGameApp.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class PlayerTests
    {
        [Test]
        public void GetCommunication_PlaysBackwardsAndSmallestDistanceWithCommunication_CommunicatesBackwardTrick()
        {
            var player = PlayerFactory.PlaysBackwardsAndSmallestDistanceWithCommunication();
            player.ReceiveCard(90);
            var rows = TestHelper.GetPreparedRows(10, 12, 80, 15);
            var expectedCommunication = new List<PlayerCommunication>
                                        {
                                            new(RowOfCardsIdentifier.FirstRowDown,
                                                PlayerCommunicationType.DoNotPlayHere)
                                        };

            var communication =
                player.GetCommunication(new PlayerInformation(0, 2, rows, new List<(int, PlayerCommunication)>()));

            Assert.That(communication, Is.EqualTo(expectedCommunication));
        }

        [Test]
        public void GetNextMove_PlayerCanNeverPlayIfNotPossibleToPlay()
        {
            // 15 can go nowhere
            foreach (var player in TestHelper.AllImplementedPlayers())
            {
                player.ReceiveCard(15);
                var rows         = TestHelper.GetPreparedRows(20, 21, 10, 6);
                var expectedMove = new PlayerMove(PlayerMoveDecision.CannotPlay);

                var move =
                    player.GetNextMove(new PlayerInformation(0, 2, rows, new List<(int, PlayerCommunication)>()));

                Assert.That(move, Is.EqualTo(expectedMove), $"Failed for {player}.");
            }
        }

        [Test]
        public void GetNextMove_PlayerHasNoCards()
        {
            foreach (var player in TestHelper.AllImplementedPlayers())
            {
                var rows         = TestHelper.GetPreparedRows(20, 21, 10, 6);
                var expectedMove = new PlayerMove(PlayerMoveDecision.HaveNoCards);

                var move =
                    player.GetNextMove(new PlayerInformation(0, 2, rows, new List<(int, PlayerCommunication)>()));

                Assert.That(move, Is.EqualTo(expectedMove), $"Failed for {player}.");
            }
        }

        [Test]
        public void GetNextMove_PlaysBackwardsAndSmallestDistance_DoesNotWantToPlayIfDoesNotHaveTo()
        {
            // could play 60, but does not have to
            var player = PlayerFactory.PlaysBackwardsAndSmallestDistance();
            player.ReceiveCard(60);
            var rows         = TestHelper.GetPreparedRows(1, 2, 70, 75);
            var expectedMove = new PlayerMove(PlayerMoveDecision.DoNotWantToPlay);

            var move = player.GetNextMove(new PlayerInformation(2, 2, rows, new List<(int, PlayerCommunication)>()));

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void GetNextMove_PlaysBackwardsAndSmallestDistance_PlaysBackwardOnDown()
        {
            // plays 85 on 75
            var player = PlayerFactory.PlaysBackwardsAndSmallestDistance();
            player.ReceiveCard(85);
            var rows = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowDown, 85));

            var move = player.GetNextMove(new PlayerInformation(0, 2, rows, new List<(int, PlayerCommunication)>()));

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void GetNextMove_PlaysBackwardsAndSmallestDistance_PlaysBackwardOnUp()
        {
            // plays 2 on 12
            var player = PlayerFactory.PlaysBackwardsAndSmallestDistance();
            player.ReceiveCard(2);
            var rows = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 2));

            var move = player.GetNextMove(new PlayerInformation(0, 2, rows, new List<(int, PlayerCommunication)>()));

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void GetNextMove_PlaysBackwardsAndSmallestDistance_PlaySmallestDistanceIfBackwardNotPossible()
        {
            // plays 17 on 20
            var player = PlayerFactory.PlaysBackwardsAndSmallestDistance();
            player.ReceiveCard(17);
            var rows = TestHelper.GetPreparedRows(11, 12, 20, 75);
            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.FirstRowDown, 17));

            var move = player.GetNextMove(new PlayerInformation(0, 2, rows, new List<(int, PlayerCommunication)>()));

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void GetNextMove_PlaysBackwardsAndSmallestDistanceWithCommunication_DoesNotPlayOnBackwardTrick()
        {
            // 13 should go on 12, but is blocked, goes for 13 on 15
            var player = PlayerFactory.PlaysBackwardsAndSmallestDistanceWithCommunication();
            player.ReceiveCard(13);
            var rows = TestHelper.GetPreparedRows(10, 12, 80, 15);
            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowDown, 13));

            var move = player.GetNextMove(new PlayerInformation(0, 2, rows,
                                                                new List<(int, PlayerCommunication)>
                                                                {
                                                                    (1,
                                                                     new
                                                                         PlayerCommunication(RowOfCardsIdentifier.SecondRowUp,
                                                                             PlayerCommunicationType
                                                                                .DoNotPlayHere))
                                                                }));

            Assert.That(move, Is.EqualTo(expectedMove));
        }
    }
}