namespace TheGameApp
{
    /// <summary>
    /// The decisions the <see cref="Player" /> can make for a <see cref="PlayerMove" />.
    /// </summary>
    internal enum PlayerMoveDecision
    {
        /// <summary>
        /// The <see cref="Player" /> has not decided yet.
        /// </summary>
        Undecided,

        /// <summary>
        /// The <see cref="Player" /> has no more cards to play.
        /// </summary>
        HaveNoCards,

        /// <summary>
        /// The <see cref="Player" /> wants to play a card.
        /// </summary>
        WantToPlay,

        /// <summary>
        /// The <see cref="Player" /> does not want to play a card.
        /// </summary>
        DoNotWantToPlay,

        /// <summary>
        /// The <see cref="Player" /> cannot play a card.
        /// </summary>
        CannotPlay
    }
}