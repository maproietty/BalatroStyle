using UnityEngine;
using System;
using System.Collections.Generic;

namespace BalatroStyle
{
    /// <summary>
    /// Coordinates card selection, play, and discard actions.
    /// Gates Card.OnSelectToggleRequested with a selection cap, consumes hand/discard
    /// counts on GameManager, and refills the Hand from the Deck after each action.
    /// </summary>
    public class HandActionController : MonoBehaviour
    {
        /// <summary>Fires after a hand is played. Listeners get the played cards' data.</summary>
        public static event Action<List<CardData>> OnHandPlayed;

        /// <summary>Fires after a hand is discarded. Listeners get the discarded cards' data.</summary>
        public static event Action<List<CardData>> OnHandDiscarded;

        /// <summary>Fires when the selection count changes. Listeners get the new count.</summary>
        public static event Action<int> OnSelectionCountChanged;

        /// <summary>Fires when a selection request is rejected (cap reached). Listeners can add juice.</summary>
        public static event Action<Card> OnSelectionRejected;

        [Header("Dependencies")]
        [SerializeField] private Hand hand;
        [SerializeField] private Deck deck;

        [Header("Rules")]
        [SerializeField] private int maxSelected = 5;
        [SerializeField] private int handSize = 8;

        [Header("Hotkeys")]
        [SerializeField] private KeyCode playKey = KeyCode.Space;
        [SerializeField] private KeyCode discardKey = KeyCode.D;

        [Header("Juice")]
        [SerializeField] private float playShakeAmplitude = 0.08f;
        [SerializeField] private float playShakeDuration = 0.18f;

        private void OnEnable()
        {
            Card.OnSelectToggleRequested += HandleSelectToggleRequested;
            Card.OnSelectionChanged += HandleSelectionChanged;
            GameManager.OnRoundStarted += HandleRoundStarted;
        }

        private void OnDisable()
        {
            Card.OnSelectToggleRequested -= HandleSelectToggleRequested;
            Card.OnSelectionChanged -= HandleSelectionChanged;
            GameManager.OnRoundStarted -= HandleRoundStarted;
        }

        private void HandleRoundStarted()
        {
            if (hand == null || deck == null) return;
            hand.RefillTo(deck, handSize);
        }

        private void Update()
        {
            if (!IsPlayingState()) return;

            if (Input.GetKeyDown(playKey))
                PlaySelected();
            else if (Input.GetKeyDown(discardKey))
                DiscardSelected();
        }

        private void HandleSelectToggleRequested(Card card)
        {
            if (card == null || hand == null) return;

            // Always allow deselecting.
            if (card.IsSelected)
            {
                card.TrySetSelected(false);
                return;
            }

            // Cap-aware selection.
            if (hand.SelectedCount >= maxSelected)
            {
                OnSelectionRejected?.Invoke(card);
                StartCoroutine(card.BounceReject());
                return;
            }

            card.TrySetSelected(true);
        }

        private void HandleSelectionChanged(Card card, bool isSelected)
        {
            if (hand == null) return;
            OnSelectionCountChanged?.Invoke(hand.SelectedCount);
        }

        /// <summary>
        /// Play the currently selected cards: consume a hand on GameManager, remove
        /// them from the Hand, fire OnHandPlayed, and refill from the Deck.
        /// </summary>
        public void PlaySelected()
        {
            if (hand == null || hand.SelectedCount == 0) return;

            var gm = GameManager.Instance;
            if (gm == null || gm.HandsRemaining <= 0) return;

            var playedData = hand.RemoveSelected();
            gm.UseHand();
            ScreenShake.Request(playShakeAmplitude, playShakeDuration);
            OnHandPlayed?.Invoke(playedData);
            OnSelectionCountChanged?.Invoke(hand.SelectedCount);

            hand.RefillTo(deck, handSize);
        }

        /// <summary>
        /// Discard the currently selected cards: consume a discard on GameManager,
        /// remove them from the Hand, push to the Deck discard pile, fire OnHandDiscarded,
        /// and refill from the Deck.
        /// </summary>
        public void DiscardSelected()
        {
            if (hand == null || hand.SelectedCount == 0) return;

            var gm = GameManager.Instance;
            if (gm == null || gm.DiscardsRemaining <= 0) return;

            var discardedData = hand.RemoveSelected();
            gm.UseDiscard();

            if (deck != null)
            {
                foreach (var data in discardedData)
                    deck.Discard(data);
            }

            OnHandDiscarded?.Invoke(discardedData);
            OnSelectionCountChanged?.Invoke(hand.SelectedCount);

            hand.RefillTo(deck, handSize);
        }

        private bool IsPlayingState()
        {
            var gm = GameManager.Instance;
            return gm != null && gm.CurrentState == GameState.Playing;
        }
    }
}
