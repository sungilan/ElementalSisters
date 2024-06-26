using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterPanel : XRBaseInteractable
{

Map map;
GamePlayController gamePlayController;
UIController uiController;
public GameObject panel;
private void Start() 
{
    map = FindObjectOfType<Map>();
    gamePlayController = FindObjectOfType<GamePlayController>();
    uiController = FindObjectOfType<UIController>();
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
        
        ChampionController championController = GetComponent<ChampionController>();
        if (championController != null)
            {
                gamePlayController.currentGold += championController.champion.cost;
                StartCoroutine(SellText());
            }
            Destroy(gameObject);
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