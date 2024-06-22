// Author MikeNspired.
// 작성자: MikeNspired.

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    public class OnGrabEnableDisable : MonoBehaviour, IReturnMovedColliders
    {
        [SerializeField] private XRGrabInteractable grabInteractable;
        // XRGrabInteractable을 참조할 SerializeField로 선언된 변수

        [Tooltip("Transform gets disabled when the interactable is grabbed and enabled when released")]
        [SerializeField]
        private Transform disableOnGrab = null;
        // 상호 작용이 발생하면 비활성화되고 릴리스될 때 활성화되는 Transform

        [Tooltip("Transform is disabled at start, and enabled when the interactable is grabbed, and disabled when released")]
        [SerializeField]
        private Transform enableOnGrab = null;
        // 시작 시 비활성화되고 상호 작용이 발생하면 활성화되며, 릴리스될 때 비활성화되는 Transform

        [SerializeField] private bool moveAndDisableAfterFrameOnGrabColliders = true;
        // 상호 작용이 발생한 후 프레임 이후에 컬라이더를 이동하고 비활성화할지 여부를 결정하는 부울 값

        private bool PreventDisableOfCollidersForObjectDisable;
        private Vector3 enableOnGrabStartPosition;
        private Vector3 disableOnGrabStartPosition;

        private void Awake()
        {
            OnValidate();

            grabInteractable.onSelectEntered.AddListener(x => OnGrab());
            grabInteractable.onSelectExited.AddListener(x => OnRelease());

            if (disableOnGrab) disableOnGrabStartPosition = disableOnGrab.transform.localPosition;
            if (enableOnGrab) enableOnGrabStartPosition = enableOnGrab.transform.localPosition;
        }
        // Awake 메서드: 유효성을 검사하고 이벤트 리스너를 추가하며 초기 위치를 설정하는 메서드

        private void OnValidate()
        {
            if (!grabInteractable)
                grabInteractable = GetComponent<XRGrabInteractable>();
        }
        // OnValidate 메서드: XRGrabInteractable 컴포넌트가 없으면 자동으로 할당하는 메서드

        private void Start()
        {
            if (disableOnGrab) disableOnGrab.gameObject.SetActive(true);
            if (enableOnGrab) enableOnGrab.gameObject.SetActive(false);
        }
        // Start 메서드: 시작 시 disableOnGrab이 활성화되고 enableOnGrab이 비활성화되도록 설정하는 메서드

        public void EnableAll()
        {
            StopAllCoroutines();

            if (disableOnGrab)
            {
                disableOnGrab.gameObject.SetActive(true);
                disableOnGrab.transform.localPosition = disableOnGrabStartPosition;
                disableOnGrab.GetComponent<CollidersSetToTrigger>()?.ReturnToDefaultState();
            }

            if (enableOnGrab)
            {
                enableOnGrab.gameObject.SetActive(true);
                enableOnGrab.transform.localPosition = enableOnGrabStartPosition;
                enableOnGrab.GetComponent<CollidersSetToTrigger>()?.ReturnToDefaultState();
            }
        }
        // EnableAll 메서드: 모든 오브젝트를 활성화하는 메서드

        private void OnRelease()
        {
            if (moveAndDisableAfterFrameOnGrabColliders)
            {
                StopAllCoroutines();
                if (disableOnGrab)
                    disableOnGrab.GetComponent<CollidersSetToTrigger>()?.ReturnToDefaultState();
                StartCoroutine(MoveDisableAndReturn(enableOnGrab));
            }
            else if (enableOnGrab)
                enableOnGrab.gameObject.SetActive(false);

            if (disableOnGrab)
                disableOnGrab.gameObject.SetActive(true);
        }
        // OnRelease 메서드: 상호 작용이 해제될 때 실행되는 메서드

        private void OnGrab()
        {
            if (moveAndDisableAfterFrameOnGrabColliders)
            {
                StopAllCoroutines();
                if (enableOnGrab)
                    enableOnGrab.GetComponent<CollidersSetToTrigger>()?.ReturnToDefaultState();
                StartCoroutine(MoveDisableAndReturn(disableOnGrab));
            }
            else if (disableOnGrab)
                disableOnGrab.gameObject.SetActive(false);

            if (enableOnGrab)
            {
                enableOnGrab.gameObject.SetActive(true);
                enableOnGrab.transform.localPosition = enableOnGrabStartPosition;
            }
        }
        // OnGrab 메서드: 상호 작용이 발생했을 때 실행되는 메서드

        private IEnumerator MoveDisableAndReturn(Transform objectToMove)
        {
            if (!objectToMove) yield break;
            objectToMove.GetComponent<CollidersSetToTrigger>()?.SetAllToTrigger();
            yield return new WaitForSeconds(Time.fixedDeltaTime * 2);

            objectToMove.position += Vector3.one * 9999;
            yield return new WaitForSeconds(Time.fixedDeltaTime * 2);
            objectToMove.gameObject.SetActive(false);
            objectToMove.localPosition = objectToMove == enableOnGrab ? enableOnGrabStartPosition : disableOnGrabStartPosition;

            objectToMove.GetComponent<CollidersSetToTrigger>()?.ReturnToDefaultState();
        }
        // MoveDisableAndReturn 메서드: 오브젝트를 이동하고 비활성화한 후 원래 위치로 되돌리는 메서드

        public void ReturnMovedColliders()
        {
            StopAllCoroutines();
            if (enableOnGrab)
                enableOnGrab.localPosition = enableOnGrabStartPosition;
            if (disableOnGrab)
                disableOnGrab.localPosition = disableOnGrabStartPosition;
        }
        // ReturnMovedColliders 메서드: 이동된 컬라이더를 원래 위치로 되돌리는 메서드
    }
}