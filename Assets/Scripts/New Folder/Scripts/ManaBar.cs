using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays champion health over champion
/// </summary>
public class ManaBar : MonoBehaviour
{
    private GameObject championGO;
    private ChampionController championController;
    [SerializeField] private Image Mpimage;
    private float currentFillAmount;

    /// Start is called before the first frame update
    void Start()
    {
        currentFillAmount = Mpimage.fillAmount;
    }

    /// Update is called once per frame
    void Update()
    {
        if (championGO != null)
        {
            this.transform.position = championGO.transform.position + new Vector3(0, 4.2f, 0);
            // 챔피언의 현재 체력과 최대 체력 비율 계산
            float targetFillAmount = championController.currentMana / championController.maxMana;

            // 마나바 너비를 비율에 따라 변경
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, 5f * Time.deltaTime);
            Mpimage.fillAmount = currentFillAmount;

            Color barColor = Color.blue;
            Mpimage.color = barColor;

            // 챔피언의 체력이 0 이하가 되면 마나바를 파괴
            if (championController.currentHealth <= 0)
            {
                championGO = null;
                Destroy(this.gameObject);
            }
            if(championController.currentMana == championController.maxMana)
            {
                championController.currentMana = 0;
                championController.UseSkill();
            }
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
