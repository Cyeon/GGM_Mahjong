using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    protected TileType _tillType;
    [SerializeField]
    protected int _tileNumber;
    [SerializeField]
    protected bool _isAka = false;

    protected bool _isDora = false;
    protected bool _isBackDora = false;

    protected int _doraIdx = -1;
    protected int _backDoraIdx = -1;
}
