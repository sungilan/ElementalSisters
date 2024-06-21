using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameStage { Preparation, Combat, Loss};

/// <summary>
/// 대부분의 게임 로직과 플레이어 상호작용을 제어합니다.
/// </summary>
public class GamePlayController : MonoBehaviour
{
    [SerializeField]private Map map;
    [SerializeField]private InputController inputController;
    [SerializeField]private GameData gameData;
    [SerializeField]private UIController uIController;
    [SerializeField]private AIopponent aIopponent;
    [SerializeField]private ChampionShop championShop;

    [HideInInspector]
    public GameObject[] ownChampionInventoryArray;
    [HideInInspector]
    public GameObject[] oponentChampionInventoryArray;
    [HideInInspector]
    public GameObject[,] gridChampionsArray;

    public GameObject starPrefab;
    [SerializeField]private Transform playerPos;
    [SerializeField]private Transform playerPreparationPos1;
    [SerializeField]private Transform playerPreparationPos2;
    [SerializeField]private Transform playerCombatPos;

    public GameStage currentGameStage;
    public float timer = 0;

    ///준비단계 지속 시간(챔피언을 배치할 수 있는 시간)
    public int PreparationStageDuration = 30;
    ///전투 단계가 지속될 수 있는 최대 시간
    public int CombatStageDuration = 60;
    ///매 라운드마다 얻을 수 있는 기본 골드 가치
    public int baseGoldIncome = 5;
    //라운드 카운트 숫자
    public int roundCount;

    [HideInInspector]
    public int currentChampionLimit = 3; //현재 챔피언 제한(초기값 : 3) 
    [HideInInspector]
    public int currentChampionCount = 0; //현재 챔피언 수
    [HideInInspector]
    public int currentGold = 5; //현재 골드
    [HideInInspector]
    public int currentLevel = 1;
    [HideInInspector]
    public int maxHP = 100; //최대 플레이어 체력
    [HideInInspector]
    public int currentHP = 100; //현재 플레이어 체력
    [HideInInspector]
    public int timerDisplay = 0;

    public Dictionary<ChampionType, int> championTypeCount;
    public List<ChampionBonus> activeBonusList;

    public CreateSpecialCard createSpecialCard;

    /// Start is called before the first frame update
    void Start()
    {
        //set starting gamestage
        currentGameStage = GameStage.Preparation; //현재게임스테이지 = 준비단계로 초기화

        //배열 초기화
        ownChampionInventoryArray = new GameObject[Map.inventorySize]; //아군 인벤토리 배열(9개)
        oponentChampionInventoryArray = new GameObject[Map.inventorySize]; //적군 인벤토리 배열(9개)
        gridChampionsArray = new GameObject[Map.hexMapSizeX, Map.hexMapSizeZ / 2]; //챔피언 배치 그리드 배열(hexMapSizeX(7),hexMapSizeZ / 2(4))
        roundCount = 1;
        uIController.UpdateUI(); //UI 업데이트
    }

