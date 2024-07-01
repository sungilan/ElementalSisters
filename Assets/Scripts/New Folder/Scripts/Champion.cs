using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 챔피언 캐릭터의 모든 능력치와 정보를 저장합니다.
/// </summary>
[CreateAssetMenu(fileName = "DefaultChampion", menuName = "AutoChess/Champion", order = 1)] //Asset 메뉴에 추가합니다.
public class Champion : ScriptableObject
{
    ///게임에서 생성할 물리적 챔피언 프리팹
    public GameObject prefab;

    ///챔피언이 공격할 때 생성되는 발사체 프리팹
    public GameObject attackProjectile;

    ///UI 프레임에 표시되는 챔피언 이름
    public string uiname;

    ///챔피언 구매 가격
    public int cost;

    ///챔피언 타입1
    public ChampionType type1;

    ///챔피언 타입2
    public ChampionType type2;

    ///챔피언 타입3
    public ChampionType type3;

    ///챔피언 캐릭터의 시작 체력 포인트
    public float health = 100;

    ///공격 성공시 챔피언 캐릭터의 데미지
    public float damage = 10;

    ///챔피언 캐릭터의 방어력
    public float defense = 10;

    public float mana = 0;

    ///챔피언 캐릭터의 공격범위
    public float attackRange = 1;

    public Material cardMaterial;

    public GameObject attackEffectPrefab;

    public float attackEffectDuration;

    public GameObject skillEffectPrefab;
}


