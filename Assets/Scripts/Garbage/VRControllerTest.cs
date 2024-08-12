using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRControllerInteraction : MonoBehaviour
{
    public InputActionAsset inputActionAsset; // Input actions asset
    public XRRayInteractor rayInteractor; // XRRayInteractor reference

    private void Start()
    {
        // XRRayInteractor 컴포넌트를 찾습니다.
        //rayInteractor = GetComponent<XRRayInteractor>();
        if (rayInteractor == null)
        {
            Debug.LogError("XRRayInteractor component not found!");
        }
    }

    private void Update()
    {
        if (rayInteractor != null)
        {
            // 레이캐스트가 활성화된 상태에서 입력을 확인합니다.
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // 레이캐스트가 물체에 닿았을 때 입력을 처리합니다.
                float gripValue = inputActionAsset.actionMaps[2].actions[0].ReadValue<float>();
                float triggerValue = inputActionAsset.actionMaps[2].actions[2].ReadValue<float>();

                // 그립 버튼 상태 확인
                if (gripValue > 0.5f)
                {
                    OnGripPressed();
                }

                // 트리거 버튼 상태 확인
                if (triggerValue > 0.5f)
                {
                    OnTriggerPressed();
                }
            }
        }
    }

    private void OnGripPressed()
    {
        Debug.Log("그립 버튼 눌림");
    }

    private void OnTriggerPressed()
    {
        Debug.Log("트리거 버튼 눌림");
    }
}
