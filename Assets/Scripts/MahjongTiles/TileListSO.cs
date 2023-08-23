using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/TileListSO")]
public class TileListSO : ScriptableObject
{
    public List<TileSO> TileList = new List<TileSO>();
    public List<TileSO> AkaTileList = new List<TileSO>();
}
