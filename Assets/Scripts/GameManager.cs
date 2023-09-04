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
    private Queue<TileSO> _tileQueue = new Queue<TileSO>();
    private List<TileSO> _doraTiles = new List<TileSO>(); // 도라 표시패 타일들
    private List<TileSO> _backDoraTiles = new List<TileSO>();

    private void Awake()
    {
        TileInit();
    }

    /// <summary>
    /// 기본 타일 전체 생성 
    /// </summary>
    private void TileInit()
    {
        _tiles.Clear();
        _tileQueue.Clear();
        for (int i = 0; i < _tileList.TileList.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                TileSO tileSO =  _tileList.TileList[i];
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
        return _tileQueue.Dequeue();
    }
}