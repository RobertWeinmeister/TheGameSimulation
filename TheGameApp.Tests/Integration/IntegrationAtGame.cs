using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TheGameApp.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    [Timeout(1000)]
    public class IntegrationAtGame
    {
        private readonly Random random = new();

        [Test]
        public void SoloPlayer()
        {
            foreach (var player in TestHelper.AllImplementedPlayers())
            {
                var game = new Game(new List<Player> {player}.AsReadOnly(), random);

                var result = game.Run();

                Assert.That(result, Is.LessThan(50).And.AtLeast(0), $"Failed for {player}.");
            }
        }
    }
}