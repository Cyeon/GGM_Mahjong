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

    private List<Tile> _tiles = new List<Tile>(); // 패산 타일들  
    private Queue<Tile> _tileQueue = new Queue<Tile>();
    private List<Tile> _doraTiles = new List<Tile>(); // 도라 표시패 타일들
    private List<Tile> _backDoraTiles = new List<Tile>();

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
                Tile tile = new Tile();
                tile._tileSO = _tileList.TileList[i];
                _tiles.Add(tile);
            }
        }

        for (int i = 0; i < _tileList.AkaTileList.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Tile tile = new Tile();
                tile._tileSO = _tileList.AkaTileList[i];
                _tiles.Add(tile);

                if (_tileList.AkaTileList[i]._isAka)
                    break;
            }
        }

        Shuffle();

        foreach (Tile tile in _tiles)
        {
            _tileQueue.Enqueue(tile);
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
            Tile value = _tiles[k];
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
            Tile tile = _tileQueue.Dequeue();
            tile.SetDora(true);
            _doraTiles.Add(tile);

            tile = _tileQueue.Dequeue();
            tile.SetBackDora(true);
            _backDoraTiles.Add(tile);
        }
    }
}