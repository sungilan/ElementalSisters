using UnityEngine;
/// <summary>
/// 플레이어가 챔피언을 이동할 수 있는 지도 그리드를 생성합니다.
/// </summary>
public class Map : MonoBehaviour
{
    //그리드 유형 선언 
    public static int GRIDTYPE_OWN_INVENTORY = 0;
    public static int GRIDTYPE_OPONENT_INVENTORY = 1;
    public static int GRIDTYPE_HEXA_MAP = 2;
    public static int GRIDTYPE_TRASH_CAN = 3;
    public static int GRIDTYPE_SYNTHESIZER = 4;

    public static int hexMapSizeX = 8; // 맵 전체 x축 사이즈 : 7칸
    public static int hexMapSizeZ = 8; // 맵 전체 z축 사이즈 : 8칸
    public static int inventorySizeX = 9; // 인벤토리 x축 사이즈
    public static int inventorySizeZ = 3; // 인벤토리 z축 사이즈
    public static int inventorySize = inventorySizeX * inventorySizeZ; // 총 인벤토리 크기


    public Plane m_Plane;

    //start positions
    public Transform ownInventoryStartPosition; // 아군 캐릭터 생성 지점
    public Transform oponentInventoryStartPosition; // 적군 캐릭터 생성 지점
    public Transform mapStartPosition; //그리드 생성 시작 지점
    public Transform trashCanPosition; //쓰레기통 지점
    public Transform synthesizerPosition; //합성기 지점


    //챔피언을 어디에 배치하는지 보여주는 지표
    public GameObject squareIndicator; // 사각형 모양 그리드
    public GameObject hexaIndicator; // 헥스 모양 그리드
    public GameObject sexaIndicator; // 쓰레기통 모양 그리드


    public Color indicatorDefaultColor; // 기본 색상
    public Color indicatorActiveColor; // 활성화 색상

    // Start is called before the first frame update
    void Start()
    {
        CreateGridPosition();
        CreateIndicators();
        HideIndicators();

        m_Plane = new Plane(Vector3.up, Vector3.zero);

        //다른 스크립트에게 지도가 준비되었음을 알립니다.
        this.SendMessage("OnMapReady", SendMessageOptions.DontRequireReceiver);
    }

    /// Update is called once per frame
    void Update()
    {
        
    }



    //그리드 위치를 저장하는 배열
    [HideInInspector]
    public Vector3[] ownInventoryGridPositions; //아군 인벤토리 위치 저장 배열
    [HideInInspector]
    public Vector3[] oponentInventoryGridPositions; //적군 인벤토리 위치 저장 배열
    [HideInInspector]
    public Vector3[,] mapGridPositions; //그리드 위치 저장 배열
    [HideInInspector]
    public Vector3 trashCanGridPosition; //쓰레기통 위치 저장
    [HideInInspector]
    public Vector3 synthesizerGridPosition; //합성기 위치 저장

