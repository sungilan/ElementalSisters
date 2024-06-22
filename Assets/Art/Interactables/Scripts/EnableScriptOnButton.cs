using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    // 버튼을 눌렀을 때 스크립트를 활성화하는 클래스
    public class EnableScriptOnButton : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour behaviour; // 활성화할 스크립트
        [SerializeField] private ActionBasedController actionBasedController; // 액션 기반 컨트롤러
        [SerializeField] private bool useTrigger; // 트리거 사용 여부
        [SerializeField] private bool useGrip; // 그립 사용 여부
        [SerializeField] private bool inverse; // 반전 여부

        private void Start()
        {
            OnValidate();

            // 액션 기반 컨트롤러가 없으면 비활성화
            if (!actionBasedController)
                enabled = false;

            // 그립 사용할 경우 이벤트 리스너 등록
            if (useGrip)
            {
                actionBasedController.selectActionValue.reference.GetInputAction().performed += x => Activate(!inverse);
                actionBasedController.selectActionValue.reference.GetInputAction().canceled += x => Activate(inverse);
            }

            // 트리거 사용할 경우 이벤트 리스너 등록
            if (useTrigger)
            {
                actionBasedController.activateActionValue.reference.GetInputAction().performed += x => Activate(!inverse);
                actionBasedController.activateActionValue.reference.GetInputAction().canceled += x => Activate(inverse);
            }
        }

        // 유효성 검사
        private void OnValidate()
        {
            if (!actionBasedController) actionBasedController = GetComponentInParent<ActionBasedController>();
        }

        // 스크립트 활성화/비활성화 메서드
        private void Activate(bool state)
        {
            if (behaviour)
                behaviour.enabled = state;
        }
    }
}