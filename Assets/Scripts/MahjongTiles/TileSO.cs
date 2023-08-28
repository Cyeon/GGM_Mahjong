using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/TileSO")]
public class TileSO : ScriptableObject
{
    public TileType _tileType;
    public int _tileNumber;
    public bool _isAka = false;
    public Sprite _tileSprite;
}
