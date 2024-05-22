using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Updates and controls UI elements
/// </summary>
public class UIController : MonoBehaviour
{
    public ChampionShop championShop;
    public GamePlayController gamePlayController;

    public GameObject[] championsFrameArray;
    public GameObject[] bonusPanels;


    public TextMeshProUGUI timerText;
    public Text championCountText;
    public TextMeshProUGUI goldText;
    public Text hpText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI saleText;

    public GameObject shop;
    public GameObject restartButton;
    //public GameObject placementText;
    public GameObject gold;
    public GameObject bonusContainer;
    public GameObject bonusUIPrefab;
    public GameObject recipeUIPrefab;
    public GameObject saleUIPrefab;

    public Slider progressBar;

    public void StartButtonClicked()
    {
        gamePlayController.OnGameStageComplate();
    }

    /// <summary>
    /// 상점 UI에서 챔피언 패널을 클릭하면 호출됩니다.
    /// </summary>
    public void OnChampionClicked()
    {
        //get clicked champion ui name
        string name = EventSystem.current.currentSelectedGameObject.transform.parent.name;

        //calculate index from name
        string defaultName = "champion container ";
        int championFrameIndex = int.Parse(name.Substring(defaultName.Length, 1));

        //message shop from click
        championShop.OnChampionFrameClicked(championFrameIndex);
    }

    /// <summary>
    /// 상점 UI에서 리프레시 버튼을 클릭하면 호출됩니다.
    /// </summary>
    public void Refresh_Click()
    {
        championShop.RefreshShop(false);   
    }

    /// <summary>
    /// 상점 UI에서 BuyXP 버튼을 클릭하면 호출됩니다.
    /// </summary>
    public void BuyXP_Click()
    {
        championShop.BuyLvl();
    }

    /// <summary>
    /// UI에서 Restart 버튼을 클릭하면 호출됩니다.
    /// </summary>
    public void Restart_Click()
    {
        gamePlayController.RestartGame();
    }
    /// <summary>
    /// UI에서 조합법 버튼을 클릭하면 호출됩니다.
    /// </summary>
    public void Recipe_Click()
    {
        gamePlayController.RecipeButton();
    }

    /// <summary>
    /// hides chamipon ui frame
    /// </summary>
    public void HideChampionFrame(int index)
    {
        championsFrameArray[index].transform.Find("champion").gameObject.SetActive(false);
    }

    /// <summary>
    /// make shop items visible
    /// </summary>
    public void ShowShopItems()
    {
        //unhide all champion frames
        for (int i = 0; i < championsFrameArray.Length; i++)
        {
            championsFrameArray[i].transform.Find("champion").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// displays champion info to given index on UI
    /// </summary>
    /// <param name="champion"></param>
    /// <param name="index"></param>
    public void LoadShopItem(Champion champion, int index)
    {
        Debug.Log("로드샵 실행");
        //get unit frames
        Transform championUI = championsFrameArray[index].transform.Find("champion");
        Debug.Log("챔피언UI :" + championUI);
        Transform back = championUI.Find("back");
        Transform front = championUI.Find("front");
        Transform type1 = back.Find("type 1");
        Transform type2 = back.Find("type 2");
        Transform name = front.Find("Name");
        Transform cost = front.Find("Cost");
        Transform icon1 = back.Find("icon 1");
        Transform icon2 = back.Find("icon 2");
        


        //assign texts from champion info to unit frames
        MeshRenderer renderer = championUI.GetComponent<MeshRenderer>();
        if (renderer != null) {
            renderer.material = champion.cardMaterial;
            }
        name.GetComponent<TextMeshProUGUI>().text = champion.uiname;
        cost.GetComponent<TextMeshProUGUI>().text = champion.cost.ToString();
        type1.GetComponent<TextMeshProUGUI>().text = champion.type1.displayName;
        type2.GetComponent<TextMeshProUGUI>().text = champion.type2.displayName;
        icon1.GetComponent<Image>().sprite = champion.type1.icon;
        icon2.GetComponent<Image>().sprite = champion.type2.icon;
    }

    /// <summary>
    /// Updates ui when needed
    /// </summary>
    public void UpdateUI()
    {
        roundText.text = gamePlayController.roundCount.ToString();
        goldText.text = gamePlayController.currentGold.ToString();
        //championCountText.text = gamePlayController.currentChampionCount.ToString() + " / " + gamePlayController.currentChampionLimit.ToString();
        //hpText.text = "HP " + gamePlayController.currentHP.ToString();


        //hide bonusus UI
        foreach (GameObject go in bonusPanels) {
            go.SetActive(false);
        }


        //if not null
        if (gamePlayController.championTypeCount != null)
        {
            int i = 0;
            //iterate bonuses
            foreach (KeyValuePair<ChampionType, int> m in gamePlayController.championTypeCount)
            {
                //Now you can access the key and value both separately from this attachStat as:
                GameObject bonusUI = bonusPanels[i];
                bonusUI.transform.SetParent(bonusContainer.transform);
                bonusUI.transform.Find("icon").GetComponent<Image>().sprite = m.Key.icon;
                bonusUI.transform.Find("name").GetComponent<TextMeshProUGUI>().text = m.Key.displayName;
                bonusUI.transform.Find("count").GetComponent<TextMeshProUGUI>().text = m.Value.ToString() + " / " + m.Key.championBonus.championCount.ToString();

                bonusUI.SetActive(true);

                i++;   
            }
        }
    }

    /// <summary>
    /// updates timer
    /// </summary>
    public void UpdateTimerText()
    {
        timerText.text = gamePlayController.timerDisplay.ToString();
    }

    /// <summary>
    /// sets timer visibility
    /// </summary>
    /// <param name="b"></param>
    public void SetTimerTextActive(bool b)
    {
        timerText.gameObject.SetActive(b);

        //placementText.SetActive(b);
    }

    /// <summary>
    /// displays loss screen when game ended
    /// </summary>
    public void ShowLossScreen()
    {
        SetTimerTextActive(false);
        shop.SetActive(false);
        gold.SetActive(false);
        

        restartButton.SetActive(true);
    }

    /// <summary>
    /// displays game screen when game started
    /// </summary>
    public void ShowGameScreen()
    {
        SetTimerTextActive(true);
        shop.SetActive(true);
        gold.SetActive(true);


        restartButton.SetActive(false);
    }

    private void Update()
    {
        {
            if (gamePlayController.currentGameStage == GameStage.Preparation) // 현재 스테이지가 준비단계면
            {
                // 타이머가 진행되는 비율을 계산하여 프로그레스 바에 반영
                float progress = gamePlayController.timer / gamePlayController.PreparationStageDuration; // 준비단계의 진행률 계산
                progressBar.value = progress; // Slider의 값을 변경하여 프로그레스 바 표시
            }
            else if (gamePlayController.currentGameStage == GameStage.Combat) // 현재스테이지가 전투단계이면
            {
                // 타이머가 진행되는 비율을 계산하여 프로그레스 바에 반영
                float progress = gamePlayController.timer / gamePlayController.CombatStageDuration; // 준비단계의 진행률 계산
                progressBar.value = progress; // Slider의 값을 변경하여 프로그레스 바 표시
            }
        }
    }
}
