using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TheGameApp
{
    /// <summary>
    /// The information that is handed to a <see cref="TheGameApp.Player" /> to make his
    /// <see cref="TheGameApp.PlayerMoveDecision" /> or
    /// <see cref="TheGameApp.PlayerCommunication" />.
    /// </summary>
    internal record PlayerInformation(int CardsPlayedThisTurn, int CardsToPlayThisTurn,
                                      in ReadOnlyDictionary<RowOfCardsIdentifier, RowOfCards> RowsOfCards,
                                      List<(int RelativePosition, PlayerCommunication PlayerCommunication)>
                                          Communication);
}