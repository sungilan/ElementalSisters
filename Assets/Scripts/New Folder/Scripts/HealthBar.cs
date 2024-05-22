using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays champion health over champion
/// </summary>
public class HealthBar : MonoBehaviour
{
    private GameObject championGO;
    private ChampionController championController;
    //private Renderer barRenderer; // 체력바 렌더러
    private Image barImage;

    private float initialScaleX; // 초기 체력바 너비

    /// Start is called before the first frame update
    void Start()
    {
        //barRenderer = GetComponent<Renderer>(); // 체력바의 렌더러 컴포넌트 가져오기
        barImage = GetComponent<Image>();
        initialScaleX = transform.localScale.x; // 초기 체력바 너비 저장
    }

    /// Update is called once per frame
    void Update()
    {
        if(championGO != null)
        {
            this.transform.position = championGO.transform.position + new Vector3(0, 4.5f, 0);
            // 챔피언의 현재 체력과 최대 체력 비율 계산
            float healthRatio = championController.currentHealth / championController.maxHealth;

            // 체력바 너비를 비율에 따라 변경
            Vector3 newScale = transform.localScale;
            newScale.x = initialScaleX * healthRatio;
            transform.localScale = newScale;

            // 챔피언의 체력에 따라 색상 변경
            //Color barColor = Color.Lerp(Color.red, Color.green, healthRatio);
            //barRenderer.material.color = barColor;
            Color barColor = Color.Lerp(Color.red, Color.green, healthRatio);
            barImage.color = barColor;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 챔피언이 생성되었을 때 호출됩니다.
    /// </summary>
    /// <param name="_championGO"></param>
    public void Init(GameObject _championGO)
    {
        championGO = _championGO;
        championController = championGO.GetComponent<ChampionController>(); 
        //이 Init 메소드는 HealthBar 컴포넌트의 초기화를 담당합니다. 넘겨받은 _championGO를 championGO에 할당하여 해당 체력바가 어떤 챔피언에 대한 것인지 저장합니다. 또한, championGO로부터 ChampionController 컴포넌트를 가져와 championController에 할당합니다. 이렇게 함으로써 이후에 체력바가 챔피언의 상태를 감지하고 표시할 수 있게 됩니다.

    }
}
