// 작성자: MikeNspired.
using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // 콜라이더를 활성화/비활성화하고 초기 위치로 되돌리는 클래스
    public class ColliderDisableMoveReturn : MonoBehaviour
    {
        public BoxCollider col; // 대상 콜라이더
        private Vector3 startingPosition; // 초기 위치 저장 변수

        private void Start()
        {
            startingPosition = col.center; // 초기 위치 저장
        }

        // 콜라이더를 비활성화하는 메서드
        public void DisableCollider()
        {
            if (!col.enabled) return; // 이미 비활성화된 경우 종료

            col.center = Vector3.forward * 1000; // 멀리 이동하여 숨김
            Invoke(nameof(Disable), .1f); // 지연 후 완전히 비활성화
        }

        // 콜라이더를 활성화하는 메서드
        public void EnableCollider()
        {
            if (col.enabled) return; // 이미 활성화된 경우 종료

            col.center = startingPosition; // 초기 위치로 되돌림
            col.enabled = true; // 콜라이더 활성화
        }

        // 비활성화 메서드
        private void Disable()
        {
            col.enabled = false; // 콜라이더 비활성화
            col.center = startingPosition; // 초기 위치로 되돌림
        }
    }
}