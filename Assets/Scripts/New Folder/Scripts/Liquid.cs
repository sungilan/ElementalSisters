using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    
    [SerializeField] private GameObject liquid1;
    [SerializeField] private GameObject liquid2;
    [SerializeField] private GameObject liquid3;
    private GamePlayController gamePlayController;

    // Start is called before the first frame update
    void Start()
    {
        gamePlayController = GameObject.Find("Scripts").GetComponent<GamePlayController>();
        liquid();
    }
    private void liquid()
    {
        switch (gamePlayController.currentGold)
        {
            case int n when n >= 3:
                liquid1.SetActive(true);
                break;

            // �ٸ� ��� �翡 ���� ���� �߰�
            // case ������ �ش��ϴ� ��� ���� �ۼ��ϰ� ���ϴ� ������ �߰��մϴ�.
            // ��:
             case int n when n >= 20:
                liquid1.SetActive(false);
                liquid2.SetActive(true);
                break;
             case int n when n >= 30:
                liquid2.SetActive(false);
                liquid3.SetActive(true);
                break;

            default:
                // �⺻������ ������ ���� (��: ��� ���� ��Ȱ��ȭ)
                liquid1.SetActive(false);
                liquid2.SetActive(false);
                liquid3.SetActive(false);
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
