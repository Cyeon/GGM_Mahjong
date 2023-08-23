using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private TileListSO _tileList = null;

    private List<Tile> _tiles = new List<Tile>(); // 패산 타일들  
    private List<Tile> _doraTiles = new List<Tile>(); // 도라 표시패 타일들

    /// <summary>
    /// 기본 타일 전체 생성 
    /// </summary>
    private void TileInit()
    {
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
    }

    /// <summary>
    /// 도라/뒷도라 설정 
    /// </summary>
    private void SetDora()
    {

    }
    

}