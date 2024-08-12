using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionCombination : MonoBehaviour
{
    // 다른 필드 생략...

    // 조합식 데이터 구조 정의
    [System.Serializable]
    public class CombinationRecipe
    {
        public List<Champion> inputChampions; // 조합에 필요한 입력 챔피언들
        public Champion outputChampion; // 조합 결과로 생성되는 새로운 챔피언
        public override string ToString()
        {
            string inputNames = string.Join(" + ", inputChampions.ConvertAll(c => c.name));
            return $" {inputNames} -> {outputChampion.name}";
        }
    }

    private Map map; // 맵
    private GamePlayController gamePlayController;
    public List<CombinationRecipe> combinationRecipes; // 조합식 리스트
    private List<Champion> champions = new List<Champion>(); // 합성 중 챔피언 저장 리스트
    public GameObject levelupEffectPrefab; // 레벨 업 이펙트 프리팹
    private UIController uiController;
    //public Animator synthesizerAnimator; // 합성기 애니메이터
    //public Transform synthesizerTransform; //합성기 위치

    //public Button yourButton; // Unity Inspector에서 버튼 할당을 위한 변수
    // 다른 메서드 생략...
    void Start()
    {
        // Unity Inspector에서 버튼을 할당하고 클릭 이벤트에 함수를 추가합니다.
        //yourButton.onClick.AddListener(TaskOnClick);
        map = GameObject.Find("Scripts").GetComponent<Map>();
        gamePlayController = GameObject.Find("Scripts").GetComponent<GamePlayController>();
        uiController = GameObject.Find("Scripts").GetComponent<UIController>();
    }
    // 버튼을 클릭할 때 호출될 함수
    void TaskOnClick()
    {
        CreateNewChampionAuto();
    }

    // 주어진 챔피언이 특정 조합에 따라 업그레이드될 수 있는지 여부를 판단하는 메서드
    public bool CanUpgradeChampion(Champion champion, List<Champion> allChampions)
    {
        foreach (CombinationRecipe recipe in combinationRecipes)
        {
            if (recipe.inputChampions.Contains(champion))
            {
                // 조합식에 필요한 모든 챔피언들이 현재 챔피언 목록에 있는지 확인
                bool allInputsAvailable = true;
                foreach (Champion inputChampion in recipe.inputChampions)
                {
                    if (!allChampions.Contains(inputChampion))
                    {
                        allInputsAvailable = false;
                        break;
                    }
                }
                if (allInputsAvailable)
                {
                    // 모든 입력이 가능하면 조합 가능
                    return true;
                }
            }
        }
        // 어떤 조합식에도 해당하지 않으면 업그레이드 불가능
        return false;
    }
// 모든 챔피언들을 체크해서 allChampions 리스트에 저장 후, allChampions에 조합에 필요한 모든 input 챔피언들이 존재하는지 확인하는 방식으로 수정
   public void CreateNewChampion(GameObject championObject)
    {
        // gamePlayController나 map이 null이면 작업을 진행하지 않고 함수를 종료합니다.
        if (gamePlayController == null || map == null)
        {
            Debug.LogWarning("GamePlayController 또는 Map이 설정되지 않았습니다.");
            return;
        }
        //if (synthesizerAnimator != null)
        //{
            //Debug.Log("애니 재생");
            // 애니메이션 재생
            //synthesizerAnimator.SetBool("isAnalyzing", true);
        //}
        //else
        //{
            //Debug.LogError("Animator를 찾을 수 없습니다.");
        //}

        // 챔피언을 저장할 리스트를 초기화합니다.
        ChampionController championController = championObject.GetComponent<ChampionController>();
        Champion champion = championController.champion;
        champions.Add(champion);
        //championObject.transform.position = synthesizerTransform.transform.position;
        Debug.Log("Champions에" + champion + "추가");
        //gamePlayController.RemoveChampionFromArray(gamePlayController.dragStartTrigger.gridType, gamePlayController.dragStartTrigger.gridX, gamePlayController.dragStartTrigger.gridZ);

        // allChampions 리스트에 저장된 챔피언들을 이용하여 조합에 필요한 모든 입력 챔피언들이 존재하는지 확인합니다.
        foreach (CombinationRecipe recipe in combinationRecipes)
        {
            bool inputsAvailable = AllInputsAvailable(champions, recipe);
            if (AllInputsAvailable(champions, recipe))
            {
                // 모든 입력이 가능하면 새로운 챔피언을 생성합니다.
                bool success = StoreSynthesizedChampion(recipe.outputChampion, recipe.inputChampions);
                if (success)
                {
                    //합성이 완료되면 리스트를 초기화합니다.
                    champions.Clear();
                    Debug.Log(recipe.outputChampion + " 생성 및 인벤토리에 추가 완료");
                    //레벨업 이펙트 생성
                    GameObject levelupEffect = Instantiate(levelupEffectPrefab);

                    //레벨업 이펙트의 위치를 챔피언의 위치로 이동
                    levelupEffect.transform.position = recipe.outputChampion.prefab.transform.position;

                    //1.0f초 뒤 레벨업 이펙트를 제거
                    Destroy(levelupEffect, 1.0f);
                    break; // 챔피언을 하나만 생성하고 루프를 종료합니다.
                }
                else
                {
                    Debug.LogError("인벤토리가 가득 차거나 골드가 부족하여 새로운 챔피언을 추가할 수 없습니다.");
                }
                Debug.Log($"조합식 {recipe}의 입력 챔피언들이 존재하는지: {inputsAvailable}");
            }
        }
        //if (synthesizerAnimator != null)
        //{
            // 애니메이션 재생
            //synthesizerAnimator.SetBool("isAnalyzing", false);
        //}
    }

    // 모든 챔피언들을 체크해서 allChampions 리스트에 저장 후, allChampions에 조합에 필요한 모든 input 챔피언들이 존재하는지 확인하는 방식으로 수정
   public void CreateNewChampionAuto()
    {
        Debug.Log("조합 고고씽");
        // gamePlayController나 map이 null이면 작업을 진행하지 않고 함수를 종료합니다.
        if (gamePlayController == null || map == null)
        {
            Debug.LogWarning("GamePlayController 또는 Map이 설정되지 않았습니다.");
            return;
        }
        // 모든 챔피언을 저장할 리스트를 초기화합니다.
        List<Champion> allChampions = new List<Champion>();

        //인벤토리를 검사하여 파괴할 챔피언을 찾습니다.
        for (int i = 0; i < gamePlayController.ownChampionInventoryArray.Length; i++)
        {
            if(gamePlayController.ownChampionInventoryArray[i] != null)
            {
                Champion champion = gamePlayController.ownChampionInventoryArray[i].GetComponent<ChampionController>().champion;
                allChampions.Add(champion);
                Debug.Log("allChampions에" + champion + "추가");
            }
        }

        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++) // 맵상의 모든 지역을 반복 체크
            {
                if (gamePlayController.gridChampionsArray[x, z] != null) // gamePlayController의 챔피언 배열이 null이 아니면(플레이어의 챔피언이 존재할 경우)
                    {
                        // gamePlayController의 챔피언 배열에서 챔피언을 가져와서 allChampions 리스트에 추가합니다.
                        Champion champion = gamePlayController.gridChampionsArray[x, z].GetComponent<ChampionController>().champion;
                        allChampions.Add(champion);
                        Debug.Log("allChampions에" + champion + "추가");
                    }
            }
        }

        bool recipeFound = false;
        // allChampions 리스트에 저장된 챔피언들을 이용하여 조합에 필요한 모든 입력 챔피언들이 존재하는지 확인합니다.
        foreach (CombinationRecipe recipe in combinationRecipes)
        {
            bool inputsAvailable = AllInputsAvailable(allChampions, recipe);
            Debug.Log($"조합식 {recipe}의 입력 챔피언들이 존재하는지: {inputsAvailable}");
            if (inputsAvailable)
            {
                recipeFound = true;
                // 모든 입력이 가능하면 새로운 챔피언을 생성합니다.
                bool success = StoreSynthesizedChampion(recipe.outputChampion, recipe.inputChampions);
                if (success)
                {
                    Debug.Log(recipe.outputChampion + " 생성 및 인벤토리에 추가 완료");
                    //레벨업 이펙트 생성
                    GameObject levelupEffect = Instantiate(levelupEffectPrefab);

                    //레벨업 이펙트의 위치를 챔피언의 위치로 이동
                    levelupEffect.transform.position = recipe.outputChampion.prefab.transform.position;

                    //1.0f초 뒤 레벨업 이펙트를 제거
                    Destroy(levelupEffect, 1.0f);
                    break; // 챔피언을 하나만 생성하고 루프를 종료합니다.
                }
                else
                {
                    Debug.LogError("인벤토리가 가득 차거나 골드가 부족하여 새로운 챔피언을 추가할 수 없습니다.");
                }
            }
        }
        // 조합 가능한 레시피가 없을 경우 메시지를 출력합니다.
        if (!recipeFound)
        {
            StartCoroutine(ShowNoRecipeMessage());
        }
    }

    // 조합에 필요한 챔피언이 없을 경우 메시지를 출력하는 메서드
    private IEnumerator ShowNoRecipeMessage()
    {
        uiController.sellUIPrefab.SetActive(true);
        uiController.sellUIPrefab.transform.forward = Camera.main.transform.forward;
        uiController.sellText.text = "조합에 필요한 챔피언이 없습니다.";
        yield return new WaitForSeconds(2f);
        uiController.sellUIPrefab.SetActive(false);
    }


    // 모든 입력 챔피언들이 존재하는지 확인하는 메서드
    private bool AllInputsAvailable(List<Champion> allChampions, CombinationRecipe recipe)
    {
        foreach (Champion inputChampion in recipe.inputChampions)
        {
            if (!allChampions.Contains(inputChampion))
            {
                // 모든 입력 챔피언이 존재하지 않으면 false를 반환합니다.
                return false;
            }
        }
        // 모든 입력 챔피언이 존재하면 true를 반환합니다.
        return true;
    }

    public bool StoreSynthesizedChampion(Champion outputChampion, List<Champion> inputChampions)
    {
        //인벤토리 배열에서 빈 슬롯을 찾습니다.
        int emptyIndex = -1;
        for (int i = 0; i < gamePlayController.ownChampionInventoryArray.Length; i++) //아군 인벤토리 배열의 길이만큼 반복
        {
            if(gamePlayController.ownChampionInventoryArray[i] == null)
            {
                emptyIndex = i;
                break;
            }
        }

        //챔피언을 추가할 슬롯이 없으면 false 반환
        if (emptyIndex == -1)
            return false;

        //챔피언 프리팹 생성
        GameObject championPrefab = Instantiate(outputChampion.prefab);

        // 챔피언 컨트롤러를 가져옵니다.
        ChampionController championController = championPrefab.GetComponent<ChampionController>();

        // 챔피언 컨트롤러를 초기화합니다.
        championController.Init(outputChampion, ChampionController.TEAMID_PLAYER);

        // 그리드 위치를 설정합니다.
        championController.SetGridPosition(Map.GRIDTYPE_OWN_INVENTORY, emptyIndex, -1);
        
        // 위치와 회전을 설정합니다.
        championController.SetWorldPosition();
        championController.SetWorldRotation();


        //인벤토리 배열에 챔피언 저장
        gamePlayController.StoreChampionInArray(Map.GRIDTYPE_OWN_INVENTORY, map.ownTriggerArray[emptyIndex].gridX, -1, championPrefab);

        // 인풋 챔피언들 파괴
        foreach (Champion inputChampion in inputChampions)
        {
            DestroyMaterialChampion(inputChampion);
        }
        //구매에 성공했을 때 true 반환
        return true;
    }
    // 인풋 챔피언 파괴 메서드
    private void DestroyMaterialChampion(Champion champion)
    {
        //인벤토리를 검사하여 파괴할 챔피언을 찾습니다.
        for (int i = 0; i < gamePlayController.ownChampionInventoryArray.Length; i++)
        {
            if(gamePlayController.ownChampionInventoryArray[i] != null)
            {
                ChampionController championController = gamePlayController.ownChampionInventoryArray[i].GetComponent<ChampionController>();
                if (championController.champion == champion)
                    {
                        championController.currentHealth = 0;
                        Destroy(gamePlayController.ownChampionInventoryArray[i]);
                        Debug.Log($"{champion.name} 파괴 완료");
                        return; // 파괴된 후에는 바로 반환하여 루프를 종료합니다.
                    }
            }
        }

        // 맵 상의 모든 챔피언을 검사하여 파괴할 챔피언을 찾습니다.
        for (int x = 0; x < Map.hexMapSizeX; x++)
        {
            for (int z = 0; z < Map.hexMapSizeZ / 2; z++)
            {
                if (gamePlayController.gridChampionsArray[x, z] != null)
                {
                    ChampionController championController = gamePlayController.gridChampionsArray[x, z].GetComponent<ChampionController>();
                    if (championController.champion == champion)
                        {
                            Destroy(gamePlayController.gridChampionsArray[x, z]);
                            Debug.Log($"{champion.name} 파괴 완료");
                            return; // 파괴된 후에는 바로 반환하여 루프를 종료합니다.
                        }
                }
            }
        }   
    }
}