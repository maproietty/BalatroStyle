using UnityEngine;
using System.Collections;

namespace BalatroStyle
{
    /// <summary>
    /// MonoBehaviour on a Card prefab. Handles visual state, hover wobble, selection.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Card : MonoBehaviour
    {
        [Header("Hover Settings")]
        [SerializeField] private float hoverScaleUp = 1.12f;
        [SerializeField] private float hoverLiftY = 0.35f;
        [SerializeField] private float hoverTiltMax = 8f;
        [SerializeField] private float springStiffness = 300f;
        [SerializeField] private float springDamping = 20f;

        [Header("References")]
        [SerializeField] private SpriteRenderer frontRenderer;
        [SerializeField] private SpriteRenderer backRenderer;
        [SerializeField] private SpriteRenderer glowRenderer;

        public CardData Data { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsFaceUp { get; private set; } = true;

        private SpriteRenderer sr;
        private Camera cam;

        // Spring physics for hover wobble
        private Vector3 basePosition;
        private Quaternion baseRotation;
        private float tiltVelocity;
        private float currentTilt;
        private bool isHovered;
        private bool isAnimating;

        private static readonly int GlowIntensityId = Shader.PropertyToID("_GlowIntensity");

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            cam = Camera.main;
        }

        public void Initialize(CardData data)
        {
            Data = data;
            if (frontRenderer != null && data.frontSprite != null)
                frontRenderer.sprite = data.frontSprite;
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
            float targetTilt = Mathf.Clamp(-delta.x * 10f, -hoverTiltMax, hoverTiltMax);
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

        public void ToggleSelected()
        {
            IsSelected = !IsSelected;
            SetGlowActive(IsSelected);
            // Lift selected card slightly
            basePosition += IsSelected ? Vector3.up * 0.2f : Vector3.down * 0.2f;
        }

        private void SetGlowActive(bool active)
        {
            if (glowRenderer != null)
                glowRenderer.gameObject.SetActive(active);
        }

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

        private IEnumerator AnimateHover(bool hoveringIn)
        {
            float duration = 0.12f;
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
