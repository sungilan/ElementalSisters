using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 챔피언을 제어합니다.
/// </summary>
public class AIopponent : MonoBehaviour
{
    public ChampionShop championShop;
    public Map map;
    public UIController uIController;
    public GamePlayController gamePlayController;

    public GameObject[,] gridChampionsArray;

    public Dictionary<ChampionType, int> championTypeCount;
    public List<ChampionBonus> activeBonusList;

    ///라운드에서 패배할 때 플레이어가 받는 피해
    public int championDamage = 2;

    /// <summary>
    /// 맵이 생성될 때 호출됩니다.
    /// </summary>
    public void OnMapReady()
    {
        gridChampionsArray = new GameObject[Map.hexMapSizeX, Map.hexMapSizeZ / 2]; //그리드 챔피언 배열 : 맵의 반만큼

        AddRandomChampion();
       // AddRandomChampion();
    }

    /// <summary>
    /// 스테이지가 끝났을 때 호출됩니다.
    /// </summary>
    /// <param name="stage"></param>
    public void OnGameStageComplate(GameStage stage)
    {
        if (stage == GameStage.Preparation) //현재 스테이지가 준비단계일 때(준비단계가 끝나면 호출)
        {
            //챔피언이 공격을 시작합니다.(공격단계 시작)
            for (int x = 0; x < Map.hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
            {
                for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ의 반만큼 반복
                {
                    //there is a champion
                    if (gridChampionsArray[x, z] != null) //챔피언 배열이 null이 아닐 경우(공격할 챔피언이 맵에 존재)
                    {
                        //챔피언 배열에서 챔피언 컨트롤러를 받아옵니다.
                        ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();

                        //챔피언 공격 시작
                        championController.OnCombatStart();
                    }

                }
            }
        }

        if (stage == GameStage.Combat) //현재 스테이지가 공격단계일 때(공격단계가 끝나면 호출)
        {
            //플레이어가 입을 총 데미지
            int damage = 0;

            //iterate champions
            //start champion combat
            for (int x = 0; x < Map.hexMapSizeX; x++)
            {
                for (int z = 0; z < Map.hexMapSizeZ / 2; z++)
                {
                    //there is a champion
                    if (gridChampionsArray[x, z] != null) //챔피언 배열이 null이 아닐 경우(공격할 챔피언이 맵에 존재)
                    {
                        //챔피언 배열에서 챔피언 컨트롤러를 받아옵니다.
                        ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();

                        ////생존한 챔피언의 수에 비례하여 플레이어가 입을 데미지를 계산합니다.
                        if (championController.currentHealth > 0) //챔피언의 현재 체력이 0보다 클 경우
                            damage += championDamage; //플레이어가 입을 데미지에 챔피언의 데미지를 추가합니다.
                        //플레이어가 damage만큼 피해를 입습니다.
                        gamePlayController.TakeDamage(damage);
                    }
                }
            }

            ResetChampions(); //다음 라운드를 위해 챔피언을 리셋하고


            AddRandomChampion();  //무작위 챔피언을 추가합니다.
        }
    }

    /// <summary>
    /// 지도 그리드의 빈 위치를 반환합니다.
    /// </summary>
    /// <param name="emptyIndexX"></param>
    /// <param name="emptyIndexZ"></param>
    private void GetEmptySlot(out int emptyIndexX, out int emptyIndexZ)
    {
        emptyIndexX = -1;
        emptyIndexZ = -1;

        //get first empty inventory slot
        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++)
            {
                if (gridChampionsArray[x, z] == null)
                {
                    emptyIndexX = x;
                    emptyIndexZ = z;
                    break;
                }
            }    
        }
    }


    /// <summary>
    /// 새로운 무작위 챔피언을 생성하고 지도에 추가합니다.
    /// </summary>
    public void AddRandomChampion()
    {
        //빈 슬롯을 찾습니다.
        int indexX;
        int indexZ;
        GetEmptySlot(out indexX, out indexZ);

        //빈 슬롯이 없을 경우에는 챔피언을 추가하지 않습니다.
        if (indexX == -1 || indexZ == -1)
            return;

        Champion champion = championShop.GetRandomChampionInfo();

        //챔피언 프리팹 생성
        GameObject championPrefab = Instantiate(champion.prefab);

        //챔피언을 배열에 추가
        gridChampionsArray[indexX, indexZ] = championPrefab;

        //get champion controller
        ChampionController championController = championPrefab.GetComponent<ChampionController>();

        //setup chapioncontroller
        championController.Init(champion, ChampionController.TEAMID_AI);

        //set grid position
        championController.SetGridPosition(Map.GRIDTYPE_HEXA_MAP, indexX, indexZ + 4);

        //set position and rotation
        championController.SetWorldPosition();
        championController.SetWorldRotation();

        //check for champion upgrade
        List<ChampionController> championList_lvl_1 = new List<ChampionController>();
        List<ChampionController> championList_lvl_2 = new List<ChampionController>();

        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++)
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null)
                {
                    //get character
                    ChampionController cc = gridChampionsArray[x, z].GetComponent<ChampionController>();

                    //check if is the same type of champion that we are buying
                    if (cc.champion == champion)
                    {
                        if (cc.lvl == 1)
                            championList_lvl_1.Add(cc);
                        else if (cc.lvl == 2)
                            championList_lvl_2.Add(cc);
                    }
                }

            }
        }

        //if we have 3 we upgrade a champion and delete rest
        if (championList_lvl_1.Count == 3)
        {
            //upgrade
            championList_lvl_1[2].UpgradeLevel();

            //destroy gameobjects
            Destroy(championList_lvl_1[0].gameObject);
            Destroy(championList_lvl_1[1].gameObject);

            //we upgrade to lvl 3
            if (championList_lvl_2.Count == 2)
            {
                //upgrade
                championList_lvl_1[2].UpgradeLevel();

                //destroy gameobjects
                Destroy(championList_lvl_2[0].gameObject);
                Destroy(championList_lvl_2[1].gameObject);
            }
        }


       CalculateBonuses();
    }

    /// <summary>
    /// 그리드에 있는 모든 소유 챔피언을 재설정합니다.
    /// </summary>
    private void ResetChampions()
    {
        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++)
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null)
                {
                    //get character
                    ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();

                    //set position and rotation
                    championController.Reset();



                }

            }
        }
    }

    /// <summary>
    /// 게임을 재시작할 때 호출
    /// </summary>
    public void Restart()
    {
        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) //(맵상의 모든 챔피언에 반복 적용)
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null) //챔피언배열에 챔피언이 존재할 경우
                {
                    //챔피언 컨트롤러를 가져옵니다.
                    ChampionController championController = gridChampionsArray[x, z].GetComponent<ChampionController>();

                    Destroy(championController.gameObject); //챔피언 컨트롤러를 가진 오브젝트(챔피언)을 파괴합니다.
                    gridChampionsArray[x, z] = null; //챔피언 배열을 null로 만듭니다.

                }

            }
        }

        AddRandomChampion(); //무작위 챔피언을 추가합니다.
        //AddRandomChampion();
    }

    /// <summary>
    /// Called when champion health goes belove 0
    /// </summary>
    public void OnChampionDeath()
    {
        bool allDead = IsAllChampionDead();

        if (allDead)
            gamePlayController.EndRound();
    }


    /// <summary>
    /// Checks if all champion is dead
    /// </summary>
    /// <returns></returns>
    private bool IsAllChampionDead()
    {
        int championCount = 0;
        int championDead = 0;
        //start own champion combat
        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++)
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null)
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
    /// 챔피언 보너스 계산
    /// </summary>
    private void CalculateBonuses()
    {
        //championTypeCount 딕셔너리 초기화(챔피언의 타입(ChampionType)을 키로 사용하고, 해당 타입의 챔피언 수를 값으로 저장)
        championTypeCount = new Dictionary<ChampionType, int>();

        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++)
            {
                //there is a champion
                if (gridChampionsArray[x, z] != null) //챔피언 배열에 챔피언이 존재할 경우
                {
                    //get champion
                    Champion c = gridChampionsArray[x, z].GetComponent<ChampionController>().champion;

                    if (championTypeCount.ContainsKey(c.type1)) //championTypeCount 딕셔너리에 현재 챔피언의 첫 번째 타입(c.type1)이 이미 있는지 확인합니다.
                                                                // 이미 해당 타입의 챔피언 수를 기록하고 있는 경우입니다.
                    {
                        // 해당 타입의 챔피언 수를 나타내는 변수를 선언하고 초기화합니다.
                        int cCount = 0;
                        // TryGetValue 메서드를 사용하여 해당 타입의 챔피언 수를 가져옵니다.
                        // TryGetValue 메서드는 찾으려는 키가 딕셔너리에 있는 경우 해당 값을 가져오고, 없는 경우 지정된 변수를 기본값으로 초기화합니다.
                        championTypeCount.TryGetValue(c.type1, out cCount);
                        // 현재 타입의 챔피언 수를 1 증가시킵니다.
                        cCount++;
                        // 딕셔너리에 새로운 값을 설정합니다. 이를 통해 해당 타입의 챔피언 수를 업데이트합니다.
                        championTypeCount[c.type1] = cCount;
                    }
                    else // 해당 타입의 챔피언이 처음 등장하는 경우입니다.
                    {
                        // 딕셔너리에 새로운 항목을 추가합니다.
                        // 키로는 챔피언의 첫 번째 타입을 사용하고, 값으로는 1을 넣어 새로운 타입의 챔피언이 한 명 있다는 것을 표시합니다.
                        championTypeCount.Add(c.type1, 1);
                    }

                    if (championTypeCount.ContainsKey(c.type2)) //위와 동일, type2에 대한 처리
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

                    if (championTypeCount.ContainsKey(c.type3)) //위와 동일, type2에 대한 처리
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
        // 활성 보너스 목록을 초기화합니다.
        activeBonusList = new List<ChampionBonus>();

        // 챔피언 타입별 수를 나타내는 championTypeCount 딕셔너리를 순회합니다.
        foreach (KeyValuePair<ChampionType, int> m in championTypeCount)
        {
            // 현재 타입에 해당하는 보너스를 가져옵니다.
            ChampionBonus championBonus = m.Key.championBonus;

            // 보너스 획득을 위해 충분한 챔피언 수 확인
                if (m.Value >= championBonus.championCount)
                {
                    // 충분한 챔피언 수가 있으면 보너스를 활성 목록에 추가
                    activeBonusList.Add(championBonus);
                }  
        }

    }

}
