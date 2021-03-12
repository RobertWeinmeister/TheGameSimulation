using System;
using NUnit.Framework;

namespace TheGameApp.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class ApplicationArgumentReaderTests
    {
        [Test]
        public void GetConfigurationFromArguments_ArgumentValueNotANumber_InvalidConfiguration()
        {
            var      reader    = new ApplicationArgumentReader();
            string[] arguments = {"runs=xxx", "parallelism=2"};

            var (isValid, _) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid, Is.False);
        }

        [Test]
        public void GetConfigurationFromArguments_ArgumentValueNotInRange_InvalidConfiguration()
        {
            var      reader    = new ApplicationArgumentReader();
            string[] arguments = {"runs=2", "parallelism=0"};

            var (isValid, _) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid, Is.False);
        }

        [Test]
        public void GetConfigurationFromArguments_CaseOfArgumentsDoesNotMatter_AreIgnored()
        {
            var      reader    = new ApplicationArgumentReader();
            string[] arguments = {"RUns=2", "PARALLELISM=5"};

            var (isValid, configuration) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid,                    Is.True);
            Assert.That(configuration!.Runs,        Is.EqualTo(2));
            Assert.That(configuration!.Parallelism, Is.EqualTo(5));
        }

        [Test]
        public void GetConfigurationFromArguments_DuplicatePlayerTypesGiven_IgnoresDuplicatePlayerTypes()
        {
            var      reader    = new ApplicationArgumentReader();
            string[] arguments = {"1", "2", "2", "3", "3", "3"};

            var (isValid, configuration) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid,                               Is.True);
            Assert.That(configuration!.AvailablePlayers.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetConfigurationFromArguments_NoArguments_StandardValuesUsed()
        {
            var reader    = new ApplicationArgumentReader();
            var arguments = Array.Empty<string>();

            var (isValid, configuration) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid,                    Is.True);
            Assert.That(configuration!.Parallelism, Is.EqualTo(5));
        }

        [Test]
        public void GetConfigurationFromArguments_NoPlayerTypeGiven_AllAvailablePlayersUsed()
        {
            var      reader    = new ApplicationArgumentReader();
            string[] arguments = Array.Empty<string>();

            var (isValid, configuration) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid, Is.True);
            Assert.That(configuration!.AvailablePlayers.Count,
                        Is.EqualTo(Enum.GetValues(typeof(AvailablePlayer)).Length));
        }

        [Test]
        public void GetConfigurationFromArguments_PlayerTypesGiven_HasTournamentTypes()
        {
            var      reader    = new ApplicationArgumentReader();
            string[] arguments = {"runs=2", "1", "3"};

            var (isValid, configuration) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid,                               Is.True);
            Assert.That(configuration!.AvailablePlayers.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetConfigurationFromArguments_UnknownArguments_AreIgnored()
        {
            var      reader    = new ApplicationArgumentReader();
            string[] arguments = {"one=two", "three", "99"};

            var (isValid, _) = reader.GetConfigurationFromArguments(arguments);

            Assert.That(isValid, Is.True);
        }
    }
}