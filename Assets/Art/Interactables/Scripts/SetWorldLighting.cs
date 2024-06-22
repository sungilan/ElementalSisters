using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // 세계 조명을 설정하는 클래스
    public class SetWorldLighting : MonoBehaviour
    {
        [SerializeField] private Color color1 = Color.black; // 색상 1
        [SerializeField] private Color color2 = Color.black; // 색상 2

        [SerializeField] private Light mixedLight; // 혼합 라이트

        private Color startingColor; // 시작 색상
        private float startingIntensity; // 시작 강도
        private LightmapData[] startingLightMaps; // 시작 라이트맵

        private void Start()
        {
            startingColor = RenderSettings.ambientLight; // 초기 조명 설정 저장
            startingLightMaps = LightmapSettings.lightmaps; // 초기 라이트맵 설정 저장
            startingIntensity = mixedLight.intensity; // 초기 라이트 강도 저장
        }

        // 색상 1로 조명 설정
        public void SetToColor1()
        {
            RenderSettings.ambientLight = color1;
        }

        // 색상 2로 조명 설정
        public void SetToColor2()
        {
            RenderSettings.ambientLight = color2;
        }

        // 세계를 검게 만듦
        public void BlackenWorld()
        {
            RenderSettings.ambientLight = Color.black; // 조명을 검정색으로 설정
            LightmapSettings.lightmaps = new LightmapData[] { }; // 라이트맵을 비움
            mixedLight.intensity = .1f; // 혼합 라이트 강도 설정
        }

        // 시작 색상으로 복귀
        public void ReturnToStartingColor()
        {
            mixedLight.intensity = startingIntensity; // 시작 강도로 복귀
            RenderSettings.ambientLight = startingColor; // 시작 조명으로 복귀
            LightmapSettings.lightmaps = startingLightMaps; // 시작 라이트맵으로 복귀
        }

        // 상태를 정수 값에 따라 설정
        public void SetStateInt(int x)
        {
            if (x == 0)
                ReturnToStartingColor();
            else if (x == 1)
                SetToColor1();
            else
                SetToColor2();
        }
    }
}