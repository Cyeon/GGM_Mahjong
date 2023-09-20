using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/TileSO")]
public class TileSO : ScriptableObject
{
    public TileType _tileType;
    public int _tileNumber;
    public bool _isAka = false;

    public void SetData(TileType tileType, int tileNumber, bool aka = false)
    {
        _tileType = tileType;
        _tileNumber = tileNumber;
        _isAka = aka;
    }

    /// <summary>
    /// �ٷ� �� Ÿ������ üũ
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsNeighbour(TileSO tile) 
    {
        if (this._tileType == tile._tileType && (int)Mathf.Abs(this._tileNumber - tile._tileNumber) == 1)
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
        if (this._tileType == tile._tileType && (int)Mathf.Abs(this._tileNumber - tile._tileNumber) == 2)
        {
            return true;
        }
        return false;
    }

}
