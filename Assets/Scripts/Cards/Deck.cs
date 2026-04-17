using UnityEngine;
using System.Collections.Generic;

namespace BalatroStyle
{
    /// <summary>
    /// Runtime deck — holds all CardData assets, handles shuffle and draw.
    /// Populate via the inspector by dragging all 52 CardData assets into allCards.
    /// </summary>
    public class Deck : MonoBehaviour
    {
        [SerializeField] private List<CardData> allCards = new();

        private List<CardData> drawPile = new();
        private List<CardData> discardPile = new();

        private void Start()
        {
            Reset();
        }

        /// <summary>Rebuild and shuffle the draw pile from all 52 cards.</summary>
        public void Reset()
        {
            drawPile = new List<CardData>(allCards);
            discardPile.Clear();
            Shuffle();
        }

        /// <summary>Draw the top card from the pile; reshuffles discard if pile is empty.</summary>
        public CardData Draw()
        {
            // Self-initialize if Start() hasn't run yet (e.g. first-frame deal).
            if (drawPile.Count == 0 && discardPile.Count == 0 && allCards.Count > 0)
                Reset();

            if (drawPile.Count == 0)
                Reshuffle();

            if (drawPile.Count == 0)
            {
                Debug.LogWarning("Deck is completely empty!");
                return null;
            }

            CardData card = drawPile[^1];
            drawPile.RemoveAt(drawPile.Count - 1);
            return card;
        }

        /// <summary>Draw <paramref name="count"/> cards and return them as a list.</summary>
        public List<CardData> DrawMultiple(int count)
        {
            var hand = new List<CardData>(count);
            for (int i = 0; i < count; i++)
            {
                var c = Draw();
                if (c != null) hand.Add(c);
            }
            return hand;
        }

        /// <summary>Move a card to the discard pile.</summary>
        public void Discard(CardData card)
        {
            discardPile.Add(card);
        }

        private void Reshuffle()
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle();
        }

        private void Shuffle()
        {
            for (int i = drawPile.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (drawPile[i], drawPile[j]) = (drawPile[j], drawPile[i]);
            }
        }

        public int DrawPileCount => drawPile.Count;
        public int DiscardPileCount => discardPile.Count;
    }
}
