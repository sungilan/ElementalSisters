using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // 모든 콜라이더를 트리거로 설정하는 클래스
    public class CollidersSetToTrigger : MonoBehaviour
    {
        // 콜라이더 데이터를 저장하는 리스트
        [SerializeField] List<colliderData> colliders = new List<colliderData>();

        private void Start()
        {
            // 자식 오브젝트에서 모든 콜라이더를 가져와서 리스트에 추가
            var cols = GetComponentsInChildren<Collider>();
            foreach (var c in cols)
                colliders.Add(new colliderData(c, c.isTrigger));
        }

        // 모든 콜라이더를 트리거로 설정하는 메서드
        public void SetAllToTrigger()
        {
            foreach (var c in colliders)
                c.collider.isTrigger = true;
        }

        // 기본 상태로 돌아가는 메서드
        public void ReturnToDefaultState()
        {
            foreach (var c in colliders)
                c.collider.isTrigger = c.isTrigger;
        }

        // 콜라이더 데이터 구조체
        [Serializable]
        private struct colliderData
        {
            public Collider collider; // 콜라이더
            public bool isTrigger; // 트리거 여부

            // 생성자
            public colliderData(Collider c, bool isTrigger)
            {
                collider = c;
                this.isTrigger = isTrigger;
            }
        }
    }
}