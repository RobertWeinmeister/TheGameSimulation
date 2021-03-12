namespace TheGameApp
{
    /// <summary>
    /// The communication between <see cref="TheGameApp.Player" />s.
    /// </summary>
    internal record PlayerCommunication(RowOfCardsIdentifier Row, PlayerCommunicationType CommunicationType);
}