using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/TileSO")]
public class TileSO : ScriptableObject
{
    public TileType TileType;
    public int TileNumber;
    public bool IsAka = false;
    public Sprite TileSprite;
}
