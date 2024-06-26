using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


/// <summary>
/// Controlls player input
/// </summary>
public class InputController : MonoBehaviour
{
    private InputDevice targetDevice;
    public GamePlayController gamePlayController;

    //map script
    public Map map;


    public LayerMask triggerLayer;

    //declare ray starting position var
    private Vector3 rayCastStartPosition;

    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    // Start is called before the first frame update
    void Start()
    {
        //set position of ray starting point to trigger objects
        rayCastStartPosition = new Vector3(0, 20, 0);

        // 특정 컨트롤러하나만 가져오는 방법
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics =
        InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
 
        if (devices.Count > 0)
        	targetDevice = devices[0];
    }

    //to store mouse position
    private Vector3 mousePosition;

    
    [HideInInspector]
    public TriggerInfo triggerInfo = null;

    /// Update is called once per frame
    void Update()
    {
        Debug.Log("할당됨 "+targetDevice);
        triggerInfo = null;
        map.resetIndicators();

        //declare rayhit
        RaycastHit hit;

        //convert mouse screen position to ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if ray hits something
        if (Physics.Raycast(ray, out hit, 100f, triggerLayer, QueryTriggerInteraction.Collide))
        {
            //히트된 오브젝트의 TriggerInfo를 가져옵니다.
            triggerInfo = hit.collider.gameObject.GetComponent<TriggerInfo>();

            //this is a trigger
            if(triggerInfo != null)
            {
                //get indicator
                GameObject indicator = map.GetIndicatorFromTriggerInfo(triggerInfo);

                //set indicator color to active
                indicator.GetComponent<MeshRenderer>().material.color = map.indicatorActiveColor;
            }
            else
                map.resetIndicators(); //reset colors
        }
               

        if (Input.GetMouseButtonDown(0))
        {
            gamePlayController.StartDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gamePlayController.StopDrag();
        }

        //store mouse position
        mousePosition = Input.mousePosition;
        ProcessXRInteraction(leftRayInteractor);
        ProcessXRInteraction(rightRayInteractor);

        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if (primaryButtonValue)
        	Debug.Log("Pressing primary button");
 
        targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        if (triggerValue > 0.1F)
        	Debug.Log("Trigger pressed " + triggerValue);
    }
    private void ProcessXRInteraction(XRRayInteractor rayInteractor)
    {
        if (rayInteractor != null)
        {
            RaycastHit hit;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
            {
                triggerInfo = hit.collider.gameObject.GetComponent<TriggerInfo>();

                if (triggerInfo != null)
                {
                    GameObject indicator = map.GetIndicatorFromTriggerInfo(triggerInfo);
                    indicator.GetComponent<MeshRenderer>().material.color = map.indicatorActiveColor;
                }
                else
                {
                    map.resetIndicators();
                }
            }
        }
    }
    
}
