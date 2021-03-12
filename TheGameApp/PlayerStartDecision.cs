namespace TheGameApp
{
    /// <summary>
    /// The decisions a <see cref="Player" /> can make at the start of the <see cref="Game" />.
    /// </summary>
    internal enum PlayerStartDecision
    {
        /// <summary>
        /// The <see cref="Player" /> wants to start.
        /// </summary>
        WantToStart,

        /// <summary>
        /// The <see cref="Player" /> could start, if necessary.
        /// </summary>
        CouldStart,

        /// <summary>
        /// The <see cref="Player" /> does not want to start, if possible.
        /// </summary>
        DoNotWantToStart
    }
}