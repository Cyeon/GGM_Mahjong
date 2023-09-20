using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Tile : MonoBehaviour
{
    public TileSO TileSO = null;

    protected bool _isDora = false;
    public bool IsDora => _isDora;
    private bool _isBackDora = false;
    public bool IsBackDora => _isBackDora;

    private int _doraIdx = -1;
    private int _backDoraIdx = -1;

    public Button btn;

    public Hands hand;

    public void Awake()
    {
        btn = GetComponentInChildren<Button>();
        btn.onClick.AddListener(OnClickBtn);
    }

    public void OnClickBtn()
    {
        hand.RemoveTile(TileSO);
    }

    public void SetDora(bool isDora)
    {
        _isDora = isDora;
    }
    public void SetBackDora(bool isBackDora)
    {
        _isBackDora = isBackDora;
    }
}