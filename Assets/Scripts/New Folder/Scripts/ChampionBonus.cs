using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChampionBonusType {Damage, Defense, Stun, Heal};
public enum BonusTarget {Self, Enemy};

/// <summary>
/// 동일한 유형의 챔피언이 충분할 때 얻을 수 있는 보너스를 제어합니다.
/// </summary>
[System.Serializable]
public class ChampionBonus
{
    ///보너스 효과를 얻기위해 필요한 챔피언의 수
    public int championCount = 0;

    ///보너스 타입
    public ChampionBonusType championBonusType;

    ///보너스를 받을 타겟
    public BonusTarget bonusTarget;

    ///보너스 수치 (Damage 타입 : 데미지량, Heal 타입 : 힐량, stun 타입 : 스턴 확률, Defence 타입 : 방어력)
    public float bonusValue = 0;

    ///How many secounds bonus lasts
    public float duration;

    ///보너스가 발생할 때 생성될 프리팹
    public GameObject effectPrefab;

    /// <summary>
    /// 공격 시 챔피언의 보너스를 계산합니다.
    /// </summary>
    /// <param name="champion"></param>
    /// <param name="targetChampion"></param>
    /// <returns></returns>
    public float ApplyOnAttack(ChampionController champion, ChampionController targetChampion)
    {
        
        float bonusDamage = 0;
        bool addEffect = false;
        switch (championBonusType)
        {
            case ChampionBonusType.Damage : //챔피언 보너스 타입이 Damage일 때
                bonusDamage += bonusValue; //보너스 수치만큼 bonusDamage에 추가(bonusValue가 곧 데미지양)
                break;
            case ChampionBonusType.Stun: //챔피언 보너스 타입이 Stun일 때
                int rand = Random.Range(0, 100); //0~100의 
                if (rand < bonusValue) //bonusValue가 rand보다 크면 ex) bonusValue가 60이면 60/100의 확률로 스턴 실행
                {
                    targetChampion.OnGotStun(duration); //스턴을 건다(스턴 시간만큼)
                    addEffect = true; // 스턴 이펙트
                }
                break;
            case ChampionBonusType.Heal: //챔피언 보너스 타입이 Heal일 때
                champion.OnGotHeal(bonusValue); //bonusValue만큼 힐
                addEffect = true; //힐 이펙트
                break;
            default:
                break;
        }


        if (addEffect)
        {
            if (bonusTarget == BonusTarget.Self)
               champion.AddEffect(effectPrefab, duration);
            else if (bonusTarget == BonusTarget.Enemy)
               targetChampion.AddEffect(effectPrefab, duration);
        }
      

        return bonusDamage;
    }

    /// <summary>
    /// 데미지를 입을 때 챔피언의 보너스를 계산합니다.
    /// </summary>
    /// <param name="champion"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    public float ApplyOnGotHit(ChampionController champion, float damage)
    {
        switch (championBonusType)
        {        
            case ChampionBonusType.Defense: //챔피언 보너스 타입이 Defense일 때
                damage = ((100 - bonusValue) / 100) * damage;
                break;   
            default:
                break;
        }

        return damage;
    }
}
