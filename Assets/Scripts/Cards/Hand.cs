using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BalatroStyle
{
    /// <summary>
    /// Manages the player's hand of Card GameObjects and their fan layout.
    /// </summary>
    public class Hand : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private float cardSpacing = 1.4f;
        [SerializeField] private float arcHeight = 0.4f;
        [SerializeField] private float maxArcAngle = 20f;
        [SerializeField] private float layoutAnimDuration = 0.25f;

        [Header("Deal")]
        [SerializeField] private float dealStagger = 0.07f;
        [SerializeField] private float dealFlipDelay = 0.15f;
        [SerializeField] private float dealFlipDuration = 0.32f;

        [Header("Prefab")]
        [SerializeField] private Card cardPrefab;

        private List<Card> cards = new();

        public IReadOnlyList<Card> Cards => cards;

        /// <summary>Number of cards currently in the selected state.</summary>
        public int SelectedCount => cards.Count(c => c.IsSelected);

        /// <summary>Instantiate a card for each CardData face-down, fan into arc, then reveal with stagger.</summary>
        public void DealCards(List<CardData> cardDatas)
        {
            var newCards = SpawnCardsFaceDown(cardDatas);
            ArrangeCards();
            StartCoroutine(StaggeredReveal(newCards));
        }

        /// <summary>
        /// Draw from the given deck until the hand holds <paramref name="handSize"/> cards,
        /// then fan and reveal new arrivals with the same cascading stagger as DealCards.
        /// </summary>
        public void RefillTo(Deck deck, int handSize)
        {
            if (deck == null) return;

            int needed = handSize - cards.Count;
            if (needed <= 0) return;

            var drawn = deck.DrawMultiple(needed);
            if (drawn.Count == 0) return;

            var newCards = SpawnCardsFaceDown(drawn);
            ArrangeCards();
            StartCoroutine(StaggeredReveal(newCards));
        }

        private List<Card> SpawnCardsFaceDown(List<CardData> cardDatas)
        {
            var newCards = new List<Card>(cardDatas.Count);
            foreach (var data in cardDatas)
            {
                var card = Instantiate(cardPrefab, transform);
                card.SetFaceUpInstant(false);
                card.Initialize(data);
                cards.Add(card);
                newCards.Add(card);
            }
            return newCards;
        }

        private IEnumerator StaggeredReveal(List<Card> dealtCards)
        {
            yield return new WaitForSeconds(dealFlipDelay);
            foreach (var card in dealtCards)
            {
                StartCoroutine(card.Flip(true, dealFlipDuration));
                yield return new WaitForSeconds(dealStagger);
            }
        }

        /// <summary>Remove selected cards and return their data for discard.</summary>
        public List<CardData> RemoveSelected()
        {
            var selected = cards.Where(c => c.IsSelected).ToList();
            var datas = selected.Select(c => c.Data).ToList();
            foreach (var card in selected)
            {
                cards.Remove(card);
                Destroy(card.gameObject);
            }
            ArrangeCards();
            return datas;
        }

        /// <summary>Return the list of currently selected Card instances (read-only snapshot).</summary>
        public List<Card> GetSelected() => cards.Where(c => c.IsSelected).ToList();

        /// <summary>Return the CardData for currently selected cards, without removing them.</summary>
        public List<CardData> GetSelectedData() => cards.Where(c => c.IsSelected).Select(c => c.Data).ToList();

        /// <summary>Destroy all card GameObjects and clear the hand list.</summary>
        public void ClearHand()
        {
            foreach (var card in cards)
                Destroy(card.gameObject);
            cards.Clear();
        }

        private void ArrangeCards()
        {
            int count = cards.Count;
            if (count == 0) return;

            float totalWidth = (count - 1) * cardSpacing;

            for (int i = 0; i < count; i++)
            {
                float t = count > 1 ? (float)i / (count - 1) : 0.5f;
                float x = -totalWidth / 2f + i * cardSpacing;
                float y = -arcHeight * Mathf.Abs(t - 0.5f) * 2f;
                float z = (t - 0.5f) * -maxArcAngle;

                var target = new Vector3(x, y, 0f);
                var rot = Quaternion.Euler(0f, 0f, z);

                // Animate to target position
                StartCoroutine(cards[i].MoveToPosition(target, rot, layoutAnimDuration));

                // Sort order: higher index = rendered on top
                cards[i].SetSortOrder(i);
            }
        }
    }
}
