using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사용 가능한 챔피언 생성 및 저장, 경험치 및 레벨 구매
/// </summary>
public class ChampionShop : MonoBehaviour
{
    public UIController uIController; // UI 컨트롤러
    public GamePlayController gamePlayController; // 게임 플레이 컨트롤러
    public GameData gameData; // 게임 데이터

    /// 구매할 수 있는 챔피언을 저장하는 배열
    public Champion[] availableChampionArray;

    public bool isLock = false;

    /// Start is called before the first frame update
    void Start()
    {
        RefreshShop(true); // 초기 상점 업데이트
    }

    /// Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 플레이어의 레벨을 상승시킵니다.
    /// </summary>
    public void BuyLvl()
    {
        gamePlayController.Buylvl();
    }

    /// <summary>
    /// 새로운 무작위 챔피언으로 상점을 새로 고칩니다.
    /// </summary>
    public void RefreshShop(bool isFree)
    {
        if(!isLock)
        {
            // 충분한 골드가 없다면 반환
            if (gamePlayController.currentGold < 2 && isFree == false)
            return;

            // 배열 초기화
            availableChampionArray = new Champion[5];

            // 상점 채우기
            for (int i = 0; i < availableChampionArray.Length; i++)
            {
                // 무작위 챔피언 가져오기
                Champion champion = GetRandomChampionInfo();
           
                // 챔피언을 배열에 저장
                availableChampionArray[i] = champion;

                // UI에 챔피언 로드
                uIController.LoadShopItem(champion, i);
          
                // 상점 아이템 표시
                uIController.ShowShopItems();
            }

            // 골드 차감
            if (isFree == false)
                gamePlayController.currentGold -= 2;

            // UI 업데이트
            uIController.UpdateUI();
        }
    }

    /// <summary>
    /// UI 챔피언 프레임을 클릭했을 때 호출됩니다.
    /// </summary>
    /// <param name="index">클릭한 챔피언 프레임의 인덱스입니다.</param>
    public void OnChampionFrameClicked(int index)
    {
        bool isSuccess = gamePlayController.BuyChampionFromShop(availableChampionArray[index]);

        if (isSuccess)
            uIController.HideChampionFrame(index);
    }

    /// <summary>
    /// 무작위 챔피언을 반환합니다.
    /// </summary>
    public Champion GetRandomChampionInfo()
    {
        // 무작위 번호 생성
        int rand = Random.Range(0, gameData.championsArray.Length);

        // 배열에서 반환
        return gameData.championsArray[rand];

    }
}
