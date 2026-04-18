using UnityEngine;
using System.Collections;
using TMPro;

namespace BalatroStyle
{
    /// <summary>
    /// Momentary banner that shows the played hand's name (e.g. "Full House")
    /// plus its chips × multiplier when <see cref="HandScorer.OnHandEvaluated"/>
    /// fires. Fades and scale-pops through reveal → hold → retract.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class HandNameBanner : MonoBehaviour
    {
        [Header("Labels")]
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text chipsMultLabel;

        [Header("Animation — Timings")]
        [SerializeField] private float revealDuration = 0.18f;
        [SerializeField] private float holdDuration = 0.9f;
        [SerializeField] private float retractDuration = 0.28f;

        [Header("Animation — Scale")]
        [SerializeField] private float startScale = 0.7f;
        [SerializeField] private float peakScale = 1.12f;
        [SerializeField] private float restScale = 1.0f;

        [Header("Animation — Retract Drift")]
        [Tooltip("Local Y offset the banner drifts upward during retract, in local units.")]
        [SerializeField] private float retractDrift = 12f;

        private CanvasGroup canvasGroup;
        private RectTransform rect;
        private Vector3 baseScale;
        private Vector2 baseAnchoredPosition;
        private Coroutine current;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rect = transform as RectTransform;
            baseScale = Vector3.one * restScale;
            if (rect != null) baseAnchoredPosition = rect.anchoredPosition;
            HideImmediate();
        }

        private void OnEnable()
        {
            HandScorer.OnHandEvaluated += HandleHandEvaluated;
        }

        private void OnDisable()
        {
            HandScorer.OnHandEvaluated -= HandleHandEvaluated;
        }

        private void HandleHandEvaluated(EvaluatedHand evaluated)
        {
            Show(evaluated);
        }

        /// <summary>Display the banner for a freshly-evaluated hand. Restarts if already showing.</summary>
        public void Show(EvaluatedHand evaluated)
        {
            if (nameLabel != null) nameLabel.text = evaluated.DisplayName;
            if (chipsMultLabel != null)
                chipsMultLabel.text = evaluated.TotalChips + " × " + evaluated.BaseMultiplier;

            if (current != null) StopCoroutine(current);
            current = StartCoroutine(ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            if (rect != null) rect.anchoredPosition = baseAnchoredPosition;
            transform.localScale = Vector3.one * startScale;
            if (canvasGroup != null) canvasGroup.alpha = 0f;

            // Reveal: fade in + overshoot scale pop.
            float elapsed = 0f;
            while (elapsed < revealDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / revealDuration);
                float scale = Mathf.Lerp(startScale, peakScale, t);
                if (canvasGroup != null) canvasGroup.alpha = t;
                transform.localScale = Vector3.one * scale;
                yield return null;
            }

            // Settle peak → rest (part of reveal for snappiness).
            float settleDuration = revealDuration * 0.6f;
            elapsed = 0f;
            while (elapsed < settleDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / settleDuration);
                float scale = Mathf.Lerp(peakScale, restScale, t);
                transform.localScale = Vector3.one * scale;
                yield return null;
            }
            transform.localScale = baseScale;
            if (canvasGroup != null) canvasGroup.alpha = 1f;

            // Hold.
            if (holdDuration > 0f)
                yield return new WaitForSeconds(holdDuration);

            // Retract: fade out + drift up.
            elapsed = 0f;
            while (elapsed < retractDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / retractDuration);
                if (canvasGroup != null) canvasGroup.alpha = 1f - t;
                if (rect != null)
                    rect.anchoredPosition = baseAnchoredPosition + new Vector2(0f, retractDrift * t);
                yield return null;
            }

            HideImmediate();
            current = null;
        }

        private void HideImmediate()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            transform.localScale = baseScale;
            if (rect != null) rect.anchoredPosition = baseAnchoredPosition;
        }
    }
}
