using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace TheGameApp
{
    /// <summary>
    /// Provides preset <see cref="Player" />s.
    /// </summary>
    internal static class PlayerFactory
    {
        /// <summary>
        ///     <para><see cref="Player" /> implementation with three rules:</para>
        ///     <list type="number">
        ///         <item>
        ///             <description>Play every possible backwards trick.</description>
        ///         </item>
        ///         <item>
        ///             <description>Play every card with a distance up to 3.</description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///             If more cards need to be played, play them in descending order of distance
        ///             until the minimum number of cards to play is reached.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="name">Name of the player.</param>
        [Pure]
        internal static Player PlaysBackwardsAndSmallestDistance(string name = "PlaysBackwardsAndSmallestDistance")
        {
            return Player.NewPlayer(name).AddMoveRule(PlayerMoveRules.NoCardsInHand)
                         .AddMoveRule(PlayerMoveRules.PlayBackwardsTrick)
                         .AddMoveRule(PlayerMoveRules.PlayMovesUpToLimitedDistance)
                         .AddMoveRule(PlayerMoveRules.OnlyKeepPlayingIfYouHaveTo)
                         .AddMoveRule(PlayerMoveRules.PlayLowestGapMove);
        }

        /// <summary>
        ///     <para><see cref="Player" /> implementation that communicates possible backward tricks, with three rules:</para>
        ///     <list type="number">
        ///         <item>
        ///             <description>Play every possible backwards trick.</description>
        ///         </item>
        ///         <item>
        ///             <description>Play every card with a distance up to 5 unless blocked by DoNotPlayHere.</description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///             If more cards need to be played, play them in descending order of distance
        ///             until the minimum number of cards to play is reached.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="name">Name of the player.</param>
        [Pure]
        internal static Player PlaysBackwardsAndSmallestDistanceWithCommunication(
            string name = "PlaysBackwardsAndSmallestDistanceWithCommunication")
        {
            return Player.NewPlayer(name).AddMoveRule(PlayerMoveRules.NoCardsInHand)
                         .AddMoveRule(PlayerMoveRules.PlayBackwardsTrick)
                         .AddMoveRule(PlayerMoveRules.PlayMovesUpToLimitUnlessBlockedByCommunication)
                         .AddMoveRule(PlayerMoveRules.OnlyKeepPlayingIfYouHaveTo)
                         .AddMoveRule(PlayerMoveRules.PlayLowestGapMove)
                         .AddCommunicationRule(PlayerCommunicationRules.CommunicateBackwardsTrick)
                         .SetStartRule(PlayerStartDecisionRules.MakeStartDecision);
        }

        /// <summary>
        /// Gets the actual players from the available players.
        /// </summary>
        /// <param name="availablePlayers">The available players.</param>
        /// <returns>The actual players.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="availablePlayers" /> is null</exception>
        /// <exception cref="NotSupportedException"></exception>
        [Pure]
        internal static List<Player> GetPlayers(IReadOnlyList<AvailablePlayer> availablePlayers)
        {
            if (availablePlayers is null)
            {
                throw new ArgumentNullException(nameof(availablePlayers));
            }

            var players = new List<Player>();
            for (var i = 0; i < availablePlayers.Count; i++)
            {
                switch (availablePlayers[i])
                {
                    case AvailablePlayer.PlaysBackwardsAndSmallestDistance:
                        players.Add(PlaysBackwardsAndSmallestDistance(((char) (i + 65)).ToString(CultureInfo
                                                                         .InvariantCulture)));
                        break;
                    case AvailablePlayer.PlaysBackwardsAndSmallestDistanceWithCommunication:
                        players.Add(PlaysBackwardsAndSmallestDistanceWithCommunication(((char) (i + 65))
                                       .ToString(CultureInfo.InvariantCulture)));
                        break;
                    case AvailablePlayer.PlaysBestScoredPlays:
                        players.Add(PlaysBestScoredPlays(((char) (i + 65)).ToString(CultureInfo.InvariantCulture)));
                        break;
                    case AvailablePlayer.PlaysBestScoredPlaysWithSimpleCommunication:
                        players.Add(PlaysBestScoredPlaysWithSimpleCommunication(((char) (i + 65)).ToString(CultureInfo
                                                                                   .InvariantCulture)));
                        break;
                    case AvailablePlayer.PlaysBestScoredPlaysWithCommunication:
                        players.Add(PlaysBestScoredPlaysWithCommunication(((char) (i + 65)).ToString(CultureInfo
                                                                             .InvariantCulture)));
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return players;
        }

        /// <summary>
        /// Player with extended communication.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        [Pure]
        private static Player PlaysBestScoredPlaysWithCommunication(
            string name = "BackwardsTrickWithOptionalBestScoredMove")
        {
            return Player.NewPlayer(name).AddMoveRule(PlayerMoveRules.NoCardsInHand)
                         .AddMoveRule(PlayerMoveRules.PlayBackwardsTrick)
                         .AddMoveRule(PlayerMoveRules.PlayBestScoredMoveUpToLimitConsideringCommunication)
                         .AddMoveRule(PlayerMoveRules.KeepPlayingIfYouHaveToOrTheNextPlayerCouldNotPlay)
                         .AddMoveRule(PlayerMoveRules.PlayBestScoredMoveConsideringCommunication)
                         .AddCommunicationRule(PlayerCommunicationRules.CommunicateBackwardsTrick)
                         .AddCommunicationRule(PlayerCommunicationRules.CommunicateGoodMove)
                         .AddCommunicationRule(PlayerCommunicationRules.CommunicateCanNotPlay)
                         .AddCommunicationRule(PlayerCommunicationRules.CommunicateBadMove)
                         .SetStartRule(PlayerStartDecisionRules.MakeStartDecision);
        }

        /// <summary>
        ///     <para><see cref="Player" /> implementation that communicates, with two rules:</para>
        ///     <list type="number">
        ///         <item>
        ///             <description>
        ///             Play every play with a score change up to 5 if two cards have to be played, otherwise up to 8.
        ///             A row with DoNotPlayHere has a score penalty of 21.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///             If more cards need to be played, play them by lowest score
        ///             until the minimum number of cards to play is reached.
        ///             A row with DoNotPlayHere has a score penalty of 21.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="name">Name of the player.</param>
        [Pure]
        private static Player PlaysBestScoredPlaysWithSimpleCommunication(
            string name = "PlaysBestScoredPlaysWithSimpleCommunication")
        {
            return Player.NewPlayer(name).AddMoveRule(PlayerMoveRules.NoCardsInHand)
                         .AddMoveRule(PlayerMoveRules.PlayBackwardsTrick)
                         .AddMoveRule(PlayerMoveRules.PlayBestScoredMoveUpToLimitConsideringCommunication)
                         .AddMoveRule(PlayerMoveRules.OnlyKeepPlayingIfYouHaveTo)
                         .AddMoveRule(PlayerMoveRules.PlayBestScoredMoveConsideringCommunication)
                         .AddCommunicationRule(PlayerCommunicationRules.CommunicateBackwardsTrick)
                         .SetStartRule(PlayerStartDecisionRules.MakeStartDecision);
        }

        /// <summary>
        ///     <para><see cref="Player" /> implementation with two rules:</para>
        ///     <list type="number">
        ///         <item>
        ///             <description>
        ///             Play every play with a score change up to 4.
        ///             (This includes all backwards tricks.)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///             If more cards need to be played, play them by lowest score
        ///             until the minimum number of cards to play is reached.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <param name="name">name of the player.</param>
        [Pure]
        private static Player PlaysBestScoredPlays(string name = "PlaysBestScoredPlays")
        {
            return Player.NewPlayer(name).AddMoveRule(PlayerMoveRules.NoCardsInHand)
                         .AddMoveRule(PlayerMoveRules.PlayBackwardsTrick)
                         .AddMoveRule(PlayerMoveRules.PlayBestScoredMoveUpToLimit)
                         .AddMoveRule(PlayerMoveRules.OnlyKeepPlayingIfYouHaveTo)
                         .AddMoveRule(PlayerMoveRules.PlayBestScoredMove);
        }
    }
}