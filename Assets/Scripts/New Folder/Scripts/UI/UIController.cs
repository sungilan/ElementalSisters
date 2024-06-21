using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
    public GameObject[] specialCardArray;
    public GameObject[] bonusPanels;
    public GameObject[] bonusTooltips;
    public GameObject uiPanel;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI championCountText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI saleText;
    public TextMeshProUGUI levelText;
    [SerializeField] private Image Hpimage;
    [SerializeField] private Image BackHpimage;
    public bool backHpHit = false;

    public GameObject shop;
    public GameObject gameOverPanel;
    public GameObject restartButton;
    //public GameObject placementText;
    public GameObject gold;
    public GameObject bonusContainer;
    public GameObject bonusUIPrefab;
    public GameObject recipeUIPrefab;
    public GameObject saleUIPrefab;

    public Slider progressBar;

    private void Start()
    {
        Hpimage.fillAmount = (float)gamePlayController.currentHP / (float)gamePlayController.maxHP;
    }
    private void HandleHp()
    {
        Hpimage.fillAmount = Mathf.Lerp(Hpimage.fillAmount, (float)gamePlayController.currentHP / (float)gamePlayController.maxHP, Time.deltaTime * 5f);
        if (backHpHit)
        {
            BackHpimage.fillAmount = Mathf.Lerp(BackHpimage.fillAmount, Hpimage.fillAmount, Time.deltaTime * 10f);
            if (Hpimage.fillAmount >= BackHpimage.fillAmount - 0.01f)
            {
                backHpHit = false;
                BackHpimage.fillAmount = Hpimage.fillAmount;
            }
        }
    }

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
    /// hides chamipon shop
    /// </summary>
    public void HideChampionShop()
    {
        shop.gameObject.SetActive(!shop.gameObject.activeSelf);
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
        //get unit frames
        Transform championUI = championsFrameArray[index].transform.Find("champion");
        Transform back = championUI.Find("back");
        Transform front = championUI.Find("front");
        Transform type1 = back.Find("type 1");
        Transform type2 = back.Find("type 2");
        Transform type3 = back.Find("type 3");
        Transform name = front.Find("Name");
        Transform cost = front.Find("Cost");
        Transform icon1 = back.Find("icon 1");
        Transform icon2 = back.Find("icon 2");
        Transform icon3 = back.Find("icon 3");

        //assign texts from champion info to unit frames
        MeshRenderer renderer = championUI.GetComponent<MeshRenderer>();
        if (renderer != null) 
        {
            renderer.material = champion.cardMaterial;
        }
        name.GetComponent<TextMeshProUGUI>().text = champion.uiname;
        cost.GetComponent<TextMeshProUGUI>().text = champion.cost.ToString();
        type1.GetComponent<TextMeshProUGUI>().text = champion.type1.displayName;
        type2.GetComponent<TextMeshProUGUI>().text = champion.type2.displayName;
        type3.GetComponent<TextMeshProUGUI>().text = champion.type3.displayName;
        icon1.GetComponent<Image>().sprite = champion.type1.icon;
        icon2.GetComponent<Image>().sprite = champion.type2.icon;
        icon3.GetComponent<Image>().sprite = champion.type3.icon;
    }

    public void LoadSpecialCard(SpecialCard specialCard, int index)
    {
        //get unit frames
        Transform cardUI = specialCardArray[index].transform.Find("UI Components");
        Transform cardName1 = cardUI.transform.Find("Name1");
        Transform cardExplain1 = cardUI.transform.Find("Explain1");
        Transform cardicon1 = cardUI.transform.Find("icon1");
        Transform cardName2 = cardUI.transform.Find("Name2");
        Transform cardExplain2 = cardUI.transform.Find("Explain2");
        Transform cardicon2 = cardUI.transform.Find("icon2");

        cardName1.GetComponent<TextMeshProUGUI>().text = specialCard.cardName;
        cardExplain1.GetComponent<TextMeshProUGUI>().text = specialCard.cardExplain;
        cardicon1.GetComponent<Image>().sprite = specialCard.cardIcon;
        cardName2.GetComponent<TextMeshProUGUI>().text = specialCard.cardName;
        cardExplain2.GetComponent<TextMeshProUGUI>().text = specialCard.cardExplain;
        cardicon2.GetComponent<Image>().sprite = specialCard.cardIcon;
    }

    /// <summary>
    /// Updates ui when needed
    /// </summary>
    public void UpdateUI()
    {
        roundText.text = gamePlayController.roundCount.ToString();
        goldText.text = gamePlayController.currentGold.ToString();
        championCountText.text = gamePlayController.currentChampionCount.ToString() + " / " + gamePlayController.currentChampionLimit.ToString();
        levelText.text = gamePlayController.currentLevel.ToString();
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
    public void ShowBonusTooltips(int number)
    {
        bonusTooltips[number].SetActive(true);
    }

    public void HideBonusTooltips(int number)
    {
        bonusTooltips[number].SetActive(false);
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
        //gold.SetActive(false);
        uiPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        //restartButton.SetActive(true);
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
        HandleHp();
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
