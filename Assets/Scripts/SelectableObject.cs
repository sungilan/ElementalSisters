using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectableObject : XRBaseInteractable
{
    Map map;
    GamePlayController gamePlayController;
    private void Start() 
    {

        map = FindObjectOfType<Map>();
        gamePlayController = FindObjectOfType<GamePlayController>();
    }
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        //Debug.Log("닿았다");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        XRRayInteractor rayInteractor = (XRRayInteractor)args.interactorObject;
        RaycastHit hit;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            //Debug.Log("드래그");
            gamePlayController.StartDrag();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        XRRayInteractor rayInteractor = (XRRayInteractor)args.interactorObject;
        RaycastHit hit;
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
        {
            //Debug.Log("드래그 끝");
            gamePlayController.StopDrag();
        }
    }
}