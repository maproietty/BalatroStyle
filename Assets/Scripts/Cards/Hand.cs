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

        /// <summary>Instantiate a card for each CardData face-down, fan into arc, then reveal with stagger.</summary>
        public void DealCards(List<CardData> cardDatas)
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
            ArrangeCards();
            StartCoroutine(StaggeredReveal(newCards));
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

        /// <summary>Return selected card data without destroying (for play).</summary>
        public List<Card> GetSelected() => cards.Where(c => c.IsSelected).ToList();

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
