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

            // 다른 골드 양에 따른 동작 추가
            // case 다음에 해당하는 골드 양을 작성하고 원하는 동작을 추가합니다.
            // 예:
             case int n when n >= 20:
                liquid1.SetActive(false);
                liquid2.SetActive(true);
                break;
             case int n when n >= 30:
                liquid2.SetActive(false);
                liquid3.SetActive(true);
                break;

            default:
                // 기본적으로 수행할 동작 (예: 모든 것을 비활성화)
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
