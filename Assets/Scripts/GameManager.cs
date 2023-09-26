using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private TileListSO _tileList = null;

    private static System.Random rng = new System.Random();

    [SerializeField]
    private List<TileSO> _tiles = new List<TileSO>(); // 패산 타일들  
    [SerializeField]
    private Queue<TileSO> _tileQueue = new Queue<TileSO>();
    private List<TileSO> _doraTiles = new List<TileSO>(); // 도라 표시패 타일들
    private List<TileSO> _backDoraTiles = new List<TileSO>();

    private int _turnCount = 1;
    public int TurnCount => _turnCount;

    private void Awake()
    {
        TileInit();
    }

    /// <summary>
    /// 기본 타일 전체 생성 
    /// </summary>
    public void TileInit()
    {
        _tiles.Clear();
        _tileQueue.Clear();
        for (int i = 0; i < _tileList.TileList.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                TileSO tileSO = _tileList.TileList[i];
                _tiles.Add(tileSO);
            }
        }

        for (int i = 0; i < _tileList.AkaTileList.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                TileSO tileSO = _tileList.AkaTileList[i];
                _tiles.Add(tileSO);

                if (_tileList.AkaTileList[i].IsAka)
                    break;
            }
        }

        Shuffle();

        foreach (TileSO tile in _tiles)
        {
            _tileQueue.Enqueue(tile);
            Debug.Log(tile.TileType + tile.TileNumber);
        }

        SetDora();
    }

    public void RemoveTile(TileSO tile, Tile tileObj)
    {
        if (tile == null)
        {
            Debug.LogError("보내진 타일이 null입니다.");
        }
        _tiles.Remove(tile);

        TileSO newTile = _tileQueue.Dequeue();
        Debug.Log(newTile.TileNumber);
        _tiles.Add(newTile);
        tileObj.TileSO = newTile;


    }

    private void Shuffle()
    {
        int n = _tiles.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            TileSO value = _tiles[k];
            _tiles[k] = _tiles[n];
            _tiles[n] = value;
        }
    }

    /// <summary>
    /// 도라/뒷도라 설정 
    /// </summary>
    private void SetDora()
    {
        for (int i = 0; i < 5; i++)
        {
            TileSO tile = _tileQueue.Dequeue();
            _doraTiles.Add(tile);

            tile = _tileQueue.Dequeue();
            _backDoraTiles.Add(tile);
        }
    }

    public TileSO PickUp()
    {
        _turnCount++;
        return _tileQueue.Dequeue();
    }

    public TileSO GetTile(TileType type, int number)
    {
        for (int i = 0; i < _tileList.TileList.Count; i++)
        {
            if (IsSameTile(_tileList.TileList[i], type, number))
            {
                return _tileList.TileList[i];
            }
        }

        for (int i = 0; i < _tileList.AkaTileList.Count; i++)
        {
            if (IsSameTile(_tileList.AkaTileList[i], type, number))
            {
                return _tileList.AkaTileList[i];
            }
        }


        return null;
    }

    public bool IsSameTile(TileSO tileOne, TileSO tileTwo)
    {
        if (tileOne.TileType == tileTwo.TileType && tileOne.TileNumber == tileTwo.TileNumber)
            return true;
        return false;
    }

    public bool IsSameTile(TileSO tile, TileType tileTwoType, int tileTwoNumber)
    {
        if (tile.TileType == tileTwoType && tile.TileNumber == tileTwoNumber && !tile.IsAka)
            return true;
        return false;
    }

    public void ResetTurn()
    {
        _turnCount = 1;
    }
}