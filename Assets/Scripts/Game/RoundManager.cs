using UnityEngine;
using System;

namespace BalatroStyle
{
    /// <summary>
    /// Drives ante/round progression. Holds blind targets and escalates each round.
    /// </summary>
    public class RoundManager : MonoBehaviour
    {
        public static RoundManager Instance { get; private set; }

        [Header("Blind Targets")]
        [SerializeField] private int[] smallBlindTargets = { 300, 800, 2000, 5000, 11000, 20000, 35000, 50000 };
        [SerializeField] private int[] bigBlindTargets   = { 450, 1200, 3000, 7500, 16500, 30000, 52500, 75000 };
        [SerializeField] private int[] bossBlindTargets  = { 600, 1600, 4000, 10000, 22000, 40000, 70000, 100000 };

        public int CurrentAnte { get; private set; } = 1;
        public int CurrentBlindIndex { get; private set; } = 0; // 0=small, 1=big, 2=boss
        public int CurrentBlindTarget => GetBlindTarget(CurrentAnte - 1, CurrentBlindIndex);

        public static event Action<int, int, int> OnBlindChanged; // ante, blindIndex, target

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            GameManager.OnRoundEnded += HandleRoundEnd;
        }

        private void OnDisable()
        {
            GameManager.OnRoundEnded -= HandleRoundEnd;
        }

        private void HandleRoundEnd()
        {
            bool scoremet = ScoreManager.Instance != null &&
                            ScoreManager.Instance.TotalScore >= CurrentBlindTarget;

            if (!scoremet)
            {
                GameManager.Instance?.TriggerGameOver();
                return;
            }

            AdvanceBlind();
        }

        private void AdvanceBlind()
        {
            CurrentBlindIndex++;
            if (CurrentBlindIndex > 2)
            {
                CurrentBlindIndex = 0;
                CurrentAnte++;
            }

            if (CurrentAnte - 1 >= smallBlindTargets.Length)
            {
                // Player won all antes — handle win condition
                Debug.Log("All antes cleared — you win!");
                return;
            }

            OnBlindChanged?.Invoke(CurrentAnte, CurrentBlindIndex, CurrentBlindTarget);
            GameManager.Instance?.StartRound();
        }

        private int GetBlindTarget(int anteIndex, int blindIndex)
        {
            if (anteIndex < 0 || anteIndex >= smallBlindTargets.Length) return int.MaxValue;
            return blindIndex switch
            {
                0 => smallBlindTargets[anteIndex],
                1 => bigBlindTargets[anteIndex],
                2 => bossBlindTargets[anteIndex],
                _ => int.MaxValue
            };
        }
    }
}
