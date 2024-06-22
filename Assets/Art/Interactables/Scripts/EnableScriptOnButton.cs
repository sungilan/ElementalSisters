using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    // ��ư�� ������ �� ��ũ��Ʈ�� Ȱ��ȭ�ϴ� Ŭ����
    public class EnableScriptOnButton : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour behaviour; // Ȱ��ȭ�� ��ũ��Ʈ
        [SerializeField] private ActionBasedController actionBasedController; // �׼� ��� ��Ʈ�ѷ�
        [SerializeField] private bool useTrigger; // Ʈ���� ��� ����
        [SerializeField] private bool useGrip; // �׸� ��� ����
        [SerializeField] private bool inverse; // ���� ����

        private void Start()
        {
            OnValidate();

            // �׼� ��� ��Ʈ�ѷ��� ������ ��Ȱ��ȭ
            if (!actionBasedController)
                enabled = false;

            // �׸� ����� ��� �̺�Ʈ ������ ���
            if (useGrip)
            {
                actionBasedController.selectActionValue.reference.GetInputAction().performed += x => Activate(!inverse);
                actionBasedController.selectActionValue.reference.GetInputAction().canceled += x => Activate(inverse);
            }

            // Ʈ���� ����� ��� �̺�Ʈ ������ ���
            if (useTrigger)
            {
                actionBasedController.activateActionValue.reference.GetInputAction().performed += x => Activate(!inverse);
                actionBasedController.activateActionValue.reference.GetInputAction().canceled += x => Activate(inverse);
            }
        }

        // ��ȿ�� �˻�
        private void OnValidate()
        {
            if (!actionBasedController) actionBasedController = GetComponentInParent<ActionBasedController>();
        }

        // ��ũ��Ʈ Ȱ��ȭ/��Ȱ��ȭ �޼���
        private void Activate(bool state)
        {
            if (behaviour)
                behaviour.enabled = state;
        }
    }
}