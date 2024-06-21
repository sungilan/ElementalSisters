using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    public class IgnoreCharacterControllerCollider : MonoBehaviour
    {
        private Collider[] mainColliders; // 메인 콜라이더 배열

        private void Start()
        {
            mainColliders = GetComponentsInChildren<Collider>(true); // 하위 게임 오브젝트에서 모든 콜라이더를 가져옴
            var playerCollider = FindObjectOfType<CharacterController>(); // CharacterController를 찾음
            if (!playerCollider) return; // CharacterController를 찾지 못하면 반환
            foreach (var c in mainColliders) // 모든 메인 콜라이더에 대해 반복
            {
                Physics.IgnoreCollision(c, playerCollider); // 메인 콜라이더와 CharacterController의 충돌을 무시함
            }
        }
    }
}