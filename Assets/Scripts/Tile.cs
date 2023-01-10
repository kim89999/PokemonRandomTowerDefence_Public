using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsBuildTower { set; get; }

    private void Awake()
    {
        IsBuildTower = false;
    }
}

/*
 * File : Tile.cs
 * Desc
 *  : 타워 배치가 가능한 TileWall 오브젝트에 부착
 *  중복 배치 제어
 */