    /// Update is called once per frame
    void Update()
    {
        //manage game stage
        if (currentGameStage == GameStage.Preparation) // 현재 스테이지가 준비단계면
        {
            timer += Time.deltaTime; //타이머 시작

            timerDisplay = (int) (PreparationStageDuration - timer);

            uIController.UpdateTimerText();

            if (timer > PreparationStageDuration) //배치 시간을 타이머가 초과하면
            {
                timer = 0; //타이머의 수치를 0으로 되돌리고

                OnGameStageComplate(); //
            }
        }
        else if (currentGameStage == GameStage.Combat) // 현재스테이지가 전투단계이면
        {
            timer += Time.deltaTime; //타이머 시작

            timerDisplay = (int)timer;

            uIController.UpdateTimerText();

            if (timer > CombatStageDuration) //전투 단계 시간을 타이머가 초과하면
            {
                timer = 0; //타이머의 수치를 0으로 되돌리고

                OnGameStageComplate();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 플레이어의 위치를 새로운 위치로 변경합니다.
            playerPos.transform.position = playerPreparationPos1.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 플레이어의 위치를 새로운 위치로 변경합니다.
            playerPos.transform.position = playerPreparationPos2.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 플레이어의 위치를 새로운 위치로 변경합니다.
            playerPos.transform.position = playerCombatPos.transform.position;
        }
    }




    /// <summary>
    /// 상점에서 챔피언을 구매하여 인벤토리에 추가합니다.
    /// </summary>
    /// /// <param name="champion">구매할 챔피언입니다.</param>
    /// <returns>구매에 성공하면 true를 반환합니다. 실패할 경우 false를 반환합니다.</returns>
    public bool BuyChampionFromShop(Champion champion)
    {
        //인벤토리 배열에서 빈 슬롯을 찾습니다.
        int emptyIndex = -1;
        for (int i = 0; i < ownChampionInventoryArray.Length; i++) //아군 인벤토리 배열의 길이만큼 반복
        {
            if(ownChampionInventoryArray[i] == null)
            {
                emptyIndex = i;
                break;
            }
        }

        //챔피언을 추가할 슬롯이 없으면 false 반환
        if (emptyIndex == -1)
            return false;

        //챔피언을 구매하기에 충분한 골드가 없으면 false 반환
        if (currentGold < champion.cost) //현재 골드가 챔피언의 가격보다 적으면
            return false;

        //챔피언 프리팹 생성
        GameObject championPrefab = Instantiate(champion.prefab);

        // 챔피언 컨트롤러를 가져옵니다.
        ChampionController championController = championPrefab.GetComponent<ChampionController>();

        // 챔피언 컨트롤러를 초기화합니다.
        championController.Init(champion, ChampionController.TEAMID_PLAYER);

        // 그리드 위치를 설정합니다.
        championController.SetGridPosition(Map.GRIDTYPE_OWN_INVENTORY, emptyIndex, -1);
        
        // 위치와 회전을 설정합니다.
        championController.SetWorldPosition();
        championController.SetWorldRotation();


        //인벤토리 배열에 챔피언 저장
        StoreChampionInArray(Map.GRIDTYPE_OWN_INVENTORY, map.ownTriggerArray[emptyIndex].gridX, -1, championPrefab);

       

       
        //preparation stage에서만 업그레이드 가능
        if(currentGameStage == GameStage.Preparation) //현재 스테이지가 준비 스테이지일 때
            TryUpgradeChampion(champion); //업그레이드가 가능하면 업그레이드 실행


        //골드 지출
        currentGold -= champion.cost; //현재 골드에서 챔피언 가격만큼 차감

        //골드 UI 업데이트
        uIController.UpdateUI();

        //구매에 성공했을 때 true 반환
        return true;
    }


    /// <summary>
    /// 챔피언을 업그레이드할 수 있는지 확인하고, 업그레이드합니다.
    /// </summary>
    /// <param name="champion">업그레이드할 챔피언입니다.</param>
    private void TryUpgradeChampion(Champion champion)
    {
        // 업그레이드 가능한 챔피언을 저장할 리스트를 초기화합니다.
        List<ChampionController> championList_lvl_1 = new List<ChampionController>();
        List<ChampionController> championList_lvl_2 = new List<ChampionController>();

        // 아군 인벤토리 배열을 검사하여 업그레이드 가능한 챔피언을 찾습니다.
        for (int i = 0; i < ownChampionInventoryArray.Length; i++) //아군 인벤토리 배열의 길이만큼 반복
        {
            //there is a champion
            if (ownChampionInventoryArray[i] != null) //아군 인벤토리 배열이 null이 아니면(인벤토리 안에 챔피언이 있을 경우)
            {
                // 인벤토리 내의 챔피언 컨트롤러를 가져옵니다.
                ChampionController championController = ownChampionInventoryArray[i].GetComponent<ChampionController>();

                //업그레이드할 챔피언(champion)이 인벤토리 안의 챔피언(championController.champion)과 동일한 종류의 챔피언이면 리스트에 추가합니다.
                if (championController.champion == champion)
                {
                    if (championController.lvl == 1) //인벤토리 안의 챔피언이 1레벨일 경우
                        championList_lvl_1.Add(championController); //1레벨 리스트에 추가
                    else if (championController.lvl == 2) //인벤토리 안의 챔피언 2레벨일 경우
                        championList_lvl_2.Add(championController); //2레벨 리스트에 추가
                }
            }

        }

        // 전장에 배치된 챔피언을 검사하여 업그레이드 가능한 챔피언을 찾습니다.
        for (int x = 0; x < Map.hexMapSizeX; x++) //hexMapSizeX만큼 반복
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //hexMapSizeZ의 반만큼 반복(아군 진영)
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null)
                {
                    // 전장의 챔피언 컨트롤러를 가져옵니다.
                    ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();

                    // 업그레이드할 챔피언이 전장의 챔피언과 동일한 종류의 챔피언이면 리스트에 추가합니다.
                    if (championController.champion == champion)
                    {
                        if (championController.lvl == 1) //전장의 챔피언이 1레벨일 경우
                            championList_lvl_1.Add(championController); //1레벨 리스트에 추가
                        else if (championController.lvl == 2) //전장의 챔피언이 1레벨일 경우
                            championList_lvl_2.Add(championController); 
                    }
                }

            }
        }

        //리스트를 확인하여 일정수 이상 있으면 그 챔피언을 업그레이드하고 나머지는 삭제합니다.
        if (championList_lvl_1.Count > 2) // 1성 챔피언이 2 이상(3) 있을 때
        {
            // 2레벨로 업그레이드합니다.
            championList_lvl_1[2].UpgradeLevel();

            // 업그레이드한 챔피언을 배열에서 제거합니다.
            RemoveChampionFromArray(championList_lvl_1[0].gridType, championList_lvl_1[0].gridPositionX, championList_lvl_1[0].gridPositionZ);
            RemoveChampionFromArray(championList_lvl_1[1].gridType, championList_lvl_1[1].gridPositionX, championList_lvl_1[1].gridPositionZ);

            // 업그레이드한 챔피언 게임오브젝트를 파괴합니다.
            Destroy(championList_lvl_1[0].gameObject);
            Destroy(championList_lvl_1[1].gameObject);

            // 3레벨로 업그레이드합니다.
            if (championList_lvl_2.Count > 1) // 2성 챔피언이 1 이상(2) 있을 때
            {
                //upgrade
                championList_lvl_1[2].UpgradeLevel();

                // 업그레이드한 챔피언을 배열에서 제거합니다.
                RemoveChampionFromArray(championList_lvl_2[0].gridType, championList_lvl_2[0].gridPositionX, championList_lvl_2[0].gridPositionZ);
                RemoveChampionFromArray(championList_lvl_2[1].gridType, championList_lvl_2[1].gridPositionX, championList_lvl_2[1].gridPositionZ);

                // 업그레이드한 챔피언 게임오브젝트를 파괴합니다.
                Destroy(championList_lvl_2[0].gameObject);
                Destroy(championList_lvl_2[1].gameObject);
            }
        }


        // 현재 챔피언 수를 업데이트합니다.
        currentChampionCount = GetChampionCountOnHexGrid();

        // UI를 업데이트합니다.
        uIController.UpdateUI();

    }

    private GameObject draggedChampion = null;
    public TriggerInfo dragStartTrigger = null;

    /// <summary>
    /// 지도에서 챔피언을 드래그하기 시작하면 호출
    /// </summary>
    public void StartDrag()
    {
        // 현재 게임 스테이지가 준비 스테이지가 아니면 드래그를 불가능하게 합니다.
        if (currentGameStage != GameStage.Preparation) //현재 스테이지가 준비 스테이지가 아니면
            return; // 빠져나온다(드래그 불가)

        // 트리거 정보를 가져옵니다.
        TriggerInfo triggerinfo = inputController.triggerInfo;
        
        //마우스 커서가 트리거 위에 있으면
        if (triggerinfo != null) //triggerinfo가 null이 아니면
        { 
            dragStartTrigger = triggerinfo;

            // 트리거 정보에서 챔피언을 가져옵니다.
            GameObject championGO = GetChampionFromTriggerInfo(triggerinfo);
            
            if(championGO != null)
            {
                // 인디케이터를 표시합니다.
                map.ShowIndicators();

                draggedChampion = championGO;

                // 챔피언이 드래그되고 있는 상태를 설정합니다.
                championGO.GetComponent<ChampionController>().IsDragged = true;
                //Debug.Log("STARTDRAG");
            }

        }
    }

    /// <summary>
    /// 지도에서 챔피언 드래그를 중단하면 호출
    /// </summary>
    public void StopDrag()
    {
        // 인디케이터를 숨깁니다.
        map.HideIndicators();

        // 현재 전투지역에 있는 챔피언의 수를 가져옵니다.
        int championsOnField = GetChampionCountOnHexGrid();


         if (draggedChampion != null)
         {
            // 드래그된 챔피언의 드래그 상태를 해제합니다.
            draggedChampion.GetComponent<ChampionController>().IsDragged = false;

            // 트리거 정보를 가져옵니다.
            TriggerInfo triggerinfo = inputController.triggerInfo;

            // 마우스 커서가 트리거 위에 있는지 확인합니다.
            if (triggerinfo != null)
            {
                // 마우스 커서 위치의 챔피언을 가져옵니다.
                GameObject currentTriggerChampion = GetChampionFromTriggerInfo(triggerinfo);

                // 다른 챔피언이 마우스 커서 위치에 있는지 확인합니다.
                if (currentTriggerChampion != null) 
                {
                    // 챔피언을 이동할 위치에 다른 챔피언이 있는 경우
                    //store this champion to start position
                    StoreChampionInArray(dragStartTrigger.gridType, dragStartTrigger.gridX, dragStartTrigger.gridZ, currentTriggerChampion);

                    //store this champion to dragged position
                    StoreChampionInArray(triggerinfo.gridType, triggerinfo.gridX, triggerinfo.gridZ, draggedChampion);
                }
                else
                {
                    // 드래그된 챔피언을 새 위치에 배치합니다.
                    // 전투지역에 추가하는 경우
                    if (triggerinfo.gridType == Map.GRIDTYPE_HEXA_MAP) //드래그하여 옮긴 곳이 HEXA_MAP(전투지역)일 경우
                    {
                        // 빈 자리가 있거나 전투지역에서 추가하는 경우에만 추가합니다.
                        if (championsOnField < currentChampionLimit || dragStartTrigger.gridType == Map.GRIDTYPE_HEXA_MAP)
                        {
                            // 드래그된 챔피언을 이전 위치에서 제거합니다.
                            RemoveChampionFromArray(dragStartTrigger.gridType, dragStartTrigger.gridX, dragStartTrigger.gridZ);

                            // 드래그된 챔피언을 새 위치에 추가합니다.
                            StoreChampionInArray(triggerinfo.gridType, triggerinfo.gridX, triggerinfo.gridZ, draggedChampion);

                            // 이전 위치가 전투지역이 아니었다면 전투지역에 추가되었으므로 챔피언 수를 증가시킵니다.
                            if (dragStartTrigger.gridType != Map.GRIDTYPE_HEXA_MAP)
                                championsOnField++;
                        }
                    }
                    // 아군 인벤토리에 추가하는 경우
                    else if (triggerinfo.gridType == Map.GRIDTYPE_OWN_INVENTORY) //드래그하여 옮긴 곳이 OWN_INVENTORY(아군 인벤토리)일 경우
                    {
                        // 드래그된 챔피언을 이전 위치에서 제거합니다.
                        RemoveChampionFromArray(dragStartTrigger.gridType, dragStartTrigger.gridX, dragStartTrigger.gridZ);

                        // 드래그된 챔피언을 새 위치에 추가합니다.
                        StoreChampionInArray(triggerinfo.gridType, triggerinfo.gridX, triggerinfo.gridZ, draggedChampion);

                        // 이전 위치가 전투지역이었다면 아군 인벤토리에 추가되었으므로 챔피언 수를 감소시킵니다.
                        if (dragStartTrigger.gridType == Map.GRIDTYPE_HEXA_MAP)
                            championsOnField--;
                    }
                    // 쓰레기통에 갖다놓는 경우
                    else if (triggerinfo.gridType == Map.GRIDTYPE_TRASH_CAN) //드래그하여 옮긴 곳이 GRIDTYPE_TRASH_CAN(쓰레기통)일 경우
                    {
                        // 챔피언 제거
                        Destroy(draggedChampion);
                        ChampionController championController = draggedChampion.GetComponent<ChampionController>();
                        if (championController != null)
                        {
                            currentGold += championController.champion.cost;
                            StartCoroutine(saleText());
                            Debug.Log("챔피언" + draggedChampion + "을 팔아" + championController.champion.cost + "원이 증가했습니다.");
                        }
                    }
                    // 합성기에 갖다놓는 경우
                    else if (triggerinfo.gridType == Map.GRIDTYPE_SYNTHESIZER) //드래그하여 옮긴 곳이 GRIDTYPE_SYNTHESIZER(합성기)일 경우
                    {
                        ChampionController championController = draggedChampion.GetComponent<ChampionController>();
                        ChampionCombination championCombination = GetComponent<ChampionCombination>();
                        if (championController != null && championCombination != null)
                        {
                            Debug.Log("합성기");
                            
                            championCombination.CreateNewChampion(draggedChampion);
                        }
                    }

                }




            }

            // 보너스를 계산합니다.
            CalculateBonuses();

            // 현재 챔피언 수를 업데이트합니다.
            currentChampionCount = GetChampionCountOnHexGrid();

            // UI를 업데이트합니다.
            uIController.UpdateUI();

            // 드래그된 챔피언을 null로 설정합니다.
            draggedChampion = null;
        }

       
    }


    /// <summary>
    /// triggerinfo에서 챔피언 게임오브젝트를 받아옵니다.
    /// </summary>
    /// <param name="triggerinfo"></param>
    /// <returns></returns>
    public GameObject GetChampionFromTriggerInfo(TriggerInfo triggerinfo)
    {
        GameObject championGO = null;

        if (triggerinfo.gridType == Map.GRIDTYPE_OWN_INVENTORY) //triggerinfo의 gridType이 Map.GRIDTYPE_OWN_INVENTORY일 경우
        {
            championGO = ownChampionInventoryArray[triggerinfo.gridX];
        }
        else if (triggerinfo.gridType == Map.GRIDTYPE_OPONENT_INVENTORY) //triggerinfo의 gridType이 Map.GRIDTYPE_OPONENT_INVENTORY일 경우
        {
            championGO = oponentChampionInventoryArray[triggerinfo.gridX];
        }
        else if (triggerinfo.gridType == Map.GRIDTYPE_HEXA_MAP) //triggerinfo의 gridType이 Map.GRIDTYPE_HEXA_MAP일 경우
        {
            championGO = gridChampionsArray[triggerinfo.gridX, triggerinfo.gridZ];
        }

        return championGO;
    }

    /// <summary>
    /// 챔피언 게임오브젝트를 배열에 저장합니다.
    /// </summary>
    /// <param name="triggerinfo"></param>
    /// <param name="champion"></param>
    public void StoreChampionInArray(int gridType, int gridX, int gridZ, GameObject champion)
    {
        //현재 트리거를 챔피언에 할당합니다.
        ChampionController championController = champion.GetComponent<ChampionController>();
        championController.SetGridPosition(gridType, gridX, gridZ);

        if (gridType == Map.GRIDTYPE_OWN_INVENTORY)
        {
            ownChampionInventoryArray[gridX] = champion;
        }    
        else if (gridType == Map.GRIDTYPE_HEXA_MAP)
        {
            gridChampionsArray[gridX, gridZ] = champion;
        }
    }

    /// <summary>
    /// 배열에서 챔피언을 제거합니다.
    /// </summary>
    /// <param name="triggerinfo"></param>
    public void RemoveChampionFromArray(int type, int gridX, int gridZ)
    {
        if (type == Map.GRIDTYPE_OWN_INVENTORY)
        {
            ownChampionInventoryArray[gridX] = null;
        }
        else if (type == Map.GRIDTYPE_HEXA_MAP)
        {
            gridChampionsArray[gridX, gridZ] = null;
        }
    }

    /// <summary>
    /// 지도에 있는 우리 챔피언의 수를 계산하여 반환합니다.
    /// </summary>
    /// <returns></returns>
    private int GetChampionCountOnHexGrid()
    {
        int count = 0;
        for (int x = 0; x < Map.hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ의 반만큼 반복
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null) //gridChampionsArray[x, z]이 null이 아니면(챔피언이 있으면)
                {
                    count++; //count 추가
                }
            }
        }

        return count; //count를 반환
    }

    /// <summary>
    /// 현재 가지고 있는 보너스를 계산합니다.
    /// </summary>
    private void CalculateBonuses()
    {
        // 챔피언 종류와 수량을 저장할 딕셔너리 초기화
        championTypeCount = new Dictionary<ChampionType, int>();

        for (int x = 0; x < Map.hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ의 반만큼 반복
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null) //gridChampionsArray[x, z]이 null이 아니면(챔피언이 있으면)
                {
                    // 셀에 챔피언이 존재하는 경우
                    // 챔피언 가져오기
                    Champion c = gridChampionsArray[x, z].GetComponent<ChampionController>().champion;

                    // 첫 번째 챔피언 종류 카운트 업데이트
                    if (championTypeCount.ContainsKey(c.type1))
                    {
                        int cCount = 0;
                        championTypeCount.TryGetValue(c.type1, out cCount);

                        cCount++;

                        championTypeCount[c.type1] = cCount;
                    }
                    else
                    {
                        championTypeCount.Add(c.type1, 1);
                    }

                    // 두 번째 챔피언 종류 카운트 업데이트
                    if (championTypeCount.ContainsKey(c.type2))
                    {
                        int cCount = 0;
                        championTypeCount.TryGetValue(c.type2, out cCount);

                        cCount++;

                        championTypeCount[c.type2] = cCount;
                    }
                    else
                    {
                        championTypeCount.Add(c.type2, 1);
                    }

                     // 세 번째 챔피언 종류 카운트 업데이트
                    if (championTypeCount.ContainsKey(c.type3))
                    {
                        int cCount = 0;
                        championTypeCount.TryGetValue(c.type3, out cCount);

                        cCount++;

                        championTypeCount[c.type3] = cCount;
                    }
                    else
                    {
                        championTypeCount.Add(c.type3, 1);
                    }

                }
            }
        }

        // 활성 보너스 목록 초기화
        //activeBonusList = new List<ChampionBonus>();

        // 각 챔피언 종류별로 보너스 계산
        foreach (KeyValuePair<ChampionType, int> m in championTypeCount)
        {
            // 챔피언 종류에 해당하는 보너스 가져오기
            ChampionBonus championBonus = m.Key.championBonus;

            // 보너스 획득을 위해 충분한 챔피언 수 확인
            if (m.Value >= championBonus.championCount)
            {
                // 충분한 챔피언 수가 있으면 보너스를 활성 목록에 추가
                activeBonusList.Add(championBonus);
            }
        }
    }
    public void ApplyChampionBonus(ChampionBonus bonus)
{
    if (!activeBonusList.Contains(bonus))
    {
        activeBonusList.Add(bonus);
        Debug.Log("특성 적용");
    }
}

    /// <summary>
    /// 모든 챔피언의 스탯과 위치를 재설정합니다.
    /// </summary>
    private void ResetChampions()
    {
        for (int x = 0; x < Map.hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ의 반만큼 반복
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null) //gridChampionsArray[x, z]이 null이 아니면(챔피언이 있으면)
                {
                    //get character
                    ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>(); //gridChampionsArray[x, z]의 ChampionController를 받아옴

                    //reset
                    championController.Reset();
                }

            }
        }
    }

    /// <summary>
    /// 게임스테이지가 끝났을 때 호출
    /// </summary>
    public void OnGameStageComplate()
    {
        //ai에게 이 스테이지가 끝났음을 알림
        aIopponent.OnGameStageComplate(currentGameStage);

        if (currentGameStage == GameStage.Preparation) // 현재스테이지가 준비단계이면
        {
            roundCount += 1;
            //set new game stage
            currentGameStage = GameStage.Combat; // 현재스테이지를 전투단계로

            //show indicators
            map.HideIndicators(); //모든 지도표시를 숨김

            //hide timer text
            //uIController.SetTimerTextActive(false); //타이머 텍스트를 숨김


            if (draggedChampion != null) //드래그챔피언이 null이 아니면
            {
                //stop dragging 드래그를 멈춤
                draggedChampion.GetComponent<ChampionController>().IsDragged = false;
                draggedChampion = null;
            }


            for (int i = 0; i < ownChampionInventoryArray.Length; i++)
            {
                //there is a champion
                if (ownChampionInventoryArray[i] != null)
                {
                    //get character
                    ChampionController championController = ownChampionInventoryArray[i].GetComponent<ChampionController>();

                    //start combat
                    championController.OnCombatStart();
                }
            }

            //start own champion combat 
            for (int x = 0; x < Map.hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
            {
                for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ의 반만큼 반복
                {
                    //there is a champion
                    if (gridChampionsArray[x, z] != null)
                    {
                        //get character
                        ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();

                        //start combat
                        championController.OnCombatStart();
                    }

                }
            }


            //check if we start with 0 champions
            if (IsAllChampionDead())
                EndRound();
           

        }
        else if (currentGameStage == GameStage.Combat) // 현재스테이지가 전투단계이면
        {
            //set new game stage
            currentGameStage = GameStage.Preparation; // 현재스테이지를 준비단계로

            createSpecialCard.SetSpecialCard();

            //show timer text
            //uIController.SetTimerTextActive(true);

            //reset champion
            ResetChampions();

            //go through all champion infos
            for (int i = 0; i < gameData.championsArray.Length; i++)
            {
                TryUpgradeChampion(gameData.championsArray[i]);
            }


            //add gold
            currentGold += CalculateIncome();

            //set gold ui
            uIController.UpdateUI();

            //refresh shop ui
            championShop.RefreshShop(true);

            //check if we have lost
            if (currentHP <= 0)
            {
                currentGameStage = GameStage.Loss;
                uIController.ShowLossScreen();
             
            }
            
        }
    }

    /// <summary>
    /// 우리가 받아야 할 금의 수를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private int CalculateIncome()
    {
        int income = 0;

        //banked gold
        int bank = (int)(currentGold / 10);

       
        income += baseGoldIncome;
        income += bank;

        return income;
    }

    /// <summary>
    /// 사용 가능한 챔피언 슬롯을 1개 늘립니다.
    /// </summary>
    public void Buylvl()
    {
        //골드가 부족하면 이 함수를 빠져나옵니다.
        if (currentGold < 4) //현재 골드가 4보다 적으면
            return;

        if(currentChampionLimit < 9) //현재 챔피언 제한이 9(최대)보다 적으면
        {
            currentLevel += 1;
            //챔피언 보유 제한을 늘립니다.
            currentChampionLimit++;

            //4 골드를 지출합니다.
            currentGold -= 4;

            //ui를 업데이트합니다.
            uIController.UpdateUI();
        
        }
      
    }

    /// <summary>
    /// 라운드에서 졌을 때 호출
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        uIController.backHpHit = true;
        uIController.UpdateUI();

    }

    /// <summary>
    /// 게임에서 졌을 때 호출
    /// </summary>
    public void RestartGame()
    {
       
       

        //챔피언을 제거합니다.
        for (int i = 0; i < ownChampionInventoryArray.Length; i++)
        {
            //there is a champion
            if (ownChampionInventoryArray[i] != null)
            {
                //get character
                ChampionController championController = ownChampionInventoryArray[i].GetComponent<ChampionController>();

                Destroy(championController.gameObject);
                ownChampionInventoryArray[i] = null;
            }

        }

        for (int x = 0; x < Map.hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ의 반만큼 반복
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null)
                {
                    //get character
                    ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();

                    Destroy(championController.gameObject);
                    gridChampionsArray[x, z] = null;
                }

            }
        }

        //스탯을 리셋합니다.
        currentHP = 100;
        currentGold = 5;
        currentGameStage = GameStage.Preparation;
        currentChampionLimit = 3;
        currentChampionCount = GetChampionCountOnHexGrid();

        uIController.UpdateUI();

        //restart ai
        aIopponent.Restart();

        //show hide ui
        uIController.ShowGameScreen();
   

    }


    /// <summary>
    /// 라운드를 끝냅니다.
    /// </summary>
    public void EndRound()
    {
        timer = CombatStageDuration - 3; //reduce timer so game ends fast
    }


    /// <summary>
    /// 챔피언이 죽었을 때 호출
    /// </summary>
    public void OnChampionDeath()
    {
        bool allDead = IsAllChampionDead();

        if (allDead)
            EndRound();
    }


    /// <summary>
    /// 모든 챔피언이 죽으면 true를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private bool IsAllChampionDead()
    {
        int championCount = 0;
        int championDead = 0;
        //start own champion combat
        for (int x = 0; x < Map.hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ의 반만큼 반복
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null) //gridChampionsArray[x, z]이 null이 아니면(챔피언이 있으면)
                {
                    //get character
                    ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();


                    championCount++;

                    if (championController.isDead)
                        championDead++;

                }

            }
        }

        if (championDead == championCount)
            return true;

        return false;

    }
    /// <summary>
    /// 조합법 버튼을 누를 때마다 조합법이 활성/비활성화됩니다.
    /// </summary>
    public void RecipeButton()
    {
        if (uIController.recipeUIPrefab != null)
        {
            if (uIController.recipeUIPrefab.activeSelf)
            {
                uIController.recipeUIPrefab.SetActive(false);
            }
            else
            {
                uIController.recipeUIPrefab.SetActive(true);
            }
        }
    }
    IEnumerator saleText()
    {
        ChampionController championController = draggedChampion.GetComponent<ChampionController>();
        Champion champion = championController.champion;
        uIController.saleUIPrefab.SetActive(true);
        uIController.saleText.text = "챔피언 " + champion.uiname + "을/를 팔아 " + championController.champion.cost + "원이 증가했습니다.";
        yield return new WaitForSeconds(2);
        uIController.saleUIPrefab.SetActive(false);
    }
    
}