    /// <summary>
    /// 모든 지도 그리드의 위치를 생성합니다.
    /// </summary>
    private void CreateGridPosition()
    {
        //위치 배열 초기화
        ownInventoryGridPositions = new Vector3[inventorySize];
        oponentInventoryGridPositions = new Vector3[inventorySize]; 
        mapGridPositions = new Vector3[hexMapSizeX, hexMapSizeZ];
        trashCanGridPosition = new Vector3();


        //아군 인벤토리 위치 생성
        for (int x = 0; x < inventorySizeX; x++)
        {
            for (int z = 0; z < inventorySizeZ; z++)
            {
                float offsetX = x * -2.5f;
                float offsetZ = z * 2.5f;

                Vector3 position = GetMapHitPoint(ownInventoryStartPosition.position + new Vector3(offsetX, 0, offsetZ));
                ownInventoryGridPositions[x * inventorySizeZ + z] = position;
            }
        }


        //적군 인벤토리 위치 생성
        for (int i = 0; i < inventorySize; i++) //inventorySize(9)만큼 반복
        {
            // 이 슬롯에 대한 위치 x 오프셋 계산
            float offsetX = i * -2.5f;

            // 위치 계산 및 저장
            Vector3 position = GetMapHitPoint(oponentInventoryStartPosition.position + new Vector3(offsetX, 0, 0)); //스타트 포지션에서 -2.5f 간격만큼 이동

            // 배열에 위치 변수 추가
            oponentInventoryGridPositions[i] = position;
        }

        // 맵 위치 생성
        for (int x = 0; x < hexMapSizeX; x++) //Map의 hexMapSizeX(7)만큼 반복
        {
            for (int z = 0; z < hexMapSizeZ; z++) //Map의 hexMapSizeZ(8)만큼 반복(맵 전체)
            {
                // 짝수 또는 홀수 줄 계산
                int rowOffset = z % 2; // rowOffset = z를 2로 나눈 나머지(z축이 짝수일 때 0, 홀수일 때 1)

                // x와 z 위치 계산
                float offsetX = x * -3f + rowOffset * 1.5f; //x축으로 -3f씩 이동, z가 홀수일때는 1.5f 더 이동 (rowOffset이 1일 때 +1.5f)
                float offsetZ = z * -2.5f; //z축으로 -2.5f씩 이동

                // 위치 계산 및 저장
                Vector3 position = GetMapHitPoint(mapStartPosition.position + new Vector3(offsetX, 0, offsetZ)); //스타트 포지션에서 x축으로 offsetX, z축으로 offsetZ만큼 이동

                // 배열에 위치 변수 추가
                mapGridPositions[x, z] = position; // 배열에 계산된 위치 저장
            }
          
        }

    }



    // 지표를 저장할 배열 선언
    [HideInInspector]
    public GameObject[] ownIndicatorArray;  //아군 지표 저장 배열
    [HideInInspector]
    public GameObject[] oponentIndicatorArray; //적군 지표 저장 배열
    [HideInInspector]
    public GameObject[,] mapIndicatorArray; //맵 지표 저장 배열
    [HideInInspector]
    public GameObject trashCanIndicator; //쓰레기통 지표 저장
    [HideInInspector]
    public GameObject synthesizerIndicator; //합성기 지표 저장


    [HideInInspector]
    public TriggerInfo[] ownTriggerArray; //인벤토리 트리거 저장 배열
    [HideInInspector]
    public TriggerInfo[,] mapGridTriggerArray; //맵 트리거 저장 배열
    [HideInInspector]
    public TriggerInfo trashCan; //쓰레기통 트리거 저장배열
    [HideInInspector]
    public TriggerInfo synthesizer; //합성기 트리거 저장배열



    [SerializeField]private GameObject indicatorContainer;

