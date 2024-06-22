using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // 충격 표면 유형을 정의하는 클래스
    public class ImpactSurfaceType : MonoBehaviour, IImpactType
    {
        [SerializeField] private ImpactType impactType; // 충격 유형을 설정하기 위한 변수

        // 충격 유형을 반환하는 메서드
        public ImpactType GetImpactType() => impactType;
    }
}