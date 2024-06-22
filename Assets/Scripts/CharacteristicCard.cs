using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacteristicCard : MonoBehaviour
{
    [SerializeField] private GamePlayController gamePlayController;
    [SerializeField] private GameObject selectEffect;
    [SerializeField] private ChampionController champion;  // 보너스를 적용할 챔피언
    [SerializeField] private ChampionBonus championBonus;  // 적용할 보너스

    private void Start()
    {
        gamePlayController = GameObject.Find("Scripts").GetComponent<GamePlayController>();
    }

    IEnumerator CharateristicCardEffect()
    {
        // 이펙트 생성
        GameObject cardEffect = Instantiate(selectEffect, this.gameObject.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);

        // 이펙트 제거
        Destroy(cardEffect);

    }

    public void SelectcharateristicCard()
    {
        // 코루틴 시작
        StartCoroutine(CharateristicCardEffect());

        // 이 게임 오브젝트 비활성화 및 로그 메시지 출력
        this.gameObject.SetActive(false);
    }

}