namespace TheGameApp
{
    /// <summary>
    /// A move a <see cref="TheGameApp.Player" /> wants to do.
    /// </summary>
    internal record PlayerMove(PlayerMoveDecision Decision, CardPlacement? CardPlacement = null);
}