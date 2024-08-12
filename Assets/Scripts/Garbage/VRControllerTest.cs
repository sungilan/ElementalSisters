using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRControllerInteraction : MonoBehaviour
{
    public InputActionAsset inputActionAsset; // Input actions asset
    public XRRayInteractor rayInteractor; // XRRayInteractor reference

    private void Start()
    {
        // XRRayInteractor ������Ʈ�� ã���ϴ�.
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
            // ����ĳ��Ʈ�� Ȱ��ȭ�� ���¿��� �Է��� Ȯ���մϴ�.
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // ����ĳ��Ʈ�� ��ü�� ����� �� �Է��� ó���մϴ�.
                float gripValue = inputActionAsset.actionMaps[2].actions[0].ReadValue<float>();
                float triggerValue = inputActionAsset.actionMaps[2].actions[2].ReadValue<float>();

                // �׸� ��ư ���� Ȯ��
                if (gripValue > 0.5f)
                {
                    OnGripPressed();
                }

                // Ʈ���� ��ư ���� Ȯ��
                if (triggerValue > 0.5f)
                {
                    OnTriggerPressed();
                }
            }
        }
    }

    private void OnGripPressed()
    {
        Debug.Log("�׸� ��ư ����");
    }

    private void OnTriggerPressed()
    {
        Debug.Log("Ʈ���� ��ư ����");
    }
}
