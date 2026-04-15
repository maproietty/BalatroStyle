using UnityEngine;

namespace BalatroStyle
{
    /// <summary>
    /// Scales a background Quad to fully cover the target camera's view each frame.
    /// Attach this to a Quad GameObject whose material uses the BalatroStyle/AnimatedBackground shader.
    /// Runs in edit mode so the background previews correctly without pressing Play.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    public class AnimatedBackground : MonoBehaviour
    {
        private const float DefaultOverscan = 1.10f;
        private const float DefaultDepth = 10f;

        [Header("Camera")]
        [Tooltip("Camera the background should cover. Falls back to Camera.main when empty.")]
        [SerializeField] private Camera targetCamera;

        [Header("Layout")]
        [Tooltip("Extra size multiplier so edges stay hidden when CRT curvature pulls the image inward.")]
        [SerializeField] private float overscan = DefaultOverscan;

        [Tooltip("Distance in front of a perspective camera. Ignored for orthographic cameras.")]
        [SerializeField] private float depth = DefaultDepth;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            FitToCamera();
        }

        private void OnValidate()
        {
            FitToCamera();
        }

        /// <summary>
        /// Resizes and positions this GameObject so its Quad fully covers the target camera's frustum.
        /// </summary>
        public void FitToCamera()
        {
            if (targetCamera == null)
            {
                return;
            }

            if (targetCamera.orthographic)
            {
                float height = targetCamera.orthographicSize * 2f * overscan;
                float width = height * targetCamera.aspect;
                transform.localScale = new Vector3(width, height, 1f);
                Vector3 camPos = targetCamera.transform.position;
                transform.position = new Vector3(camPos.x, camPos.y, 0f);
                transform.rotation = Quaternion.identity;
            }
            else
            {
                float height = 2f * depth * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * overscan;
                float width = height * targetCamera.aspect;
                transform.localScale = new Vector3(width, height, 1f);
                transform.position = targetCamera.transform.position + targetCamera.transform.forward * depth;
                transform.rotation = targetCamera.transform.rotation;
            }
        }
    }
}