    /// <summary>
    /// 모든 지도 지표를 생성합니다.
    /// </summary>
    private void CreateIndicators()
    {
        // 지표를 저장할 컨테이너 생성
        indicatorContainer = new GameObject();
        indicatorContainer.name = "IndicatorContainer";

        // 트리거를 저장할 컨테이너 생성
        GameObject triggerContainer = new GameObject();
        triggerContainer.name = "TriggerContainer";


        // 지표를 저장할 배열 초기화
        ownIndicatorArray = new GameObject[inventorySize];
        oponentIndicatorArray = new GameObject[inventorySize];
        mapIndicatorArray = new GameObject[hexMapSizeX, hexMapSizeZ / 2];
        trashCanIndicator = new GameObject();

        ownTriggerArray = new TriggerInfo[inventorySize];
        mapGridTriggerArray = new TriggerInfo[hexMapSizeX, hexMapSizeZ / 2];
        trashCan = new TriggerInfo();
        synthesizer = new TriggerInfo();

        if (synthesizerPosition != null)
        {
            // 지표 게임오브젝트 생성(hexaIndicator를 생성하여 indicatorGO에 저장)
            GameObject indicatorGO = Instantiate(hexaIndicator);

            // 지표 게임오브젝트 위치 설정(indicatorGO의 위치를 mapGridPositions[x,z]으로 설정)
            indicatorGO.transform.position = synthesizerPosition.transform.position;

            // 지표 부모 설정(indicatorGO의 부모를 indicatorContainer.transform으로 설정)
            indicatorGO.transform.parent = indicatorContainer.transform;

            // 배열에 지표 게임오브젝트 저장
            synthesizerIndicator = indicatorGO;

            // 트리거 게임오브젝트 생성
            GameObject trigger = CreateBoxTrigger(GRIDTYPE_SYNTHESIZER, 1);
            // 트리거 부모 설정
            trigger.transform.parent = triggerContainer.transform;

            // 트리거 게임오브젝트 위치 설정
            trigger.transform.position = synthesizerPosition.transform.position;

            // TriggerInfo 저장
            synthesizer = trigger.GetComponent<TriggerInfo>();
        }

        if (trashCanPosition != null)
        {
            // 지표 게임오브젝트 생성(hexaIndicator를 생성하여 indicatorGO에 저장)
            GameObject indicatorGO = Instantiate(sexaIndicator);

            // 지표 게임오브젝트 위치 설정(indicatorGO의 위치를 mapGridPositions[x,z]으로 설정)
            indicatorGO.transform.position = trashCanPosition.transform.position;

            // 지표 부모 설정(indicatorGO의 부모를 indicatorContainer.transform으로 설정)
            indicatorGO.transform.parent = indicatorContainer.transform;

            // 배열에 지표 게임오브젝트 저장
            trashCanIndicator = indicatorGO;

            // 트리거 게임오브젝트 생성
            GameObject trigger = CreateBoxTrigger(GRIDTYPE_TRASH_CAN, 10);
            // 트리거 부모 설정
            trigger.transform.parent = triggerContainer.transform;

            // 트리거 게임오브젝트 위치 설정
            trigger.transform.position = trashCanPosition.transform.position;

            // TriggerInfo 저장
            trashCan = trigger.GetComponent<TriggerInfo>();
        }

        // 아군 그리드 위치 반복
            for (int i = 0; i < inventorySize; i++) //inventorySize만큼 반복
            {
                // 지표 게임오브젝트 생성(squareIndicator를 생성하여 indicatorGO에 저장)
                GameObject indicatorGO = Instantiate(squareIndicator);

                // 지표 게임오브젝트 위치 설정
                indicatorGO.transform.position = ownInventoryGridPositions[i];

                // 지표 부모 설정
                indicatorGO.transform.parent = indicatorContainer.transform;

                // 배열에 지표 게임오브젝트 저장
                ownIndicatorArray[i] = indicatorGO;

                // 트리거 게임오브젝트 생성
                GameObject trigger = CreateBoxTrigger(GRIDTYPE_OWN_INVENTORY, i);

                // 트리거 부모 설정
                trigger.transform.parent = triggerContainer.transform;

                // 트리거 게임오브젝트 위치 설정
                trigger.transform.position = ownInventoryGridPositions[i];

                // TriggerInfo 저장
                ownTriggerArray[i] = trigger.GetComponent<TriggerInfo>();
            }


        /*
        //iterate oponent grid position
        for (int i = 0; i < inventorySize; i++)
        {
            //create indicator gameobject
            GameObject indicatorGO = Instantiate(squareIndicator);

            //set indicator gameobject position
            indicatorGO.transform.position = oponentInventoryGridPositions[i];

            //set indicator parent
            indicatorGO.transform.parent = indicatorContainer.transform;

            //store indicator gameobject in array
            oponentIndicatorArray[i] = indicatorGO;


        }
        */

        //맵 그리드 위치 반복
        for (int x = 0; x < hexMapSizeX; x++) //Map의 hexMapSizeX만큼 반복
        {
            for (int z = 0; z < hexMapSizeZ /2; z++) //Map의 hexMapSizeZ의 반만큼 반복
            {
                // 지표 게임오브젝트 생성(hexaIndicator를 생성하여 indicatorGO에 저장)
                GameObject indicatorGO = Instantiate(hexaIndicator);

                // 지표 게임오브젝트 위치 설정(indicatorGO의 위치를 mapGridPositions[x,z]으로 설정)
                indicatorGO.transform.position = mapGridPositions[x,z];

                // 지표 부모 설정(indicatorGO의 부모를 indicatorContainer.transform으로 설정)
                indicatorGO.transform.parent = indicatorContainer.transform;

                // 배열에 지표 게임오브젝트 저장
                mapIndicatorArray[x, z] = indicatorGO;

                // 트리거 게임오브젝트 생성
                GameObject trigger = CreateSphereTrigger(GRIDTYPE_HEXA_MAP, x, z);

                // 트리거 부모 설정
                trigger.transform.parent = triggerContainer.transform;

                // 트리거 게임오브젝트 위치 설정
                trigger.transform.position = mapGridPositions[x, z];

                // TriggerInfo 저장
                mapGridTriggerArray[x, z] = trigger.GetComponent<TriggerInfo>();

            }
        }

    }

