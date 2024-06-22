using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // 대미지를 입을 수 있는 인터페이스
    public interface IDamageable
    {
        // 대미지를 받는 메서드
        void TakeDamage(float damage, GameObject damager);
    }

    // 충격 유형을 반환할 수 있는 인터페이스
    public interface IImpactType
    {
        // 충격 유형을 반환하는 메서드
        ImpactType GetImpactType();
    }

    // 충격 유형 열거형
    public enum ImpactType
    {
        Metal,    // 금속
        Flesh,    // 살
        Wood,     // 나무
        Neutral   // 중립
    }
}