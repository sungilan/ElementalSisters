using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    public class TeleportRayEnabler : MonoBehaviour
    {
        [SerializeField] private XRRayInteractor teleportRayInteractor; // 텔레포트 레이 상호작용기
        [SerializeField] private InputActionReference teleportActivate; // 텔레포트 활성화 입력 액션
        [SerializeField] private TeleportationProvider teleportationProvider; // 텔레포트 제공자

        private void Start()
        {
            OnValidate();
            LogMessages();
            // 텔레포트 활성화 입력 액션에 대한 이벤트 리스너 추가
            teleportActivate.GetInputAction().performed += context => EnableRay();
            teleportActivate.GetInputAction().canceled += context => DisableRay();
            // 초기에는 텔레포트 레이 상호작용기를 비활성화 상태로 설정
            teleportRayInteractor.enabled = false;
        }

        private void OnValidate()
        {
            if (!teleportationProvider) teleportationProvider = GetComponentInParent<TeleportationProvider>(); // 텔레포트 제공자가 없으면 부모에서 가져옴
        }

        private void EnableRay()
        {
            if (!teleportationProvider.enabled) return; // 텔레포트 제공자가 비활성화된 경우 레이 활성화하지 않음
            teleportRayInteractor.enabled = true; // 텔레포트 레이 상호작용기 활성화
        }

        // 다음 프레임까지 레이를 비활성화하지 않으면 텔레포트가 발생하지 않음
        private void DisableRay() => StartCoroutine(DisableInteractable());

        private IEnumerator DisableInteractable()
        {
            yield return null;
            teleportRayInteractor.enabled = false; // 레이 비활성화
        }

        private void LogMessages()
        {
            if (!teleportActivate)
            {
                Debug.Log("TeleportRayEnabler가 입력 액션을 누락했습니다.");
                enabled = false;
            }

            if (!teleportRayInteractor)
            {
                Debug.Log("TeleportRayEnabler가 teleportRayInteractor에 대한 참조를 누락했습니다.");
                enabled = false;
            }
        }
    }
}