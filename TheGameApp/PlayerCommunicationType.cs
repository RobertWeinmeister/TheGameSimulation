namespace TheGameApp
{
    /// <summary>
    /// The possible types of allowed <see cref="PlayerCommunication" />.
    /// </summary>
    internal enum PlayerCommunicationType
    {
        /// <summary>
        /// No one should play here.
        /// </summary>
        DoNotPlayHere,

        /// <summary>
        /// One should try not to play here.
        /// </summary>
        TryNotToPlayHere,

        /// <summary>
        /// The player can only play a bad move here.
        /// </summary>
        OnlyBadMoveHere,

        /// <summary>
        /// The player can not play at all with the current rows.
        /// </summary>
        CanNotPlay
    }
}