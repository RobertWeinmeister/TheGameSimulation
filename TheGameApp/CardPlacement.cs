namespace TheGameApp
{
    /// <summary>
    /// The placement of a card onto a <see cref="TheGameApp.RowOfCards" />.
    /// </summary>
    internal record CardPlacement(RowOfCardsIdentifier RowOfCards, int Card);
}