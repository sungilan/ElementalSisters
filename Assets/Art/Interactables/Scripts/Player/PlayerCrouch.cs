using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MikeNspired.UnityXRHandPoser
{
    public class PlayerCrouch : MonoBehaviour
    {
        [SerializeField] private InputActionReference crouchLeftHand, crouchRightHand; // 왼손, 오른손을 사용한 앉기 입력 액션
        [SerializeField] private float crouchOffSetReduction = .65f; // 카메라의 Y 오프셋을 줄이는 앉은 상태의 비율
        public XROrigin xrOrigin; // XR 원점
        private bool leftIsGripped, rightIsGripped, isCrouched; // 왼손, 오른손이 잡힌 상태 및 앉은 상태 여부
        private float crouchOffset; // 앉았을 때의 카메라 오프셋

        private void Awake()
        {
            OnValidate();
            // 앉기 토글 입력 액션에 대한 이벤트 리스너 추가
            crouchLeftHand.GetInputAction().performed += x => CrouchToggle();
            crouchRightHand.GetInputAction().performed += x => CrouchToggle();
        }

        private void OnValidate()
        {
            if (!gameObject.activeInHierarchy) return;

            if (!xrOrigin) xrOrigin = GetComponent<XROrigin>(); // XR 원점이 없으면 자신의 게임 오브젝트에서 가져옴
            if (!xrOrigin) xrOrigin = GetComponentInParent<XROrigin>(); // 부모 게임 오브젝트에서 가져옴
            crouchOffset = xrOrigin.CameraYOffset; // 초기 카메라 오프셋 설정
        }

        private void OnEnable()
        {
            crouchLeftHand.EnableAction(); // 앉기 입력 액션 활성화
            crouchRightHand.EnableAction(); // 앉기 입력 액션 활성화
        }

        private void OnDisable()
        {
            crouchLeftHand.DisableAction(); // 앉기 입력 액션 비활성화
            crouchRightHand.DisableAction(); // 앉기 입력 액션 비활성화
        }

        private void CrouchToggle()
        {
            switch (isCrouched)
            {
                case true:
                    xrOrigin.CameraYOffset = crouchOffset; // 서있을 때 카메라 오프셋을 초기값으로 설정
                    isCrouched = !isCrouched; // 서있는 상태로 변경
                    break;
                case false:
                    xrOrigin.CameraYOffset = crouchOffset * crouchOffSetReduction; // 앉았을 때 카메라 오프셋을 줄임
                    isCrouched = !isCrouched; // 앉은 상태로 변경
                    break;
            }
        }
    }
}