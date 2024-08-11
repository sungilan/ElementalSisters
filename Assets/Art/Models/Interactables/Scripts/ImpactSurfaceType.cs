using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // ��� ǥ�� ������ �����ϴ� Ŭ����
    public class ImpactSurfaceType : MonoBehaviour, IImpactType
    {
        [SerializeField] private ImpactType impactType; // ��� ������ �����ϱ� ���� ����

        // ��� ������ ��ȯ�ϴ� �޼���
        public ImpactType GetImpactType() => impactType;
    }
}