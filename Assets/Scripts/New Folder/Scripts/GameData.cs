using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores basic Game Data
/// </summary>
public class GameData : MonoBehaviour
{
    ///사용 가능한 모든 챔피언을 저장합니다. 모든 챔피언은 편집기에서 스크립트 게임 개체에 할당되어야 합니다.
    public Champion[] championsArray;

    ///사용 가능한 모든 챔피언 유형을 저장합니다. 모든 챔피언 유형은 편집기에서 스크립트 게임 개체에 할당되어야 합니다.
    public ChampionType[] championTypesArray; 
}
