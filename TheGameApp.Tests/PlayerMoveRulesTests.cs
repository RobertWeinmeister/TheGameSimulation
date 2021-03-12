using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class PlayerMoveRulesTests
    {
        [Test]
        public void
            KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay_DoesNotHaveToPlayAndNextPlayerCouldPlay_DecisionDoNotWantToPlay()
        {
            // has to play 60 or 3
            var cards = new ReadOnlyCollection<int>(new List<int> {60, 3});
            var rows  = TestHelper.GetPreparedRows(1, 2, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var result = PlayerMoveRules.KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay(cards, info);

            Assert.That(result.Decision, Is.EqualTo(PlayerMoveDecision.DoNotWantToPlay));
        }

        [Test]
        public void
            KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay_DoesNotHaveToPlayButNextPlayerCouldNotPlay_DecisionUndecided()
        {
            // could play 60 or 3, but does not have to, but should
            var cards = new ReadOnlyCollection<int>(new List<int> {60, 3});
            var rows  = TestHelper.GetPreparedRows(1, 2, 70, 75);
            var comm = new List<(int, PlayerCommunication)>
                       {
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.FirstRowUp,
                                                    PlayerCommunicationType.CanNotPlay))
                       };
            var info = new PlayerInformation(3, 2, rows, comm);

            var result = PlayerMoveRules.KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay(cards, info);

            Assert.That(result.Decision, Is.EqualTo(PlayerMoveDecision.Undecided));
        }

        [Test]
        public void
            KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay_HasToPlayAndNextPlayerCouldPlay_DecisionUndecided()
        {
            // has to play 60 or 3
            var cards = new ReadOnlyCollection<int>(new List<int> {60, 3});
            var rows  = TestHelper.GetPreparedRows(1, 2, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 2, rows, comm);

            var result = PlayerMoveRules.KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay(cards, info);

            Assert.That(result.Decision, Is.EqualTo(PlayerMoveDecision.Undecided));
        }

        [Test]
        public void NoCardsInHand_NoCards_DecisionHaveNoCards()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>());
            var rows  = TestHelper.GetPreparedRows(20, 21, 10, 6);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(0, 2, rows, comm);

            var result = PlayerMoveRules.NoCardsInHand(cards, info);

            Assert.That(result.Decision, Is.EqualTo(PlayerMoveDecision.HaveNoCards));
        }

        [Test]
        public void OnlyKeepPlayingIfYouHaveTo_DoesNotHaveToPlay_DecisionDoNotWantToPlay()
        {
            // could play 60 or 3, but does not have to
            var cards = new ReadOnlyCollection<int>(new List<int> {60, 3});
            var rows  = TestHelper.GetPreparedRows(1, 2, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(2, 2, rows, comm);

            var result = PlayerMoveRules.OnlyKeepPlayingIfYouHaveTo(cards, info);

            Assert.That(result.Decision, Is.EqualTo(PlayerMoveDecision.DoNotWantToPlay));
        }

        [Test]
        public void PlayBackwardsTrick_BackwardsTrickOnDownPossible_PlayBackwardsTrickOnDown()
        {
            // plays 85 on 75
            var cards = new ReadOnlyCollection<int>(new List<int> {85, 13});
            var rows  = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();

            var info = new PlayerInformation(0, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowDown, 85));

            var move = PlayerMoveRules.PlayBackwardsTrick(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayBackwardsTrick_NoBackwardsTrickPossible_DecisionUndecided()
        {
            // plays 2 on 12
            var cards = new ReadOnlyCollection<int>(new List<int> {3, 69});
            var rows  = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.Undecided);

            var move = PlayerMoveRules.PlayBackwardsTrick(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayBestScoredMove_MovePossible_PlayMove()
        {
            var cards = new ReadOnlyCollection<int>(new List<int> {35, 36, 37, 38});
            var rows  = TestHelper.GetPreparedRows(1, 2, 80, 70);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowDown, 38));

            var move = PlayerMoveRules.PlayBestScoredMove(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayBestScoredMoveConsideringCommunication_CommunicationBlocksTwoPlays_PlayOverallBestPlay()
        {
            // 29 - 39 on 30 is blocked, but still the best
            // 61 on 80 is less good
            // 8 on 2 is blocked
            var cards = new ReadOnlyCollection<int>(new List<int> {39, 29, 61, 8});
            var rows  = TestHelper.GetPreparedRows(30, 2, 99, 80);
            var comm = new List<(int, PlayerCommunication)>
                       {
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.FirstRowUp,
                                                    PlayerCommunicationType.DoNotPlayHere)),
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.SecondRowDown,
                                                    PlayerCommunicationType.TryNotToPlayHere)),
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.SecondRowUp,
                                                    PlayerCommunicationType.DoNotPlayHere))
                       };
            var info = new PlayerInformation(0, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.FirstRowUp, 39));

            var move = PlayerMoveRules.PlayBestScoredMoveConsideringCommunication(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayBestScoredMoveUpToLimit_BestWouldBeAboveLimit_DecisionUndecided()
        {
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        7,
                                                        8,
                                                        9,
                                                        10,
                                                        11
                                                    });
            var rows = TestHelper.GetPreparedRows(1, 2, 80, 70);
            var comm = new List<(int, PlayerCommunication)>();
            var info = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.Undecided);

            var move = PlayerMoveRules.PlayBestScoredMoveUpToLimit(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayBestScoredMoveUpToLimit_LongChainOfBackwardsTricksPossible_PlayMove()
        {
            // play 38 on 70, as 38 - 37 - 47 -57 - 67 possible
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        67,
                                                        57,
                                                        47,
                                                        37,
                                                        38
                                                    });
            var rows = TestHelper.GetPreparedRows(98, 99, 80, 70);
            var comm = new List<(int, PlayerCommunication)>();
            var info = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowDown, 38));

            var move = PlayerMoveRules.PlayBestScoredMoveUpToLimit(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void
            PlayBestScoredMoveWithScoreChangeUpToLimitConsideringCommunication_CommunicationAllowsOtherwiseBadPlay_PlayBadPlay()
        {
            // 79 on 80 would be best
            // but next players worst move would be on second row up, so 10 there is better
            var cards = new ReadOnlyCollection<int>(new List<int> {75, 10});
            var rows  = TestHelper.GetPreparedRows(1, 2, 99, 80);
            var comm = new List<(int, PlayerCommunication)>
                       {
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.SecondRowUp,
                                                    PlayerCommunicationType.OnlyBadMoveHere))
                       };
            var info = new PlayerInformation(2, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 10));

            var move = PlayerMoveRules.PlayBestScoredMoveUpToLimitConsideringCommunication(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void
            PlayBestScoredMoveWithScoreChangeUpToLimitConsideringCommunication_CommunicationBlocksTwoPlays_PlayRemainingPlay()
        {
            // 28 - 38 on 30 is blocked
            // 76 on 80 is blocked
            // 8 on 2 is played
            var cards = new ReadOnlyCollection<int>(new List<int> {38, 28, 76, 8});
            var rows  = TestHelper.GetPreparedRows(30, 2, 99, 80);
            var comm = new List<(int, PlayerCommunication)>
                       {
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.FirstRowUp,
                                                    PlayerCommunicationType.DoNotPlayHere)),
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.SecondRowDown,
                                                    PlayerCommunicationType.TryNotToPlayHere))
                       };
            var info = new PlayerInformation(0, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 8));

            var move = PlayerMoveRules.PlayBestScoredMoveUpToLimitConsideringCommunication(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void
            PlayBestScoredMoveWithScoreChangeUpToLimitConsideringCommunication_PlayCardsBetweenBackwardsTrickCards_PlaysIt()
        {
            // 4, 12, 2 is better than 12, 2, 4
            var cards = new ReadOnlyCollection<int>(new List<int> {2, 4, 12, 13});
            var rows  = TestHelper.GetPreparedRows(1, 80, 75, 78);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(0, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.FirstRowUp, 4));

            var move = PlayerMoveRules.PlayBestScoredMoveUpToLimitConsideringCommunication(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));

            cards = new ReadOnlyCollection<int>(new List<int> {2, 12, 13});
            rows  = TestHelper.GetPreparedRows(4, 80, 75, 78);
            comm  = new List<(int, PlayerCommunication)>();
            info  = new PlayerInformation(1, 2, rows, comm);
            expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                          new CardPlacement(RowOfCardsIdentifier.FirstRowUp, 12));

            move = PlayerMoveRules.PlayBestScoredMoveUpToLimitConsideringCommunication(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayLowestGapMove_MovePossible_PlayMove()
        {
            // play 50 on 5
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        2,
                                                        3,
                                                        98,
                                                        99,
                                                        50
                                                    });
            var rows = TestHelper.GetPreparedRows(4, 5, 97, 96);
            var comm = new List<(int, PlayerCommunication)>();
            var info = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 50));

            var move = PlayerMoveRules.PlayLowestGapMove(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayLowestGapMove_OnlyOneMovePossible_PlaysPossibleMove()
        {
            // 13 should not go anywhere
            var cards = new ReadOnlyCollection<int>(new List<int>
                                                    {
                                                        1,
                                                        2,
                                                        98,
                                                        99,
                                                        49
                                                    });
            var rows = TestHelper.GetPreparedRows(3, 4, 96, 97);
            var comm = new List<(int, PlayerCommunication)>
                       {
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.FirstRowUp,
                                                    PlayerCommunicationType.DoNotPlayHere)),
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.SecondRowUp,
                                                    PlayerCommunicationType.DoNotPlayHere)),
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.FirstRowDown,
                                                    PlayerCommunicationType.DoNotPlayHere)),
                           (1,
                            new PlayerCommunication(RowOfCardsIdentifier.SecondRowDown,
                                                    PlayerCommunicationType.DoNotPlayHere))
                       };
            var info = new PlayerInformation(0, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 49));

            var move = PlayerMoveRules.PlayLowestGapMove(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayMovesUpToLimitedDistance_MovePossible_PlayMove()
        {
            // plays 15 on 12
            var cards = new ReadOnlyCollection<int>(new List<int> {3, 15});
            var rows  = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 15));

            var move = PlayerMoveRules.PlayMovesUpToLimitedDistance(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayMovesUpToLimitedDistance_MovePossible_PlayMove_NoMovePossible_DecisionUndecided()
        {
            // 16 on 12 is too big a distance
            var cards = new ReadOnlyCollection<int>(new List<int> {3, 16});
            var rows  = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.Undecided);

            var move = PlayerMoveRules.PlayMovesUpToLimitedDistance(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayMovesUpToLimitUnlessBlockedByCommunication_MovePossible_PlayMove()
        {
            // plays 17 on 12
            var cards = new ReadOnlyCollection<int>(new List<int> {3, 17});
            var rows  = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.WantToPlay,
                                              new CardPlacement(RowOfCardsIdentifier.SecondRowUp, 17));

            var move = PlayerMoveRules.PlayMovesUpToLimitUnlessBlockedByCommunication(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }

        [Test]
        public void PlayMovesUpToLimitUnlessBlockedByCommunication_MovePossibleButBlocked_DecisionUndecided()
        {
            // 13 on 12 is possible, but blocked
            var cards = new ReadOnlyCollection<int>(new List<int> {3, 13});
            var rows  = TestHelper.GetPreparedRows(11, 12, 70, 75);
            var comm  = new List<(int, PlayerCommunication)>();
            var info  = new PlayerInformation(1, 2, rows, comm);

            var expectedMove = new PlayerMove(PlayerMoveDecision.Undecided);

            var move = PlayerMoveRules.PlayMovesUpToLimitUnlessBlockedByCommunication(cards, info);

            Assert.That(move, Is.EqualTo(expectedMove));
        }
    }
}