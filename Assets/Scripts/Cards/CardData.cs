using UnityEngine;
using UnityEngine.Serialization;

namespace BalatroStyle
{
    /// <summary>
    /// ScriptableObject representing a single playing card's static data.
    /// </summary>
    [CreateAssetMenu(fileName = "CardData", menuName = "BalatroStyle/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Identity")]
        [FormerlySerializedAs("suit")]
        [SerializeField] private Suit cardSuit;
        [FormerlySerializedAs("rank")]
        [SerializeField] private Rank cardRank;

        [Header("Display")]
        [FormerlySerializedAs("frontSprite")]
        [SerializeField] private Sprite cardFrontSprite;
        [FormerlySerializedAs("displayName")]
        [SerializeField] private string cardDisplayName;

        /// <summary>The card's suit (Clubs, Diamonds, Hearts, Spades).</summary>
        public Suit suit => cardSuit;

        /// <summary>The card's rank (Two through Ace).</summary>
        public Rank rank => cardRank;

        /// <summary>Sprite shown on the card face.</summary>
        public Sprite frontSprite => cardFrontSprite;

        /// <summary>Human-readable card name (e.g. "Ace of Spades").</summary>
        public string displayName => cardDisplayName;

        /// <summary>Base chip value for this rank (Balatro standard).</summary>
        public int ChipValue => cardRank switch
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
        public int RankValue => (int)cardRank;
    }

    public enum Suit { Clubs, Diamonds, Hearts, Spades }

    public enum Rank
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
        Jack, Queen, King, Ace
    }
}