    /// <summary>
    /// 정확한 y축을 가진 점을 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetMapHitPoint(Vector3 p)
    {
        Vector3 newPos = p;

        RaycastHit hit;

        if (Physics.Raycast(newPos + new Vector3(0, 10, 0), Vector3.down, out hit, 15))
        {
            newPos = hit.point;
        }

        return newPos;
    }

    /// <summary>
    /// 트리거 콜라이더 게임오브젝트를 생성하고 반환합니다.(박스콜라이더)
    /// </summary>
    /// <returns></returns>
    private GameObject CreateBoxTrigger(int type)
    {
        // 기본 게임오브젝트 생성
        GameObject trigger = new GameObject();

        // 생성된 trigger오브젝트에 콜라이더 컴포넌트 추가
        BoxCollider collider = trigger.AddComponent<BoxCollider>();

        // 콜라이더 크기 설정
        collider.size = new Vector3(2, 0.5f, 2);

        // 트리거로 설정 
        collider.isTrigger = true;

        // 생성된 trigger오브젝트에 TriggerInfo 추가 및 저장
        TriggerInfo trigerInfo = trigger.AddComponent<TriggerInfo>();
        trigerInfo.gridType = type;

        trigger.layer = LayerMask.NameToLayer("Triggers");

        return trigger;
    }


    /// <summary>
    /// 트리거 콜라이더 게임오브젝트를 생성하고 반환합니다.(박스콜라이더)
    /// </summary>
    /// <returns></returns>
    private GameObject CreateBoxTrigger(int type, int x)
    {
        // 기본 게임오브젝트 생성
        GameObject trigger = new GameObject();

        // 생성된 trigger오브젝트에 콜라이더 컴포넌트 추가
        BoxCollider collider = trigger.AddComponent<BoxCollider>();
        SelectableObject selectableObject = trigger.AddComponent<SelectableObject>();

        // 콜라이더 크기 설정
        collider.size = new Vector3(2, 0.5f, 2);

        // 트리거로 설정 
        //collider.isTrigger = true;

        // 생성된 trigger오브젝트에 TriggerInfo 추가 및 저장
        TriggerInfo trigerInfo = trigger.AddComponent<TriggerInfo>();
        trigerInfo.gridType = type;
        trigerInfo.gridX = x;

        //trigger.layer = LayerMask.NameToLayer("Triggers");

        return trigger;
    }

    /// <summary>
    /// 트리거 콜라이더 게임오브젝트를 생성하고 반환합니다.(스페어콜라이더)
    /// </summary>
    /// <returns></returns>
    private GameObject CreateSphereTrigger(int type, int x, int z)
    {
        // 기본 게임오브젝트 생성
        GameObject trigger = new GameObject();

        // 콜라이더 컴포넌트 추가
        SphereCollider collider = trigger.AddComponent<SphereCollider>();
        SelectableObject selectableObject = trigger.AddComponent<SelectableObject>();

        // 콜라이더 크기 설정
        collider.radius = 1.4f;

        // 트리거로 설정 
        //collider.isTrigger = true;

        // TriggerInfo 추가 및 저장
        TriggerInfo trigerInfo = trigger.AddComponent<TriggerInfo>();
        trigerInfo.gridType = type;
        trigerInfo.gridX = x;
        trigerInfo.gridZ = z;

        //trigger.layer = LayerMask.NameToLayer("Triggers");

        return trigger;
    }


