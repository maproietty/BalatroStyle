using UnityEngine;
using System;
using System.Collections;

namespace BalatroStyle
{
    /// <summary>
    /// MonoBehaviour on a Card prefab. Handles visual state, hover wobble, selection, flip.
    /// </summary>
    public class Card : MonoBehaviour
    {
        /// <summary>Fires after a face-up flip completes. Listeners get the revealed card.</summary>
        public static event Action<Card> OnCardRevealed;

        /// <summary>Fires when a click requests a selection toggle. A coordinator (e.g. HandActionController) decides whether to grant it.</summary>
        public static event Action<Card> OnSelectToggleRequested;

        /// <summary>Fires after a selection change is accepted. Listeners get the card and its new selected state.</summary>
        public static event Action<Card, bool> OnSelectionChanged;

        private const float FlipPopOvershoot = 1.08f;
        private const float FlipPopDuration = 0.08f;
        private const float RevealGlowDuration = 0.18f;
        private const float SelectPopOvershoot = 1.10f;
        private const float SelectPopDuration = 0.09f;
        private const float BounceRecoilDistance = 0.18f;
        private const float BounceRecoilDuration = 0.07f;
        private const int SortOrderOffset = 10;
        private const int SortOrderStride = 3;

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

        // Layout rest state — basePosition is the raw layout target; selectionOffset is
        // composed on top so a selection survives re-layouts (ArrangeCards -> MoveToPosition).
        private Vector3 basePosition;
        private Vector3 selectionOffset;
        private Quaternion baseRotation;

        // Spring physics for hover wobble (Z = lean left/right, X = lean forward/back).
        private float tiltZVelocity;
        private float currentTiltZ;
        private float tiltXVelocity;
        private float currentTiltX;

        private bool isHovered;
        private bool isAnimating;
        private Coroutine hoverRoutine;

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
            if (!isHovered || cam == null) return;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector3 delta = mouseWorld - transform.position;

            // Z lean follows cursor X (tilts toward cursor horizontally).
            float targetTiltZ = Mathf.Clamp(-delta.x * tiltCursorMultiplier, -hoverTiltMax, hoverTiltMax);
            currentTiltZ = IntegrateSpring(currentTiltZ, targetTiltZ, ref tiltZVelocity);

            // X lean follows cursor Y (tilts card forward/back — subtle shear in ortho 2D).
            float targetTiltX = Mathf.Clamp(delta.y * tiltCursorMultiplier, -hoverTiltMax, hoverTiltMax);
            currentTiltX = IntegrateSpring(currentTiltX, targetTiltX, ref tiltXVelocity);

            transform.rotation = baseRotation * Quaternion.Euler(currentTiltX, 0f, currentTiltZ);
        }

        private float IntegrateSpring(float current, float target, ref float velocity)
        {
            float accel = (target - current) * springStiffness * Time.deltaTime
                          - velocity * springDamping * Time.deltaTime;
            velocity += accel;
            return current + velocity * Time.deltaTime;
        }

        private void OnMouseEnter()
        {
            if (isAnimating) return;
            isHovered = true;
            if (hoverRoutine != null) StopCoroutine(hoverRoutine);
            hoverRoutine = StartCoroutine(AnimateHover(true));
        }

        private void OnMouseExit()
        {
            isHovered = false;
            tiltZVelocity = 0f;
            currentTiltZ = 0f;
            tiltXVelocity = 0f;
            currentTiltX = 0f;
            if (hoverRoutine != null) StopCoroutine(hoverRoutine);
            hoverRoutine = StartCoroutine(AnimateHover(false));
        }

        private void OnMouseDown()
        {
            if (isAnimating) return;
            OnSelectToggleRequested?.Invoke(this);
        }

        /// <summary>
        /// Apply a selection state decided by a coordinator. Returns true if the state changed.
        /// Use this from HandActionController instead of ToggleSelected for cap-aware selection.
        /// </summary>
        public bool TrySetSelected(bool selected)
        {
            if (IsSelected == selected) return false;

            IsSelected = selected;
            SetGlowActive(IsSelected);
            selectionOffset = IsSelected ? Vector3.up * selectedLiftY : Vector3.zero;
            StartCoroutine(SelectPop());
            OnSelectionChanged?.Invoke(this, IsSelected);
            return true;
        }

        /// <summary>Small recoil animation when a selection request is rejected (e.g. cap reached).</summary>
        public IEnumerator BounceReject()
        {
            if (isAnimating) yield break;

            Vector3 origin = transform.localPosition;
            Vector3 recoil = origin + Vector3.down * BounceRecoilDistance;
            yield return LerpLocalPosition(origin, recoil, BounceRecoilDuration);
            yield return LerpLocalPosition(recoil, origin, BounceRecoilDuration);
            transform.localPosition = origin;
        }

        private void SetGlowActive(bool active)
        {
            if (glowRenderer != null)
                glowRenderer.gameObject.SetActive(active);
        }

        /// <summary>Set the sorting order on all renderers so hand overlap is correct.</summary>
        public void SetSortOrder(int order)
        {
            int baseOrder = SortOrderOffset + order * SortOrderStride;
            if (glowRenderer)  glowRenderer.sortingOrder  = baseOrder;
            if (backRenderer)  backRenderer.sortingOrder  = baseOrder + 1;
            if (frontRenderer) frontRenderer.sortingOrder = baseOrder + 1;
        }

        /// <summary>Coroutine for the Hand to smoothly reposition cards on layout.</summary>
        public IEnumerator MoveToPosition(Vector3 targetPos, Quaternion targetRot, float duration)
        {
            isAnimating = true;
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;
            Vector3 finalPos = targetPos + selectionOffset;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.localPosition = Vector3.Lerp(startPos, finalPos, t);
                transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            transform.localPosition = finalPos;
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

        private IEnumerator SelectPop()
        {
            Vector3 baseScale = transform.localScale;
            Vector3 peakScale = baseScale * SelectPopOvershoot;
            float elapsed = 0f;

            while (elapsed < SelectPopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / SelectPopDuration);
                transform.localScale = Vector3.Lerp(baseScale, peakScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < SelectPopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / SelectPopDuration);
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
            Vector3 restPos = basePosition + selectionOffset;
            Vector3 endPos = hoveringIn ? restPos + Vector3.up * hoverLiftY : restPos;

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
            hoverRoutine = null;
        }

        private IEnumerator LerpLocalPosition(Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.localPosition = Vector3.Lerp(from, to, t);
                yield return null;
            }
            transform.localPosition = to;
        }
    }
}
