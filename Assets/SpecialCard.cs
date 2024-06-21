using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCard : MonoBehaviour
{
    [SerializeField] private GamePlayController gamePlayController;
    [SerializeField] private GameObject selectEffect;
    private GameObject currentEffect;
    public ChampionBonus championBonus;
    bool isDelete;

    public string cardName;
    public string cardExplain;
    public Sprite cardIcon;  

    private void Start()
    {
        gamePlayController = GameObject.Find("Scripts").GetComponent<GamePlayController>();
    }

    public void DestroySpecialCard()
    { 
        Destroy(this.gameObject);
        Destroy(currentEffect);
        GameObject.Find("Scripts").GetComponent<CreateSpecialCard>().DestroyAllSpecialCards();
    }

    public void SelectSpecialCard()
    {
        if(isDelete == false)
        {
            isDelete = true;
            gamePlayController.ApplyChampionBonus(championBonus);
            currentEffect = Instantiate(selectEffect, this.gameObject.transform.position, Quaternion.identity);
            Invoke("DestroySpecialCard", 1f);
        }
    }
}