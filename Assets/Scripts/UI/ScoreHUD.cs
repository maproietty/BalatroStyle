using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace BalatroStyle
{
    /// <summary>
    /// Heads-up display for score, chips × multiplier, hands remaining, discards
    /// remaining, and selection count. Subscribes to scoring and hand-action events
    /// and drives a rolling-number animation on the total score. No direct refs to
    /// other systems beyond the Inspector-wired ScoreManager (consistent with how
    /// GameManager / RoundManager hold ScoreManager refs).
    /// </summary>
    public class ScoreHUD : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private ScoreManager scoreManager;

        [Header("Labels")]
        [SerializeField] private TMP_Text totalScoreLabel;
        [SerializeField] private TMP_Text chipsMultLabel;
        [SerializeField] private TMP_Text handsRemainingLabel;
        [SerializeField] private TMP_Text discardsRemainingLabel;
        [SerializeField] private TMP_Text selectionCountLabel;

        [Header("Rolling Counter")]
        [SerializeField] private float rollDuration = 0.7f;

        [Header("Pulse — Score")]
        [SerializeField] private Transform scorePulseTarget;
        [SerializeField] private float scorePulseScale = 1.18f;
        [SerializeField] private float scorePulseDuration = 0.22f;

        [Header("Flash — Score (on scoring)")]
        [SerializeField] private Color scoreRestColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color scoreFlashColor = new Color(0.941f, 0.753f, 0.251f, 1f); // #f0c040 gold
        [SerializeField] private float scoreFlashDuration = 0.45f;

        [Header("Pulse — Counters")]
        [SerializeField] private Transform handsPulseTarget;
        [SerializeField] private Transform discardsPulseTarget;
        [SerializeField] private float counterPulseScale = 1.12f;
        [SerializeField] private float counterPulseDuration = 0.18f;

        [Header("Selection Display")]
        [SerializeField] private int maxSelected = 5;

        private int displayedScore;
        private Coroutine rollRoutine;
        private Coroutine scorePulseRoutine;
        private Coroutine scoreFlashRoutine;
        private Coroutine handsPulseRoutine;
        private Coroutine discardsPulseRoutine;

        private void OnEnable()
        {
            ScoreManager.OnScoreChanged += HandleScoreChanged;
            HandScorer.OnHandEvaluated += HandleHandEvaluated;
            HandActionController.OnHandPlayed += HandleHandPlayed;
            HandActionController.OnHandDiscarded += HandleHandDiscarded;
            HandActionController.OnSelectionCountChanged += HandleSelectionCountChanged;
            GameManager.OnRoundStarted += HandleRoundStarted;
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreChanged -= HandleScoreChanged;
            HandScorer.OnHandEvaluated -= HandleHandEvaluated;
            HandActionController.OnHandPlayed -= HandleHandPlayed;
            HandActionController.OnHandDiscarded -= HandleHandDiscarded;
            HandActionController.OnSelectionCountChanged -= HandleSelectionCountChanged;
            GameManager.OnRoundStarted -= HandleRoundStarted;
        }

        private void Start()
        {
            RefreshAll();
        }

        private void HandleScoreChanged(int chips, int multiplier)
        {
            SetChipsMult(chips, multiplier);

            // Reset case: ScoreManager.ResetScore fires (0, 1) — snap the display.
            if (chips == 0 && multiplier == 1 && scoreManager != null && scoreManager.TotalScore == 0)
            {
                SnapScore(0);
            }
        }

        private void HandleHandEvaluated(EvaluatedHand evaluated)
        {
            int cardChips = 0;
            foreach (var card in evaluated.ScoringCards)
                cardChips += card.ChipValue;

            int delta = (evaluated.BaseChips + cardChips) * evaluated.BaseMultiplier;
            RollScoreBy(delta);
        }

        private void HandleHandPlayed(List<CardData> _)
        {
            RefreshCounters();
            PulseCounter(handsPulseTarget, ref handsPulseRoutine);
        }

        private void HandleHandDiscarded(List<CardData> _)
        {
            RefreshCounters();
            PulseCounter(discardsPulseTarget, ref discardsPulseRoutine);
        }

        private void HandleSelectionCountChanged(int count)
        {
            SetSelectionCount(count);
        }

        private void HandleRoundStarted()
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            if (scoreManager != null)
            {
                SnapScore(scoreManager.TotalScore);
                SetChipsMult(scoreManager.CurrentChips, scoreManager.CurrentMultiplier);
            }
            RefreshCounters();
            SetSelectionCount(0);
        }

        private void RefreshCounters()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;
            if (handsRemainingLabel != null) handsRemainingLabel.text = gm.HandsRemaining.ToString();
            if (discardsRemainingLabel != null) discardsRemainingLabel.text = gm.DiscardsRemaining.ToString();
        }

        private void SetChipsMult(int chips, int multiplier)
        {
            if (chipsMultLabel != null)
                chipsMultLabel.text = chips + " × " + multiplier;
        }

        private void SetSelectionCount(int count)
        {
            if (selectionCountLabel != null)
                selectionCountLabel.text = count + " / " + maxSelected;
        }

        private void SnapScore(int value)
        {
            if (rollRoutine != null) StopCoroutine(rollRoutine);
            displayedScore = value;
            WriteScoreLabel(displayedScore);
        }

        private void RollScoreBy(int delta)
        {
            if (delta == 0) return;
            int from = displayedScore;
            int to = displayedScore + delta;
            displayedScore = to;

            if (rollRoutine != null) StopCoroutine(rollRoutine);
            rollRoutine = StartCoroutine(RollScoreCoroutine(from, to));

            PulseScore();
            FlashScore();
        }

        private IEnumerator RollScoreCoroutine(int from, int to)
        {
            float elapsed = 0f;
            while (elapsed < rollDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / rollDuration);
                int v = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                WriteScoreLabel(v);
                yield return null;
            }
            WriteScoreLabel(to);
            rollRoutine = null;
        }

        private void WriteScoreLabel(int value)
        {
            if (totalScoreLabel != null)
                totalScoreLabel.text = value.ToString();
        }

        private void PulseScore()
        {
            if (scorePulseTarget == null) return;
            if (scorePulseRoutine != null) StopCoroutine(scorePulseRoutine);
            scorePulseRoutine = StartCoroutine(PulseCoroutine(scorePulseTarget, scorePulseScale, scorePulseDuration));
        }

        private void FlashScore()
        {
            if (totalScoreLabel == null) return;
            if (scoreFlashRoutine != null) StopCoroutine(scoreFlashRoutine);
            scoreFlashRoutine = StartCoroutine(FlashScoreCoroutine());
        }

        private IEnumerator FlashScoreCoroutine()
        {
            totalScoreLabel.color = scoreFlashColor;
            float elapsed = 0f;
            while (elapsed < scoreFlashDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / scoreFlashDuration);
                totalScoreLabel.color = Color.Lerp(scoreFlashColor, scoreRestColor, t);
                yield return null;
            }
            totalScoreLabel.color = scoreRestColor;
            scoreFlashRoutine = null;
        }

        private void PulseCounter(Transform target, ref Coroutine routine)
        {
            if (target == null) return;
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(PulseCoroutine(target, counterPulseScale, counterPulseDuration));
        }

        private static IEnumerator PulseCoroutine(Transform target, float peakScale, float duration)
        {
            Vector3 baseScale = Vector3.one;
            float half = duration * 0.5f;
            float elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / half);
                target.localScale = Vector3.Lerp(baseScale, baseScale * peakScale, t);
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / half);
                target.localScale = Vector3.Lerp(baseScale * peakScale, baseScale, t);
                yield return null;
            }
            target.localScale = baseScale;
        }
    }
}
