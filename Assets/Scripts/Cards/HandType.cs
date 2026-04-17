namespace BalatroStyle
{
    /// <summary>
    /// Poker hand classifications, ordered by strength (low → high).
    /// The integer value is used to scale juice (e.g. shake intensity).
    /// </summary>
    public enum HandType
    {
        HighCard      = 0,
        Pair          = 1,
        TwoPair       = 2,
        ThreeOfAKind  = 3,
        Straight      = 4,
        Flush         = 5,
        FullHouse     = 6,
        FourOfAKind   = 7,
        StraightFlush = 8,
        RoyalFlush    = 9
    }
}
