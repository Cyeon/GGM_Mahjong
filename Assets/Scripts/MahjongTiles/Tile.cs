using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileSO _tileSO = null;

    protected bool _isDora = false;
    protected bool _isBackDora = false;

    protected int _doraIdx = -1;
    protected int _backDoraIdx = -1;
}