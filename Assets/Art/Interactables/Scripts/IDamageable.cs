using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // ������� ���� �� �ִ� �������̽�
    public interface IDamageable
    {
        // ������� �޴� �޼���
        void TakeDamage(float damage, GameObject damager);
    }

    // ��� ������ ��ȯ�� �� �ִ� �������̽�
    public interface IImpactType
    {
        // ��� ������ ��ȯ�ϴ� �޼���
        ImpactType GetImpactType();
    }

    // ��� ���� ������
    public enum ImpactType
    {
        Metal,    // �ݼ�
        Flesh,    // ��
        Wood,     // ����
        Neutral   // �߸�
    }
}