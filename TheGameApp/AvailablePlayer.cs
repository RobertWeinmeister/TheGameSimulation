namespace TheGameApp
{
    /// <summary>
    /// The available player types.
    /// </summary>
    internal enum AvailablePlayer
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
        PlaysBackwardsAndSmallestDistance,

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
        PlaysBackwardsAndSmallestDistanceWithCommunication,

        /// <summary>
        /// Player with extended communication.
        /// </summary>
        PlaysBestScoredPlaysWithCommunication,

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
        PlaysBestScoredPlaysWithSimpleCommunication,

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
        PlaysBestScoredPlays
    }
}