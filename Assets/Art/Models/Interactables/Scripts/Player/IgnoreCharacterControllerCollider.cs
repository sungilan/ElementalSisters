using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    public class IgnoreCharacterControllerCollider : MonoBehaviour
    {
        private Collider[] mainColliders; // ���� �ݶ��̴� �迭

        private void Start()
        {
            mainColliders = GetComponentsInChildren<Collider>(true); // ���� ���� ������Ʈ���� ��� �ݶ��̴��� ������
            var playerCollider = FindObjectOfType<CharacterController>(); // CharacterController�� ã��
            if (!playerCollider) return; // CharacterController�� ã�� ���ϸ� ��ȯ
            foreach (var c in mainColliders) // ��� ���� �ݶ��̴��� ���� �ݺ�
            {
                Physics.IgnoreCollision(c, playerCollider); // ���� �ݶ��̴��� CharacterController�� �浹�� ������
            }
        }
    }
}