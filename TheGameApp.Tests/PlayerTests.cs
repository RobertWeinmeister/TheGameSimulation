using System.Collections.Generic;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class PlayerTests
    {
        [Test]
        public void GetCommunication_CommunicationRuleAdded_CommunicationRuleEvaluated()
        {
            var communication = new List<PlayerCommunication>
                                {
                                    new(RowOfCardsIdentifier.SecondRowDown, PlayerCommunicationType.DoNotPlayHere)
                                };

            var player = Player.NewPlayer("test").AddCommunicationRule((_, _) => communication);

            Assert.That(player.GetCommunication(TestHelper.EmptyPlayerInformation), Is.EqualTo(communication));
        }

        [Test]
        public void GetCommunication_StandardImplementation_NoCommunication()
        {
            var player = Player.NewPlayer("test");

            Assert.That(player.GetCommunication(TestHelper.EmptyPlayerInformation), Is.Empty);
        }

        [Test]
        public void GetNextMove_MoveRuleAdded_MoveRuleEvaluated()
        {
            var moveToDo = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                          new CardPlacement(RowOfCardsIdentifier.FirstRowUp, 2));
            var player = Player.NewPlayer("test").AddMoveRule((_, _) => moveToDo);

            Assert.That(player.GetNextMove(TestHelper.EmptyPlayerInformation), Is.EqualTo(moveToDo));
        }

        [Test]
        public void GetStartDecision_StandardImplementation_WantToStart()
        {
            var player = Player.NewPlayer("test");

            Assert.That(player.GetStartDecision(1), Is.EqualTo(PlayerStartDecision.WantToStart));
        }

        [Test]
        public void HandOverCard_CardInHand_HandsCardOver()
        {
            const int card   = 25;
            var       player = TestHelper.PlayerThatNeverWantsToPlay;
            player.ReceiveCard(card);

            player.HandOverCard(card);
        }

        [Test]
        public void HandOverCard_CardNotInHand_ThrowsException()
        {
            var player = TestHelper.PlayerThatNeverWantsToPlay;
            player.ReceiveCard(23);

            Assert.Catch(() => player.HandOverCard(22));
        }
    }
}