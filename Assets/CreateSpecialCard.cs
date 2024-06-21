using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사용 가능한 챔피언 생성 및 저장, 경험치 및 레벨 구매
/// </summary>
public class CreateSpecialCard : MonoBehaviour
{
    public UIController uIController; // UI 컨트롤러
    public GamePlayController gamePlayController; // 게임 플레이 컨트롤러
    public GameData gameData; // 게임 데이터
    public GameObject specialCardPrefab; // 스페셜 카드 프리팹
    public Transform[] spawnPos; 

    /// 구매할 수 있는 챔피언을 저장하는 배열
    public SpecialCard[] availableSpecialCardArray;

    private List<GameObject> instantiatedCards = new List<GameObject>();

    /// Start is called before the first frame update
    void Start()
    {
        SetSpecialCard();
    }

    /// Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 새로운 무작위 챔피언으로 상점을 새로 고칩니다.
    /// </summary>
    public void SetSpecialCard()
    {
        // 배열 초기화
        availableSpecialCardArray = new SpecialCard[3];

        // 상점 채우기
        for (int i = 0; i < availableSpecialCardArray.Length; i++)
        {
            // 무작위 챔피언 가져오기
            SpecialCard specialCard = GetRandomSpecialCardInfo();
           
            // 챔피언을 배열에 저장
            availableSpecialCardArray[i] = specialCard;
            GameObject specialCardGO = Instantiate(specialCardPrefab, spawnPos[i].transform.position, Quaternion.identity);
            specialCardGO.GetComponent<SpecialCard>().championBonus = specialCard.championBonus; // 스페셜 카드의 특성 설정
            //원래 위치 저장
            Vector3 originalPosition = specialCardGO.transform.position;
            Quaternion originalRotation = specialCardGO.transform.rotation;
            //부모 설정
            specialCardGO.transform.SetParent(spawnPos[i]);
            //부모의 좌표 공간으로 이동하여 변경된 위치와 회전을 원래대로 되돌림
            specialCardGO.transform.position = originalPosition;
            specialCardGO.transform.rotation = originalRotation;
            instantiatedCards.Add(specialCardGO);

            // UI에 챔피언 로드
            uIController.LoadSpecialCard(specialCard, i);
          
            // 상점 아이템 표시
            //uIController.ShowShopItems();
        }

        // 골드 차감
        //if (isFree == false)
            //gamePlayController.currentGold -= 2;

        // UI 업데이트
        uIController.UpdateUI();
    }

    /// <summary>
    /// UI 챔피언 프레임을 클릭했을 때 호출됩니다.
    /// </summary>
    /// <param name="index">클릭한 챔피언 프레임의 인덱스입니다.</param>
    //public void OnChampionFrameClicked(int index)
    //{
        //bool isSuccess = gamePlayController.BuyChampionFromShop(availableSpecialCardArray[index]);

        //if (isSuccess)
            //uIController.HideChampionFrame(index);
    //}

     public void DestroyAllSpecialCards()
    {
        foreach (GameObject card in instantiatedCards)
        {
            Destroy(card);
        }
        instantiatedCards.Clear();
    }

    /// <summary>
    /// 무작위 챔피언을 반환합니다.
    /// </summary>
    public SpecialCard GetRandomSpecialCardInfo()
    {
        // 무작위 번호 생성
        int rand = Random.Range(0, gameData.specialCardsArray.Length);

        // 배열에서 반환
        return gameData.specialCardsArray[rand];

    }
}
