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
                Champion champion = GetRandomChampionByPlayerLevel(gamePlayController.currentLevel);
           
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

// 각 플레이어 레벨별로 1코스트 ~ 5코스트 챔피언의 출현 확률 (합은 100)
private static readonly int[,] levelCostProbability = new int[,]
{
    // 1레벨, 2레벨, 3레벨, 4레벨, 5레벨, ..., n레벨
    {100, 0, 0, 0, 0},    // 1레벨에서의 확률 (100% 1코스트)
    {100, 0, 0, 0, 0},    // 2레벨에서의 확률 (75% 1코스트, 25% 2코스트)
    {75, 25, 0, 0, 0},   // 3레벨에서의 확률 (55% 1코스트, 30% 2코스트, 15% 3코스트)
    {55, 30, 15, 0, 0},   // 4레벨에서의 확률
    {45, 33, 20, 2, 0},   // 5레벨에서의 확률
    {30, 40, 25, 5, 0},  // 6레벨에서의 확률
    {19, 35, 35, 10, 1},  // 7레벨에서의 확률
    {18, 25, 36, 18, 3},  // 8레벨에서의 확률
    {10, 20, 25, 35, 10}, // 9레벨에서의 확률
    {5, 10, 20, 40, 25}   // 10레벨에서의 확률
};

/// <summary>
/// 플레이어 레벨에 따라 무작위로 코스트를 선택합니다.
/// </summary>
/// <param name="playerLevel">플레이어 레벨 (1~10)</param>
/// <returns>선택된 코스트 (1~5)</returns>
private int GetRandomCostByPlayerLevel(int playerLevel)
{

    // 해당 레벨의 확률 배열을 가져옴
    int[] probabilities = new int[5]; //길이가 5개인 배열 가져옴
    for (int i = 0; i < 5; i++) //0 ~ 4까지 반복
    {
        probabilities[i] = levelCostProbability[playerLevel - 1, i];
    }

    // 무작위 값 생성 (0부터 99까지)
    int randomValue = Random.Range(0, 100);
    int cumulativeProbability = 0;

    // 누적 확률에 따라 코스트 선택
    for (int i = 0; i < 5; i++)
    {
        cumulativeProbability += probabilities[i];
        if (randomValue < cumulativeProbability)
        {
            return i + 1; // 코스트 값은 1부터 시작하므로 i+1 반환
        }
    }

    return 1; // 기본값 (이론상 도달하지 않음)
}

/// <summary>
/// 특정 cost에 해당하는 챔피언 목록을 반환합니다.
/// </summary>
/// <param name="cost">챔피언의 cost 값 (1~5)</param>
/// <returns>해당 cost에 해당하는 챔피언 목록</returns>
public List<Champion> GetChampionsByCost(int cost)
{
    List<Champion> championsByCost = new List<Champion>();

    foreach (var champion in gameData.championsArray) // 모든 챔피언을 순회하며 해당 cost와 일치하는 챔피언을 찾는다.
    {
        if (champion.cost == cost)
        {
            championsByCost.Add(champion);
        }
    }

    return championsByCost;
}

/// <summary>
/// 특정 cost 범위 내에서 무작위 챔피언을 반환합니다.
/// </summary>
/// <param name="cost">챔피언의 cost 값 (1~5)</param>
/// <returns>무작위로 선택된 챔피언</returns>
public Champion GetRandomChampionByCost(int cost)
{
    List<Champion> championsByCost = GetChampionsByCost(cost); // 특정 cost에 해당하는 챔피언 목록을 가져온다.
    int rand = Random.Range(0, championsByCost.Count); // 챔피언 목록에서 무작위 인덱스를 선택.
    return championsByCost[rand]; // 무작위로 선택된 챔피언을 반환한다.
}

/// <summary>
/// 플레이어 레벨에 따라 무작위 챔피언을 반환합니다.
/// </summary>
/// <param name="playerLevel">플레이어 레벨 (1~10)</param>
/// <returns>무작위로 선택된 챔피언</returns>
public Champion GetRandomChampionByPlayerLevel(int playerLevel)
{
    int cost = GetRandomCostByPlayerLevel(playerLevel); // 플레이어 레벨에 따라 무작위 cost를 결정.
    return GetRandomChampionByCost(cost); // 결정된 cost에 해당하는 무작위 챔피언을 반환.
}
}
