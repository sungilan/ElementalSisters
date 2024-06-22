using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikeNspired.UnityXRHandPoser
{
    // 손전등을 제어하는 클래스
    public class FlashLight : MonoBehaviour
    {
        // 손전등의 색깔 변수들
        public Color colorOne = Color.white;
        public Color colorTwo = Color.yellow;
        public Color colorThree = Color.blue;
        public Color colorFour = Color.green;
        public Color colorFive = Color.red;
        public Color colorSix = Color.cyan;

        // 렌더러와 손전등 컴포넌트
        public Renderer rend;
        public Light flashLight;

        // 손전등 활성화 여부
        public bool isEnabled = true;

        // 손전등의 최소 밝기와 최대 밝기
        public float minBrightness = .5f, maxBrightness = 5;

        // 시작 시 손전등 설정
        private void Start()
        {
            flashLight.enabled = isEnabled;
            rend.enabled = isEnabled;
        }

        // 손전등 상태 전환
        public void SwitchState()
        {
            isEnabled = !isEnabled;
            flashLight.enabled = isEnabled;
            rend.enabled = isEnabled;
        }

        // 손전등 밝기 설정
        public void SetBrightness(float dialPercentage)
        {
            // 다이얼 비율을 최소 밝기부터 최대 밝기로 변환하여 손전등의 밝기 설정
            var dialValueZeroToOne = Remap(dialPercentage, 0f, 1f, minBrightness, maxBrightness);
            flashLight.intensity = dialValueZeroToOne;
        }

        // 값 재매핑 함수
        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        // 손전등 색상 설정
        public void SetColor(int color)
        {
            // 색상에 따라 손전등 색상 및 렌더러의 재질 색상 설정
            switch (color)
            {
                case 0:
                    flashLight.color = colorOne;
                    rend.material.SetColor("_EmissionColor", colorOne);
                    break;
                case 1:
                    flashLight.color = colorTwo;
                    rend.material.SetColor("_EmissionColor", colorTwo);
                    break;
                case 2:
                    flashLight.color = colorThree;
                    rend.material.SetColor("_EmissionColor", colorThree);
                    break;
                case 3:
                    flashLight.color = colorFour;
                    rend.material.SetColor("_EmissionColor", colorFour);
                    break;
                case 4:
                    flashLight.color = colorFive;
                    rend.material.SetColor("_EmissionColor", colorFive);
                    break;
                case 5:
                    flashLight.color = colorSix;
                    rend.material.SetColor("_EmissionColor", colorSix);
                    break;
            }
        }
    }
}