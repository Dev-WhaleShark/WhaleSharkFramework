using Unity.Cinemachine;
using UnityEngine;

namespace WhaleShark.Core
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController I;

        [Header("Camera References")]
        public CinemachineCamera followCamera;
        public CinemachineImpulseSource impulseSource;

        [Header("Shake Settings")]
        public float shakeIntensity = 1f;
        public float shakeDuration = 0.2f;

        Transform followTarget;

        void Awake()
        {
            I = this;
        }

        void Start()
        {
            EventBus.Hit += OnHit;
        }

        void OnDestroy()
        {
            EventBus.Hit -= OnHit;
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            if (followCamera != null)
            {
                // Unity 6 Cinemachine의 새로운 Target 시스템
                followCamera.Target.TrackingTarget = target;

                // LookAt은 선택적으로 설정 (2D 게임에서는 보통 불필요)
                // followCamera.LookAt = target;
            }
        }

        public void Shake(float intensity = -1f)
        {
            if (impulseSource == null) return;

            float actualIntensity = intensity < 0 ? shakeIntensity : intensity;

            var impulseForce = Vector3.one * actualIntensity;
            impulseSource.GenerateImpulse(impulseForce);
        }

        void OnHit(Vector3 hitPosition)
        {
            Shake();
        }

        public void SetCameraDistance(float distance)
        {
            if (followCamera == null) return;

            // Transform을 직접 사용하는 간단한 방법
            var cameraTransform = followCamera.transform;
            var currentPos = cameraTransform.localPosition;
            cameraTransform.localPosition = new Vector3(currentPos.x, currentPos.y, -distance);
        }

        /// <summary>
        /// 카메라의 댐핑과 거리를 설정합니다.
        /// 주의: Unity 6 Cinemachine에서는 에디터에서 미리 설정하는 것을 권장합니다.
        /// </summary>
        public void SetCameraSettings(float damping, float distance)
        {
            if (followCamera == null) return;

            // 거리만 런타임에 변경
            SetCameraDistance(distance);

            // Damping은 복잡한 컴포넌트 시스템으로 인해 에디터에서 설정 권장
            Debug.Log($"Camera distance set to {distance}. Damping should be set in editor for Cinemachine 3.x");
        }

        /// <summary>
        /// 카메라의 우선순위를 설정합니다. 높은 값이 우선됩니다.
        /// </summary>
        public void SetCameraPriority(int priority)
        {
            if (followCamera != null)
            {
                followCamera.Priority = priority;
            }
        }

        /// <summary>
        /// 카메라의 활성화 상태를 설정합니다.
        /// </summary>
        public void SetCameraActive(bool active)
        {
            if (followCamera != null)
            {
                followCamera.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// 현재 카메라가 활성화되어 있는지 확인합니다.
        /// </summary>
        public bool IsCameraActive()
        {
            return followCamera != null && followCamera.gameObject.activeInHierarchy;
        }
    }
}