    /// <summary>
    /// TriggerInfo에서 지표를 반환합니다.
    /// </summary>
    /// <param name="triggerinfo"></param>
    /// <returns></returns>
    public GameObject GetIndicatorFromTriggerInfo(TriggerInfo triggerinfo)
    {
        GameObject triggerGo = null;

        if(triggerinfo.gridType == GRIDTYPE_OWN_INVENTORY) //triggerinfo의 gridType이 GRIDTYPE_OWN_INVENTORY일 때
        {
            triggerGo = ownIndicatorArray[triggerinfo.gridX];
        }
        else if (triggerinfo.gridType == GRIDTYPE_OPONENT_INVENTORY) //triggerinfo의 gridType이 GRIDTYPE_OPONENT_INVENTORY일 때
        {
            triggerGo = oponentIndicatorArray[triggerinfo.gridX];
        }
        else if (triggerinfo.gridType == GRIDTYPE_HEXA_MAP) //triggerinfo의 gridType이 GRIDTYPE_HEXA_MAP일 때
        {
            triggerGo = mapIndicatorArray[triggerinfo.gridX, triggerinfo.gridZ];
        }
        else if (triggerinfo.gridType == GRIDTYPE_TRASH_CAN) //triggerinfo의 gridType이 GRIDTYPE_TRASH_CAN일 때
        {
            triggerGo = trashCanIndicator;
        }
        else if (triggerinfo.gridType == GRIDTYPE_SYNTHESIZER) //triggerinfo의 gridType이 GRIDTYPE_TRASH_CAN일 때
        {
            triggerGo = synthesizerIndicator;
        }


        return triggerGo;
    }

    /// <summary>
    /// 모든 지표 색상을 기본값으로 재설정합니다.
    /// </summary>
    public void resetIndicators()
    {
        trashCanIndicator.GetComponent<MeshRenderer>().material.color = indicatorDefaultColor;

        for (int x = 0; x < hexMapSizeX; x++) //Map의 hexMapSizeX(7)만큼 반복
        {
            for (int z = 0; z < hexMapSizeZ / 2; z++) //Map의 hexMapSizeZ(8)의 반만큼(아군 진영) 반복
            {
                mapIndicatorArray[x, z].GetComponent<MeshRenderer>().material.color = indicatorDefaultColor; // mapIndicatorArray[x, z]의 MeshRenderer를 불러와 indicatorDefaultColor로 변경
            }
        }

        
        for (int x = 0; x < inventorySize; x++) //아군 인벤토리사이즈(9)만큼 반복
        {
           ownIndicatorArray[x].GetComponent<MeshRenderer>().material.color = indicatorDefaultColor; // ownIndicatorArray[x]의 MeshRenderer를 불러와 indicatorDefaultColor로 변경
           // oponentIndicatorArray[x].GetComponent<MeshRenderer>().material.color = indicatorDefaultColor;
        }
        
    }

    /// <summary>
    /// 모든 지도 표시를 보이게 만듬
    /// </summary>
    public void ShowIndicators()
    {
        indicatorContainer.SetActive(true);
    }

    /// <summary>
    /// 모든 지도 표시를 보이지 않게 만듬
    /// </summary>
    public void HideIndicators()
    {
        indicatorContainer.SetActive(false);
    }
}















/*! \mainpage Auto Chess documentation
 * 
* <b>Thank you for purchasing Auto Chess.</b><br>
* <br>For any question don't hesitate to contact me at : asoliddev@gmail.com
* <br>AssetStore Profile : https://assetstore.unity.com/publishers/38620 
* <br>ArtStation Profile : https://asoliddev.artstation.com/
* 
*  \subsection Basics
* Auto Chess complete and fully functional game, <br>
* it has been created with simplicity in mind. <br>
* Great as a starting point to create your own Auto Chess game. <br>
* All the scripts are attached to the Script Gameobject In the Hierarchy window. <br>
* Basic game rules and Champions can be changed by adjusting public variables. <br>
* Core game changes can be done by changing the source code. <br>
* Source code uses MVC design and can be easly expanded on.
*  
* 
*  \subsection Champions
* Existing Champions and ChampionTypes are located in the <b>Champions</b> and <b>ChampionTypes</b> folder. <br>
* To Create new Champion or ChampionType go to Assets Menu -> Create -> Auto Chess -> Champion or ChampionType. <br>
* Champions can be customised with this two classes from the editor. <br>
* For detailed options check out : Champion and ChampionType documentation. <br>
* All Champions and ChampionTypes need to be assigned to the GameData script to be recognised by the ChampionShop.
* 
* 
* \subsection Packages
* Auto chess uses Post Processing package for better visuals. <br>
* To install it go to Window Menu -> Package Manager and install Post Processing
* 
*/
