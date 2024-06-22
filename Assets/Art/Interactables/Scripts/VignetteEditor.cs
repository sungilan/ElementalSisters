using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VignetteEditor : MonoBehaviour
{
    private TunnelingVignetteController vignetteController; // 터널링 비네트 컨트롤러

    private void Awake() => vignetteController = GetComponent<TunnelingVignetteController>(); // Awake 메서드

    // 조리개 크기 설정 메서드
    public void SetApertureSize(float value) => vignetteController.defaultParameters.apertureSize = value;

    // 흐림 효과 크기 설정 메서드
    public void SetFeatheringSize(float value) => vignetteController.defaultParameters.featheringEffect = value;
}