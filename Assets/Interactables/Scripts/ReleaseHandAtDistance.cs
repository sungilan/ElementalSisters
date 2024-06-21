using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    // 일정 거리에 도달하면 핸드에서 아이템을 해제하는 클래스
    public class ReleaseHandAtDistance : MonoBehaviour
    {
        [SerializeField] private XRBaseInteractable baseInteractable; // XRBaseInteractable을 나타내는 변수
        [SerializeField] private float distance = .2f; // 거리
        [SerializeField] public bool debugSpheresEnabled; // 디버그 스피어를 활성화할지 여부를 나타내는 변수

        private IXRSelectInteractor interactor; // IXRSelectInteractor를 나타내는 변수
        private IXRSelectInteractable interactable; // IXRSelectInteractable를 나타내는 변수
        private XRInteractionManager interactionManager; // XRInteractionManager를 나타내는 변수

        private void Start()
        {
            OnValidate(); // 유효성 검사 메서드 호출
            LogMessages(); // 로그 메시지 출력 메서드 호출
            // IXRSelectInteractable의 selectEntered 이벤트에 리스너 추가
            interactable.selectEntered.AddListener(x => interactor = x.interactorObject);
            // IXRSelectInteractable의 selectExited 이벤트에 리스너 추가
            interactable.selectExited.AddListener(x => interactor = null);
        }

        private void OnValidate()
        {
            // XRBaseInteractable이 없으면 현재 게임 오브젝트의 부모에서 가져옴
            if (!baseInteractable)
                baseInteractable = GetComponentInParent<XRBaseInteractable>();
            // XRInteractionManager를 찾아서 가져옴
            if (!interactionManager)
                interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        private void Update()
        {
            // 인터랙터가 없으면 종료
            if (interactor == null) return;
            // 거리가 설정한 거리보다 짧으면 종료
            if (Vector3.Distance(interactable.transform.position, interactor.transform.position) < distance) return;
            // 아이템 해제 메서드 호출
            ReleaseItemFromHand();
        }

        // 핸드에서 아이템을 해제하는 메서드
        private void ReleaseItemFromHand()
        {
            // XRInteractionManager의 SelectExit 메서드 호출하여 아이템 해제
            interactionManager.SelectExit(interactor, interactable);
            interactor = null; // 인터랙터 초기화
        }

        // 로그 메시지 출력 메서드
        private void LogMessages()
        {
            // 인터랙터가 없으면 경고 로그 출력하고 해당 스크립트 비활성화
            if (interactable == null)
            {
                Debug.LogWarning(this + " missing interactable on : " + gameObject);
                enabled = false;
            }

            // XRInteractionManager가 없으면 경고 로그 출력하고 해당 스크립트 비활성화
            if (interactionManager == null)
            {
                Debug.LogWarning(this + " No XRInteractionManager found in scene: " + gameObject);
                enabled = false;
            }
        }

        // 디버그 스피어를 그리는 메서드
        private void OnDrawGizmosSelected()
        {
            // 디버그 스피어가 활성화되어 있으면 스피어 그리기
            if (debugSpheresEnabled) Gizmos.DrawWireSphere(transform.position, distance);
        }
    }
}