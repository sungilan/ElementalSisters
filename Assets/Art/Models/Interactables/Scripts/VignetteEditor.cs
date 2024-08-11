using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VignetteEditor : MonoBehaviour
{
    private TunnelingVignetteController vignetteController; // �ͳθ� ���Ʈ ��Ʈ�ѷ�

    private void Awake() => vignetteController = GetComponent<TunnelingVignetteController>(); // Awake �޼���

    // ������ ũ�� ���� �޼���
    public void SetApertureSize(float value) => vignetteController.defaultParameters.apertureSize = value;

    // �帲 ȿ�� ũ�� ���� �޼���
    public void SetFeatheringSize(float value) => vignetteController.defaultParameters.featheringEffect = value;
}