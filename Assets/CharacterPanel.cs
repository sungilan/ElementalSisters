using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterPanel : XRBaseInteractable
{

Map map;
GamePlayController gamePlayController;
public GameObject panel;
private void Start() 
{
    map = FindObjectOfType<Map>();
    gamePlayController = FindObjectOfType<GamePlayController>();
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
}