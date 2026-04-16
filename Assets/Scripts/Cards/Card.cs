using UnityEngine;
using System;
using System.Collections;

namespace BalatroStyle
{
    /// <summary>
    /// MonoBehaviour on a Card prefab. Handles visual state, hover wobble, selection, flip.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Card : MonoBehaviour
    {
        /// <summary>Fires after a face-up flip completes. Listeners get the revealed card.</summary>
        public static event Action<Card> OnCardRevealed;

        private const float FlipPopOvershoot = 1.08f;
        private const float FlipPopDuration = 0.08f;
        private const float RevealGlowDuration = 0.18f;

        [Header("Hover Settings")]
        [SerializeField] private float hoverScaleUp = 1.12f;
        [SerializeField] private float hoverLiftY = 0.35f;
        [SerializeField] private float hoverTiltMax = 8f;
        [SerializeField] private float springStiffness = 300f;
        [SerializeField] private float springDamping = 20f;
        [SerializeField] private float hoverAnimDuration = 0.12f;
        [SerializeField] private float tiltCursorMultiplier = 10f;

        [Header("Selection")]
        [SerializeField] private float selectedLiftY = 0.2f;

        [Header("Flip")]
        [SerializeField] private float defaultFlipDuration = 0.32f;

        [Header("References")]
        [SerializeField] private SpriteRenderer frontRenderer;
        [SerializeField] private SpriteRenderer backRenderer;
        [SerializeField] private SpriteRenderer glowRenderer;

        public CardData Data { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsFaceUp { get; private set; } = true;

        private Camera cam;

        // Spring physics for hover wobble
        private Vector3 basePosition;
        private Quaternion baseRotation;
        private float tiltVelocity;
        private float currentTilt;
        private bool isHovered;
        private bool isAnimating;

        private void Awake()
        {
            cam = Camera.main;
        }

        /// <summary>Assign CardData and update front sprite to match.</summary>
        public void Initialize(CardData data)
        {
            Data = data;
            if (frontRenderer != null && data.frontSprite != null)
                frontRenderer.sprite = data.frontSprite;
            ApplyFaceState();
        }

        /// <summary>Set face-up state instantly without animation. Useful at deal-time.</summary>
        public void SetFaceUpInstant(bool faceUp)
        {
            IsFaceUp = faceUp;
            ApplyFaceState();
        }

        private void ApplyFaceState()
        {
            if (frontRenderer != null) frontRenderer.enabled = IsFaceUp;
            if (backRenderer  != null) backRenderer.enabled  = !IsFaceUp;
        }

        private void Update()
        {
            if (isAnimating) return;
            UpdateHoverWobble();
        }

        private void UpdateHoverWobble()
        {
            if (!isHovered) return;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector3 delta = mouseWorld - transform.position;

            // Spring tilt toward cursor
            float targetTilt = Mathf.Clamp(-delta.x * tiltCursorMultiplier, -hoverTiltMax, hoverTiltMax);
            float tiltAccel = (targetTilt - currentTilt) * springStiffness * Time.deltaTime
                              - tiltVelocity * springDamping * Time.deltaTime;
            tiltVelocity += tiltAccel;
            currentTilt += tiltVelocity * Time.deltaTime;

            transform.rotation = baseRotation * Quaternion.Euler(0f, 0f, currentTilt);
        }

        private void OnMouseEnter()
        {
            if (isAnimating) return;
            isHovered = true;
            StopAllCoroutines();
            StartCoroutine(AnimateHover(true));
        }

        private void OnMouseExit()
        {
            isHovered = false;
            tiltVelocity = 0f;
            currentTilt = 0f;
            StopAllCoroutines();
            StartCoroutine(AnimateHover(false));
        }

        private void OnMouseDown()
        {
            ToggleSelected();
        }

        /// <summary>Toggle selected state, activate glow, and lift/lower card accordingly.</summary>
        public void ToggleSelected()
        {
            IsSelected = !IsSelected;
            SetGlowActive(IsSelected);
            basePosition += IsSelected ? Vector3.up * selectedLiftY : Vector3.down * selectedLiftY;
        }

        private void SetGlowActive(bool active)
        {
            if (glowRenderer != null)
                glowRenderer.gameObject.SetActive(active);
        }

        /// <summary>Set the sorting order on all renderers so hand overlap is correct.</summary>
        public void SetSortOrder(int order)
        {
            if (frontRenderer) frontRenderer.sortingOrder = order;
            if (backRenderer)  backRenderer.sortingOrder  = order;
            if (glowRenderer)  glowRenderer.sortingOrder  = order - 1;
        }

        /// <summary>Coroutine for the Hand to smoothly reposition cards on layout.</summary>
        public IEnumerator MoveToPosition(Vector3 targetPos, Quaternion targetRot, float duration)
        {
            isAnimating = true;
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            transform.localPosition = targetPos;
            transform.localRotation = targetRot;
            basePosition = targetPos;
            baseRotation = targetRot;
            isAnimating = false;
        }

        /// <summary>Flip the card to the requested face over the given duration.</summary>
        public IEnumerator Flip(bool faceUp, float duration)
        {
            if (IsFaceUp == faceUp) yield break;

            isAnimating = true;

            float half = duration * 0.5f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 flatScale = new Vector3(0f, startScale.y, startScale.z);

            // First half: scale X to 0
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / half);
                transform.localScale = Vector3.Lerp(startScale, flatScale, t);
                yield return null;
            }

            // Mid-flip: swap which face renders
            IsFaceUp = faceUp;
            ApplyFaceState();

            // Second half: scale X back out
            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / half);
                transform.localScale = Vector3.Lerp(flatScale, startScale, t);
                yield return null;
            }

            transform.localScale = startScale;
            isAnimating = false;

            if (faceUp)
            {
                StartCoroutine(FlipPop(startScale));
                StartCoroutine(RevealGlowPulse());
                OnCardRevealed?.Invoke(this);
            }
        }

        /// <summary>Convenience: flip to face up using the inspector-default duration.</summary>
        public IEnumerator FlipToFaceUp() => Flip(true, defaultFlipDuration);

        /// <summary>Convenience: flip to face down using the inspector-default duration.</summary>
        public IEnumerator FlipToFaceDown() => Flip(false, defaultFlipDuration);

        private IEnumerator FlipPop(Vector3 baseScale)
        {
            float elapsed = 0f;
            Vector3 peakScale = baseScale * FlipPopOvershoot;

            while (elapsed < FlipPopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / FlipPopDuration);
                transform.localScale = Vector3.Lerp(baseScale, peakScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < FlipPopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / FlipPopDuration);
                transform.localScale = Vector3.Lerp(peakScale, baseScale, t);
                yield return null;
            }

            transform.localScale = baseScale;
        }

        private IEnumerator RevealGlowPulse()
        {
            if (glowRenderer == null || IsSelected) yield break;

            glowRenderer.gameObject.SetActive(true);
            float elapsed = 0f;
            Color baseColor = glowRenderer.color;
            Color transparent = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

            while (elapsed < RevealGlowDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / RevealGlowDuration);
                glowRenderer.color = Color.Lerp(baseColor, transparent, t);
                yield return null;
            }

            glowRenderer.color = baseColor;
            glowRenderer.gameObject.SetActive(false);
        }

        private IEnumerator AnimateHover(bool hoveringIn)
        {
            float duration = hoverAnimDuration;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 endScale = hoveringIn ? Vector3.one * hoverScaleUp : Vector3.one;
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = hoveringIn
                ? basePosition + Vector3.up * hoverLiftY
                : basePosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.localScale = Vector3.Lerp(startScale, endScale, t);
                transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            transform.localScale = endScale;
            transform.localPosition = endPos;
        }
    }
}
