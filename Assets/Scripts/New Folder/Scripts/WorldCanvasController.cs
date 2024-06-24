using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WorldSpace에 HealthBar 및 FloatingText Canvas UI를 표시하는 도우미 클래스
/// </summary>
public class WorldCanvasController : MonoBehaviour
{
    public GameObject worldCanvas;
    public GameObject floatingTextPrefab;
    public GameObject healthBarPrefab;
    public GameObject manaBarPrefab;

    /// <summary>
    /// For Creating a new FloatingText
    /// </summary>
    /// <param name="position"></param>
    /// <param name="v"></param>
    public void AddDamageText(Vector3 position, float v)
    {
        GameObject go = Instantiate(floatingTextPrefab);
        go.transform.SetParent(worldCanvas.transform);
       
        go.GetComponent<FloatingText>().Init(position, v);
    }

    /// <summary>
    /// For Creating a new HealthBar
    /// </summary>
    /// <param name="position"></param>
    /// <param name="v"></param>
    public void AddHealthBar(GameObject championGO)
    {
        GameObject go = Instantiate(healthBarPrefab); // 체력바 프리팹 생성
        go.transform.SetParent(worldCanvas.transform); // 생성된 체력바의 부모를 worldCanvas의 transform으로 설정

        go.GetComponent<HealthBar>().Init(championGO); // go에서 HealthBar 컴포넌트를 가져와서 그것의 Init 메소드를 호출
                                                       // 이 메소드에는 championGO라는 게임 오브젝트를 매개변수로 전달합니다. 이렇게 함으로써 HealthBar 컴포넌트의 초기화가 이루어지며, 이 체력바가 어떤 챔피언에 대한 것인지 설정됩니다.
    }
    public void AddManaBar(GameObject championGO)
    {
        GameObject go = Instantiate(manaBarPrefab); // 체력바 프리팹 생성
        go.transform.SetParent(worldCanvas.transform); // 생성된 체력바의 부모를 worldCanvas의 transform으로 설정

        go.GetComponent<ManaBar>().Init(championGO); // go에서 HealthBar 컴포넌트를 가져와서 그것의 Init 메소드를 호출
                                                       // 이 메소드에는 championGO라는 게임 오브젝트를 매개변수로 전달합니다. 이렇게 함으로써 HealthBar 컴포넌트의 초기화가 이루어지며, 이 체력바가 어떤 챔피언에 대한 것인지 설정됩니다.
    }
}
