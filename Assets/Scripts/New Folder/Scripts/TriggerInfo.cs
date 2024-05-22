using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지도에서 플레이어 상호 작용을 식별하는 도우미 클래스
/// </summary>
public class TriggerInfo : MonoBehaviour
{
    ///The type of the grid, GRIDTYPE_OWN_INVENTORY = 0 GRIDTYPE_OPONENT_INVENTORY = 1 GRIDTYPE_HEXA_MAP = 2;
    public int gridType = -1;

    ///X position on the grid
    public int gridX = -1;

    ///Z position on the grid
    public int gridZ = -1;

}
