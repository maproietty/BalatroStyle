using UnityEngine;
using System;

namespace BalatroStyle
{
    /// <summary>
    /// Tracks chips, multiplier, and total score. Fires one event per scoring action
    /// so downstream systems (HUD rolling counter, camera bloom pulse, etc.) can drive
    /// their own animations without ScoreManager owning display timing.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [Header("Magnitude")]
        [SerializeField] private float maxScoreForMagnitude = 5000f;

        public int TotalScore { get; private set; }
        public int CurrentChips { get; private set; }
        public int CurrentMultiplier { get; private set; } = 1;

        /// <summary>Fires whenever chips/multiplier change. (chips, multiplier)</summary>
        public static event Action<int, int> OnScoreChanged;

        /// <summary>Fires once per ApplyScore. (delta, magnitude 0–1) — drives juice.</summary>
        public static event Action<int, float> OnScoreRolled;

        /// <summary>Fires when score state is reset to zero (e.g. new game).</summary>
        public static event Action OnScoreReset;

        /// <summary>Reset all score state to zero.</summary>
        public void ResetScore()
        {
            TotalScore = 0;
            CurrentChips = 0;
            CurrentMultiplier = 1;
            OnScoreChanged?.Invoke(CurrentChips, CurrentMultiplier);
            OnScoreReset?.Invoke();
        }

        /// <summary>Apply chips × multiplier to TotalScore and notify listeners once.</summary>
        public void ApplyScore(int chips, int multiplier)
        {
            CurrentChips = chips;
            CurrentMultiplier = multiplier;
            int delta = chips * multiplier;
            TotalScore += delta;

            float magnitude = Mathf.Clamp01(delta / maxScoreForMagnitude);
            OnScoreChanged?.Invoke(chips, multiplier);
            OnScoreRolled?.Invoke(delta, magnitude);
        }
    }
}
