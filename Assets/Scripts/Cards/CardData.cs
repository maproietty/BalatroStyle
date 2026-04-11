using UnityEngine;

namespace BalatroStyle
{
    /// <summary>
    /// ScriptableObject representing a single playing card's static data.
    /// </summary>
    [CreateAssetMenu(fileName = "CardData", menuName = "BalatroStyle/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Identity")]
        public Suit suit;
        public Rank rank;

        [Header("Display")]
        public Sprite frontSprite;
        public string displayName;

        /// <summary>Base chip value for this rank (Balatro standard).</summary>
        public int ChipValue => rank switch
        {
            Rank.Two   => 2,
            Rank.Three => 3,
            Rank.Four  => 4,
            Rank.Five  => 5,
            Rank.Six   => 6,
            Rank.Seven => 7,
            Rank.Eight => 8,
            Rank.Nine  => 9,
            Rank.Ten   => 10,
            Rank.Jack  => 10,
            Rank.Queen => 10,
            Rank.King  => 10,
            Rank.Ace   => 11,
            _ => 0
        };

        /// <summary>Numeric rank value for hand evaluation comparisons.</summary>
        public int RankValue => (int)rank;
    }

    public enum Suit { Clubs, Diamonds, Hearts, Spades }

    public enum Rank
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
        Jack, Queen, King, Ace
    }
}
