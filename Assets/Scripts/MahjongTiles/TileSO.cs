using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/TileSO")]
public class TileSO : ScriptableObject
{
    public TileType _tillType;
    public int _tileNumber;
    public bool _isAka = false;

    public void SetData(TileType tileType, int tileNumber, bool aka = false)
    {
        _tillType = tileType;
        _tileNumber = tileNumber;
        _isAka = aka;
    }
}
