using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

/// <summary>
/// �밡�� �ִ� Ŭ����
/// </summary>
public class TilePair
{
    public TileSO _pairOne = null;
    public TileSO _pairTwo = null;
    public bool _isNeed = false;

/// <summary>
/// 
/// </summary>
/// <param name="one"></param>
/// <param name="two"></param>
/// <param name="isNeed">TWO�� �����и� true��</param>
    public TilePair(TileSO one, TileSO two, bool isNeed = false)
    {
        _pairOne = one;
        _pairTwo = two;
        _isNeed = isNeed;
    }
}

/// <summary>
/// ���� Ŀ�� �����ִ� Ŭ����
/// ���� ��� �� �صּ� ���߿� Ȯ���ϸ� �����ϱ� �ؾ���
/// </summary>
public class TileMeld
{
    public TileSO _tileOne = null;
    public TileSO _tileTwo = null;
    public TileSO _tileThree = null;
    public bool _isOpen = false;
    public bool _isNeed = false;

    public TileSO _needTile = null;
    /// <summary>
    /// ���� ��� ���µ� �����д� three�� �־������
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <param name="three"></param>
    /// <param name="isOpen">ġ �� ���� ��� TRUE. ����� ���� �����̴� false�� �θ� ��</param>
    /// <param name="isNeed">THREE�� �����ж�� true��</param>
    public TileMeld(TileSO one, TileSO two, TileSO three, bool isOpen = false, bool isNeed = false)
    {
        _tileOne = one;
        _tileTwo = two;
        _tileThree = three;
        _isOpen = isOpen;
        _isNeed = isNeed;

        if (isNeed)
            _needTile = three;

        SortTile();
    }

    /// <summary>
    /// Ÿ�� ����
    /// </summary>
    public void SortTile()
    {
        List<TileSO> list = new List<TileSO>();
        list.Add(_tileOne);
        list.Add(_tileTwo);
        list.Add(_tileThree);

        list = list.OrderBy(x => x.TileNumber).ToList();

        _tileOne = list[0];
        _tileTwo = list[1];
        _tileThree = list[2];
    }
}