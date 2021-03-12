using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    [Timeout(100)]
    public class GameTests
    {
        private readonly Random random = new();

        [Test]
        public void Run_PlayerCannotPlay_GameEnds()
        {
            var players = new List<Player> {TestHelper.PlayerThatCanNeverPlay};
            var game    = new Game(players, random);

            var result = game.Run();

            Assert.That(result, Is.EqualTo(98));
        }

        [Test]
        public void Run_PlayerDoesNotWantToPlayButHasTo_GameEnds()
        {
            var players = new List<Player> {TestHelper.PlayerThatNeverWantsToPlay};
            var game    = new Game(players, random);

            var result = game.Run();

            Assert.That(result, Is.EqualTo(98));
        }
    }
}