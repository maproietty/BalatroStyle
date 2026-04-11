using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BalatroStyle
{
    /// <summary>
    /// Drives URP Volume overrides at runtime (bloom pulse, vignette flash, chromatic aberration).
    /// Requires a Global Volume with a VolumeProfile containing Bloom, Vignette, and ChromaticAberration.
    /// </summary>
    [RequireComponent(typeof(Volume))]
    public class CameraEffects : MonoBehaviour
    {
        public static CameraEffects Instance { get; private set; }

        [Header("Bloom Pulse")]
        [SerializeField] private float bloomPulseIntensity = 3f;
        [SerializeField] private float bloomPulseDuration = 0.4f;

        [Header("Chromatic Aberration")]
        [SerializeField] private float aberrationOnScore = 0.6f;
        [SerializeField] private float aberrationDecaySpeed = 2f;

        private Volume vol;
        private Bloom bloom;
        private Vignette vignette;
        private ChromaticAberration chromatic;
        private float baseBloomIntensity;
        private float targetAberration;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            vol = GetComponent<Volume>();
            vol.profile.TryGet(out bloom);
            vol.profile.TryGet(out vignette);
            vol.profile.TryGet(out chromatic);

            if (bloom != null)
                baseBloomIntensity = bloom.intensity.value;
        }

        private void OnEnable()
        {
            ScoreManager.OnScoreRolled += HandleScore;
        }

        private void OnDisable()
        {
            ScoreManager.OnScoreRolled -= HandleScore;
        }

        private void Update()
        {
            // Decay chromatic aberration
            if (chromatic != null)
            {
                float current = chromatic.intensity.value;
                chromatic.intensity.value = Mathf.MoveTowards(current, 0f, aberrationDecaySpeed * Time.deltaTime);
            }
        }

        private void HandleScore(int delta, float magnitude)
        {
            if (magnitude < 0.05f) return;

            if (chromatic != null)
                chromatic.intensity.value = Mathf.Max(chromatic.intensity.value, aberrationOnScore * magnitude);

            if (bloom != null)
                StartCoroutine(BloomPulse(magnitude));
        }

        private System.Collections.IEnumerator BloomPulse(float magnitude)
        {
            float target = baseBloomIntensity + bloomPulseIntensity * magnitude;
            float elapsed = 0f;
            while (elapsed < bloomPulseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / bloomPulseDuration;
                bloom.intensity.value = Mathf.Lerp(target, baseBloomIntensity, t);
                yield return null;
            }
            bloom.intensity.value = baseBloomIntensity;
        }
    }
}
