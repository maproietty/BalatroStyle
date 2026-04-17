using System.Collections.Generic;
using System.Linq;

namespace BalatroStyle
{
    /// <summary>
    /// Pure static evaluator that classifies a played group of 1–5 cards into a
    /// <see cref="HandType"/> and returns the subset of cards that score. No Unity
    /// dependencies — safe to unit-test and use off the main thread if ever needed.
    /// Base chip and multiplier values follow Balatro conventions.
    /// </summary>
    public static class PokerHandEvaluator
    {
        // Balatro-style base chips × mult per hand type.
        private const int HighCardChips      = 5,   HighCardMult      = 1;
        private const int PairChips          = 10,  PairMult          = 2;
        private const int TwoPairChips       = 20,  TwoPairMult       = 2;
        private const int ThreeKindChips     = 30,  ThreeKindMult     = 3;
        private const int StraightChips      = 30,  StraightMult      = 4;
        private const int FlushChips         = 35,  FlushMult         = 4;
        private const int FullHouseChips     = 40,  FullHouseMult     = 4;
        private const int FourKindChips      = 60,  FourKindMult      = 7;
        private const int StraightFlushChips = 100, StraightFlushMult = 8;
        private const int RoyalFlushChips    = 100, RoyalFlushMult    = 8;

        private const int RequiredForStraightOrFlush = 5;

        /// <summary>
        /// Evaluate a played group of cards and return the best-matching hand.
        /// An empty or null input returns a HighCard result with no scoring cards.
        /// </summary>
        public static EvaluatedHand Evaluate(IReadOnlyList<CardData> played)
        {
            if (played == null || played.Count == 0)
                return new EvaluatedHand(HandType.HighCard, new List<CardData>(),
                                         HighCardChips, HighCardMult, "High Card");

            bool isFlush = played.Count == RequiredForStraightOrFlush && AllSameSuit(played);
            bool isRoyal = false;
            bool isStraight = played.Count == RequiredForStraightOrFlush && IsStraight(played, out isRoyal);

            // Group by rank for pair/trips/quads/full-house detection.
            var byRank = played.GroupBy(c => c.rank)
                               .OrderByDescending(g => g.Count())
                               .ThenByDescending(g => (int)g.Key)
                               .ToList();

            int topCount = byRank[0].Count();
            int secondCount = byRank.Count > 1 ? byRank[1].Count() : 0;

            // Resolve in strength order, highest first.
            if (isStraight && isFlush && isRoyal)
                return Make(HandType.RoyalFlush, played, RoyalFlushChips, RoyalFlushMult, "Royal Flush");

            if (isStraight && isFlush)
                return Make(HandType.StraightFlush, played, StraightFlushChips, StraightFlushMult, "Straight Flush");

            if (topCount == 4)
                return Make(HandType.FourOfAKind, byRank[0].ToList(), FourKindChips, FourKindMult, "Four of a Kind");

            if (topCount == 3 && secondCount == 2)
                return Make(HandType.FullHouse, played, FullHouseChips, FullHouseMult, "Full House");

            if (isFlush)
                return Make(HandType.Flush, played, FlushChips, FlushMult, "Flush");

            if (isStraight)
                return Make(HandType.Straight, played, StraightChips, StraightMult, "Straight");

            if (topCount == 3)
                return Make(HandType.ThreeOfAKind, byRank[0].ToList(), ThreeKindChips, ThreeKindMult, "Three of a Kind");

            if (topCount == 2 && secondCount == 2)
            {
                var scoring = new List<CardData>();
                scoring.AddRange(byRank[0]);
                scoring.AddRange(byRank[1]);
                return Make(HandType.TwoPair, scoring, TwoPairChips, TwoPairMult, "Two Pair");
            }

            if (topCount == 2)
                return Make(HandType.Pair, byRank[0].ToList(), PairChips, PairMult, "Pair");

            // High Card: only the single highest-rank card scores.
            var highest = played.OrderByDescending(c => (int)c.rank).First();
            return Make(HandType.HighCard, new List<CardData> { highest },
                        HighCardChips, HighCardMult, "High Card");
        }

        private static EvaluatedHand Make(HandType type, IReadOnlyList<CardData> scoring,
                                          int chips, int mult, string name)
            => new EvaluatedHand(type, scoring, chips, mult, name);

        private static bool AllSameSuit(IReadOnlyList<CardData> cards)
        {
            Suit first = cards[0].suit;
            for (int i = 1; i < cards.Count; i++)
                if (cards[i].suit != first) return false;
            return true;
        }

        /// <summary>
        /// Returns true if five cards form a consecutive run. Supports the wheel
        /// straight (A-2-3-4-5). Sets <paramref name="isRoyal"/> when the straight
        /// is 10-J-Q-K-A.
        /// </summary>
        private static bool IsStraight(IReadOnlyList<CardData> cards, out bool isRoyal)
        {
            isRoyal = false;
            if (cards.Count != RequiredForStraightOrFlush) return false;

            var ranks = cards.Select(c => (int)c.rank).OrderBy(r => r).ToList();

            // Detect duplicates first — a straight has 5 distinct ranks.
            for (int i = 1; i < ranks.Count; i++)
                if (ranks[i] == ranks[i - 1]) return false;

            // Standard consecutive check.
            bool consecutive = true;
            for (int i = 1; i < ranks.Count; i++)
                if (ranks[i] != ranks[i - 1] + 1) { consecutive = false; break; }

            if (consecutive)
            {
                isRoyal = ranks[0] == (int)Rank.Ten && ranks[4] == (int)Rank.Ace;
                return true;
            }

            // Wheel: A-2-3-4-5 — Ace counts as 1.
            bool isWheel = ranks[0] == (int)Rank.Two
                        && ranks[1] == (int)Rank.Three
                        && ranks[2] == (int)Rank.Four
                        && ranks[3] == (int)Rank.Five
                        && ranks[4] == (int)Rank.Ace;
            return isWheel;
        }
    }
}
