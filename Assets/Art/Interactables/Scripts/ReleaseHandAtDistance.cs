using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    // ���� �Ÿ��� �����ϸ� �ڵ忡�� �������� �����ϴ� Ŭ����
    public class ReleaseHandAtDistance : MonoBehaviour
    {
        [SerializeField] private XRBaseInteractable baseInteractable; // XRBaseInteractable�� ��Ÿ���� ����
        [SerializeField] private float distance = .2f; // �Ÿ�
        [SerializeField] public bool debugSpheresEnabled; // ����� ���Ǿ Ȱ��ȭ���� ���θ� ��Ÿ���� ����

        private IXRSelectInteractor interactor; // IXRSelectInteractor�� ��Ÿ���� ����
        private IXRSelectInteractable interactable; // IXRSelectInteractable�� ��Ÿ���� ����
        private XRInteractionManager interactionManager; // XRInteractionManager�� ��Ÿ���� ����

        private void Start()
        {
            OnValidate(); // ��ȿ�� �˻� �޼��� ȣ��
            LogMessages(); // �α� �޽��� ��� �޼��� ȣ��
            // IXRSelectInteractable�� selectEntered �̺�Ʈ�� ������ �߰�
            interactable.selectEntered.AddListener(x => interactor = x.interactorObject);
            // IXRSelectInteractable�� selectExited �̺�Ʈ�� ������ �߰�
            interactable.selectExited.AddListener(x => interactor = null);
        }

        private void OnValidate()
        {
            // XRBaseInteractable�� ������ ���� ���� ������Ʈ�� �θ𿡼� ������
            if (!baseInteractable)
                baseInteractable = GetComponentInParent<XRBaseInteractable>();
            // XRInteractionManager�� ã�Ƽ� ������
            if (!interactionManager)
                interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        private void Update()
        {
            // ���ͷ��Ͱ� ������ ����
            if (interactor == null) return;
            // �Ÿ��� ������ �Ÿ����� ª���� ����
            if (Vector3.Distance(interactable.transform.position, interactor.transform.position) < distance) return;
            // ������ ���� �޼��� ȣ��
            ReleaseItemFromHand();
        }

        // �ڵ忡�� �������� �����ϴ� �޼���
        private void ReleaseItemFromHand()
        {
            // XRInteractionManager�� SelectExit �޼��� ȣ���Ͽ� ������ ����
            interactionManager.SelectExit(interactor, interactable);
            interactor = null; // ���ͷ��� �ʱ�ȭ
        }

        // �α� �޽��� ��� �޼���
        private void LogMessages()
        {
            // ���ͷ��Ͱ� ������ ��� �α� ����ϰ� �ش� ��ũ��Ʈ ��Ȱ��ȭ
            if (interactable == null)
            {
                Debug.LogWarning(this + " missing interactable on : " + gameObject);
                enabled = false;
            }

            // XRInteractionManager�� ������ ��� �α� ����ϰ� �ش� ��ũ��Ʈ ��Ȱ��ȭ
            if (interactionManager == null)
            {
                Debug.LogWarning(this + " No XRInteractionManager found in scene: " + gameObject);
                enabled = false;
            }
        }

        // ����� ���Ǿ �׸��� �޼���
        private void OnDrawGizmosSelected()
        {
            // ����� ���Ǿ Ȱ��ȭ�Ǿ� ������ ���Ǿ� �׸���
            if (debugSpheresEnabled) Gizmos.DrawWireSphere(transform.position, distance);
        }
    }
}