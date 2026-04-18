using System.Collections.Generic;

namespace BalatroStyle
{
    /// <summary>
    /// Result of evaluating a played group of cards.
    /// Carries the hand's classification, the subset of cards that actually score
    /// (Balatro-style — e.g. only the pair in a Pair hand), and the base chips and
    /// multiplier contributed by the hand type itself. Final score is
    /// (BaseChips + sum of ScoringCards' chip values) × BaseMultiplier.
    /// </summary>
    public readonly struct EvaluatedHand
    {
        /// <summary>The poker classification of the played cards.</summary>
        public readonly HandType Type;

        /// <summary>The cards that contribute chips to the score (participating subset).</summary>
        public readonly IReadOnlyList<CardData> ScoringCards;

        /// <summary>Chips contributed by the hand type itself, before adding card chip values.</summary>
        public readonly int BaseChips;

        /// <summary>Multiplier contributed by the hand type.</summary>
        public readonly int BaseMultiplier;

        /// <summary>Human-readable name for display (e.g. "Full House").</summary>
        public readonly string DisplayName;

        /// <summary>Construct an evaluated hand result.</summary>
        public EvaluatedHand(HandType type, IReadOnlyList<CardData> scoringCards,
                             int baseChips, int baseMultiplier, string displayName)
        {
            Type = type;
            ScoringCards = scoringCards;
            BaseChips = baseChips;
            BaseMultiplier = baseMultiplier;
            DisplayName = displayName;
        }

        /// <summary>Total chips contributed = BaseChips + sum of ScoringCards' ChipValue.</summary>
        public int TotalChips
        {
            get
            {
                int total = BaseChips;
                if (ScoringCards != null)
                {
                    for (int i = 0; i < ScoringCards.Count; i++)
                        total += ScoringCards[i].ChipValue;
                }
                return total;
            }
        }

        /// <summary>Final score = TotalChips × BaseMultiplier.</summary>
        public int FinalScore => TotalChips * BaseMultiplier;
    }
}
