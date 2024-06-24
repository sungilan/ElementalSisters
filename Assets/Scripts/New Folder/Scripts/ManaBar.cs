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
    //private Renderer barRenderer; // ü�¹� ������
    private Image barImage;

    private float initialScaleX; // �ʱ� ü�¹� �ʺ�

    /// Start is called before the first frame update
    void Start()
    {
        //barRenderer = GetComponent<Renderer>(); // ü�¹��� ������ ������Ʈ ��������
        barImage = GetComponent<Image>();
        initialScaleX = transform.localScale.x; // �ʱ� ü�¹� �ʺ� ����
    }

    /// Update is called once per frame
    void Update()
    {
        if (championGO != null)
        {
            this.transform.position = championGO.transform.position + new Vector3(0, 4.2f, 0);
            // è�Ǿ��� ���� ü�°� �ִ� ü�� ���� ���
            float manaRatio = championController.currentMana / championController.maxMana;

            // ü�¹� �ʺ� ������ ���� ����
            Vector3 newScale = transform.localScale;
            newScale.x = initialScaleX * manaRatio;
            transform.localScale = newScale;

            // è�Ǿ��� ü�¿� ���� ���� ����
            Color barColor = Color.Lerp(Color.red, Color.blue, manaRatio);
            barImage.color = barColor;

            // è�Ǿ��� ü���� 0 ���ϰ� �Ǹ� �����ٸ� �ı�
            if (championController.currentHealth <= 0)
            {
                championGO = null;
                Destroy(this.gameObject);
                Debug.Log("������ ����");
            }
        }
    }

    /// <summary>
    /// è�Ǿ��� �����Ǿ��� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="_championGO"></param>
    public void Init(GameObject _championGO)
    {
        championGO = _championGO;
        championController = championGO.GetComponent<ChampionController>();
        //�� Init �޼ҵ�� HealthBar ������Ʈ�� �ʱ�ȭ�� ����մϴ�. �Ѱܹ��� _championGO�� championGO�� �Ҵ��Ͽ� �ش� ü�¹ٰ� � è�Ǿ� ���� ������ �����մϴ�. ����, championGO�κ��� ChampionController ������Ʈ�� ������ championController�� �Ҵ��մϴ�. �̷��� �����ν� ���Ŀ� ü�¹ٰ� è�Ǿ��� ���¸� �����ϰ� ǥ���� �� �ְ� �˴ϴ�.

    }
}
