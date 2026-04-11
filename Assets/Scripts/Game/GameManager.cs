using UnityEngine;
using System;

namespace BalatroStyle
{
    /// <summary>
    /// Central singleton that owns game state and broadcasts events to other systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Round Settings")]
        [SerializeField] private int startingHandsPerRound = 4;
        [SerializeField] private int startingDiscardsPerRound = 3;

        public int HandsRemaining { get; private set; }
        public int DiscardsRemaining { get; private set; }
        public GameState CurrentState { get; private set; } = GameState.Menu;

        // --- Events ---
        public static event Action<GameState> OnStateChanged;
        public static event Action OnRoundStarted;
        public static event Action OnRoundEnded;
        public static event Action OnGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ChangeState(GameState.Menu);
        }

        public void StartNewGame()
        {
            ScoreManager.Instance?.ResetScore();
            StartRound();
        }

        public void StartRound()
        {
            HandsRemaining = startingHandsPerRound;
            DiscardsRemaining = startingDiscardsPerRound;
            ChangeState(GameState.Playing);
            OnRoundStarted?.Invoke();
        }

        public void UseHand()
        {
            HandsRemaining = Mathf.Max(0, HandsRemaining - 1);
            if (HandsRemaining <= 0)
                EndRound();
        }

        public void UseDiscard()
        {
            DiscardsRemaining = Mathf.Max(0, DiscardsRemaining - 1);
        }

        private void EndRound()
        {
            ChangeState(GameState.RoundEnd);
            OnRoundEnded?.Invoke();
        }

        public void TriggerGameOver()
        {
            ChangeState(GameState.GameOver);
            OnGameOver?.Invoke();
        }

        private void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }

    public enum GameState
    {
        Menu,
        Playing,
        RoundEnd,
        GameOver
    }
}
