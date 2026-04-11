using UnityEngine;
using System;
using System.Collections;

namespace BalatroStyle
{
    /// <summary>
    /// Tracks chips and multiplier, fires score events, drives rolling-number display.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Scoring")]
        [SerializeField] private float rollDuration = 0.8f;

        public int TotalScore { get; private set; }
        public int CurrentChips { get; private set; }
        public int CurrentMultiplier { get; private set; } = 1;

        // Fired with the new displayed score so HUD can update
        public static event Action<int, int> OnScoreChanged;     // chips, multiplier
        public static event Action<int, float> OnScoreRolled;    // totalDelta, magnitude (0-1)

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void ResetScore()
        {
            TotalScore = 0;
            CurrentChips = 0;
            CurrentMultiplier = 1;
            OnScoreChanged?.Invoke(CurrentChips, CurrentMultiplier);
        }

        /// <summary>Apply chips × multiplier to TotalScore and trigger roll animation.</summary>
        public void ApplyScore(int chips, int multiplier)
        {
            CurrentChips = chips;
            CurrentMultiplier = multiplier;
            int delta = chips * multiplier;
            OnScoreChanged?.Invoke(chips, multiplier);

            float magnitude = Mathf.Clamp01(delta / 5000f);
            OnScoreRolled?.Invoke(delta, magnitude);

            StartCoroutine(RollScoreCoroutine(TotalScore, TotalScore + delta));
            TotalScore += delta;
        }

        private IEnumerator RollScoreCoroutine(int from, int to)
        {
            float elapsed = 0f;
            while (elapsed < rollDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / rollDuration);
                int displayed = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                // HUD subscribes to this event for the rolling counter
                OnScoreRolled?.Invoke(displayed - from, t);
                yield return null;
            }
        }
    }
}
