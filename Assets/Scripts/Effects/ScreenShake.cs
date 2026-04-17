using UnityEngine;
using System;
using System.Collections;

namespace BalatroStyle
{
    /// <summary>
    /// Attach to the main Camera. Subscribe to ScoreManager events to auto-shake on big scores.
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        /// <summary>Static channel for any system to request a shake without holding a reference.</summary>
        public static event Action<float, float> OnShakeRequested;

        [SerializeField] private float baseAmplitude = 0.15f;
        [SerializeField] private float baseDuration = 0.35f;

        private Vector3 originPosition;
        private Coroutine shakeCoroutine;

        private void OnEnable()
        {
            ScoreManager.OnScoreRolled += HandleScoreRolled;
            OnShakeRequested += HandleShakeRequested;
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreRolled -= HandleScoreRolled;
            OnShakeRequested -= HandleShakeRequested;
        }

        /// <summary>Invoke OnShakeRequested so any camera-attached ScreenShake handles it.</summary>
        public static void Request(float amplitude, float duration) =>
            OnShakeRequested?.Invoke(amplitude, duration);

        private void HandleShakeRequested(float amplitude, float duration) =>
            Shake(amplitude, duration);

        private void Start()
        {
            originPosition = transform.localPosition;
        }

        private void HandleScoreRolled(int delta, float magnitude)
        {
            if (magnitude < 0.1f) return;
            Shake(baseAmplitude * magnitude, baseDuration * (0.5f + magnitude * 0.5f));
        }

        /// <summary>Trigger a shake with explicit amplitude and duration (0–1 magnitude).</summary>
        public void Shake(float amplitude, float duration)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeCoroutine(amplitude, duration));
        }

        private IEnumerator ShakeCoroutine(float amplitude, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float remaining = 1f - (elapsed / duration);
                float x = UnityEngine.Random.Range(-1f, 1f) * amplitude * remaining;
                float y = UnityEngine.Random.Range(-1f, 1f) * amplitude * remaining;
                transform.localPosition = originPosition + new Vector3(x, y, 0f);
                yield return null;
            }
            transform.localPosition = originPosition;
        }
    }
}
