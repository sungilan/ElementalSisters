using System.Collections;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterPanel : XRBaseInteractable
{
    public InputActionAsset inputActionAsset; // Input actions asset
    public XRRayInteractor rayInteractor; // XRRayInteractor reference
    public XROrigin player;

    private Map map;
    private GamePlayController gamePlayController;
    private ChampionCombination championCombination;
    private UIController uiController;
    private ChampionController championController;
    public GameObject panel;
    public Button sellButton;
    public Button upButton;
    public Button comButton;


    private void Start()
    {
        //player = FindObjectOfType<XROrigin>();
        //rayInteractor = player.GetComponentInChildren<XRRayInteractor>();
        championController = GetComponent<ChampionController>();
        championCombination = FindObjectOfType<ChampionCombination>();
        map = FindObjectOfType<Map>();
        gamePlayController = FindObjectOfType<GamePlayController>();
        uiController = FindObjectOfType<UIController>();

        Transform firstChild = transform.GetChild(0);
        sellButton = firstChild.GetChild(1).GetComponent<Button>();
        upButton = firstChild.GetChild(2).GetComponent<Button>();
        comButton = firstChild.GetChild(3).GetComponent<Button>();

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellChampion);
        }
        if (upButton != null)
        {
            upButton.onClick.AddListener(Upgrade);
        }
        if (comButton != null)
        {
            comButton.onClick.AddListener(Combination);
        }
    }
    private void Upgrade()
    {
        panel.SetActive(false);
        gamePlayController.TryUpgradeChampion(championController.champion);
    }

    private void Combination()
    {
        panel.SetActive(false);
        uiController.Combination();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        XRRayInteractor rayInteractor = (XRRayInteractor)args.interactorObject;
        RaycastHit hit;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            // 카메라와의 거리 및 시야에 따라 UI가 항상 카메라를 향하도록 설정
            panel.transform.forward = Camera.main.transform.forward;
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
        Destroy(gameObject, 1.2f);
    }
    IEnumerator SellText()
    {
        ChampionController championController = GetComponent<ChampionController>();
        Champion champion = championController.champion;
        uiController.sellUIPrefab.SetActive(true);
        uiController.sellUIPrefab.transform.forward = Camera.main.transform.forward;
        uiController.sellText.text = "챔피언 " + champion.uiname + "을/를 팔아 " + championController.champion.cost + "원이 증가했습니다.";
        yield return new WaitForSeconds(1f);
        uiController.sellUIPrefab.SetActive(false);
    }

    /*private void Update()
    {
        if (rayInteractor != null)
        {
            Debug.Log("RayInteractor Not Null");
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                Debug.Log("RayCastHit");
                float gripValue = inputActionAsset.actionMaps[2].actions[0].ReadValue<float>();
                float triggerValue = inputActionAsset.actionMaps[2].actions[2].ReadValue<float>();

                // 그립 버튼 상태 확인
                if (gripValue > 0.5f)
                {
                    OnGripPressed();
                }

                // 트리거 버튼 상태 확인
                if (triggerValue > 0.5f)
                {
                    OnTriggerPressed();
                }
            }
        }
    }

    private void OnGripPressed()
    {
        Debug.Log("그립 버튼 눌림");
        gamePlayController.StartDrag();
    }

    private void OnTriggerPressed()
    {
        Debug.Log("트리거 버튼 눌림");
        // 카메라와의 거리 및 시야에 따라 UI가 항상 카메라를 향하도록 설정
        panel.transform.forward = Camera.main.transform.forward;
        panel.gameObject.SetActive(true);
    }*/
}