using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/TileSO")]
[Serializable]
public class TileSO : ScriptableObject
{
    public TileType TileType;
    public int TileNumber;
    public bool IsAka = false;
    public Sprite TileSprite;
    
    public void SetData(TileType tileType, int tileNumber, bool aka = false)
    {
        TileType = tileType;
        TileNumber = tileNumber;
        IsAka = aka;
    }

    /// <summary>
    /// �ٷ� �� Ÿ������ üũ
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsNeighbour(TileSO tile) 
    {
        if (this.TileType == tile.TileType && (int)Mathf.Abs(this.TileNumber - tile.TileNumber) == 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �� ĭ ��� Ÿ������ üũ
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsSecondNeighbour(TileSO tile)
    {
        if (this.TileType == tile.TileType && (int)Mathf.Abs(this.TileNumber - tile.TileNumber) == 2)
        {
            return true;
        }
        return false;
    }

    public bool IsTerminalTile()
    {
        if (this.TileNumber != 1 || this.TileNumber != 9)
            return false;
        return true;
    }
}
