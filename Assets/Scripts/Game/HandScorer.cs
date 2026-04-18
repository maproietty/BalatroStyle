using UnityEngine;
using System;
using System.Collections.Generic;

namespace BalatroStyle
{
    /// <summary>
    /// Bridges the play pipeline to scoring. Subscribes to
    /// <see cref="HandActionController.OnHandPlayed"/>, evaluates the hand via
    /// <see cref="PokerHandEvaluator"/>, applies the resulting chips × multiplier
    /// to <see cref="ScoreManager"/>, fires <see cref="OnHandEvaluated"/> for
    /// downstream UI/juice, and requests a hand-strength-scaled screen shake.
    /// </summary>
    public class HandScorer : MonoBehaviour
    {
        /// <summary>Fires after a played hand is classified and scored.</summary>
        public static event Action<EvaluatedHand> OnHandEvaluated;

        [Header("Dependencies")]
        [SerializeField] private ScoreManager scoreManager;

        [Header("Juice — Screen Shake")]
        [SerializeField] private float minShakeAmplitude = 0.05f;
        [SerializeField] private float maxShakeAmplitude = 0.35f;
        [SerializeField] private float shakeDuration = 0.32f;

        private const int StrongestHandValue = (int)HandType.RoyalFlush;

        private void OnEnable()
        {
            HandActionController.OnHandPlayed += HandleHandPlayed;
        }

        private void OnDisable()
        {
            HandActionController.OnHandPlayed -= HandleHandPlayed;
        }

        private void HandleHandPlayed(List<CardData> playedCards)
        {
            if (scoreManager == null || playedCards == null || playedCards.Count == 0)
                return;

            EvaluatedHand evaluated = PokerHandEvaluator.Evaluate(playedCards);

            scoreManager.ApplyScore(evaluated.TotalChips, evaluated.BaseMultiplier);

            RequestHandStrengthShake(evaluated.Type);

            OnHandEvaluated?.Invoke(evaluated);
        }

        private void RequestHandStrengthShake(HandType type)
        {
            float rawT = (float)(int)type / StrongestHandValue;
            float eased = Mathf.SmoothStep(0f, 1f, rawT);
            float amplitude = Mathf.Lerp(minShakeAmplitude, maxShakeAmplitude, eased);
            ScreenShake.Request(amplitude, shakeDuration);
        }
    }
}
