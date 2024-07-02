using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterPanel : XRBaseInteractable
{

    Map map;
    GamePlayController gamePlayController;
    [SerializeField] ChampionCombination championCombination;
    UIController uiController;
    public GameObject panel;
    public Button comButton;
    private void Start() 
    {
        championCombination = FindObjectOfType<ChampionCombination>();
        map = FindObjectOfType<Map>();
        gamePlayController = FindObjectOfType<GamePlayController>();
        uiController = FindObjectOfType<UIController>();

        if(comButton != null) 
            comButton.onClick.AddListener(Test);
    }

    private void Test()
    {
        championCombination.CreateNewChampionAuto();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        XRRayInteractor rayInteractor = (XRRayInteractor)args.interactorObject;
        RaycastHit hit;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            panel.gameObject.SetActive(true);

        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        XRRayInteractor rayInteractor = (XRRayInteractor)args.interactorObject;
        RaycastHit hit;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            panel.gameObject.SetActive(false);
        }
    }
    public void SellChampion()
    {
        SoundManager.instance.PlaySE("유닛 판매음");
        ChampionController championController = GetComponent<ChampionController>();
        if (championController != null)
            {
                gamePlayController.currentGold += championController.champion.cost;
                uiController.UpdateUI();
                StartCoroutine(SellText());
            }
            championController.currentHealth = 0;
            Destroy(gameObject,1.2f);
    }
    IEnumerator SellText()
    {
        ChampionController championController = GetComponent<ChampionController>();
        Champion champion = championController.champion;
        uiController.sellUIPrefab.SetActive(true);
        uiController.sellText.text = "챔피언 " + champion.uiname + "을/를 팔아 " + championController.champion.cost + "원이 증가했습니다.";
        yield return new WaitForSeconds(1f);
        uiController.sellUIPrefab.SetActive(false);
    }

    }