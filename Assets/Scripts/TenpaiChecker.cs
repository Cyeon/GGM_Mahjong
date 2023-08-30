using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenpaiChecker : MonoBehaviour
{
    [SerializeField]
    private List<TileSO> _handTiles = new List<TileSO>();
    [SerializeField]
    private TileListSO _13TileListSO = null;

    private List<TileSO> _tenpaiNeed = new List<TileSO>();


    private List<TileSO> _13Tiles = new List<TileSO>();

    private void Awake()
    {
        _13Tiles = _13TileListSO.TileList;

    }

    void Start()
    {
        if (Check_SevenPairs())
        {
            Debug.Log("Seven Pair Tenpai");
            PrintNeedTile();
        }
        else
            Debug.Log("No Seven Pair Tenpai");

        if (Check_ThirteenOrphans())
        {
            Debug.Log("13 word Tenpai");
            PrintNeedTile();
        }
        else
            Debug.Log("No 13 word Tenpai");
    }

    private void PrintNeedTile()
    {
        for (int i = 0; i < _tenpaiNeed.Count; i++)
        {
            Debug.Log("Need Tile " + _tenpaiNeed[i].TileType + " " + _tenpaiNeed[i].TileNumber);
        }
    }

    /// <summary>
    /// 치또이 체크
    /// </summary>
    /// <returns>텐파이인지 아닌지</returns>
    private bool Check_SevenPairs()
    {
        Dictionary<TileSO, int> dict = new Dictionary<TileSO, int>();

        TileSO tenpaiTile = null;

        for (int i = 0; i < _handTiles.Count; i++)
        {
            if (dict.ContainsKey(_handTiles[i]))
                dict[_handTiles[i]]++;
            else
                dict.Add(_handTiles[i], 1);
        }

        foreach (TileSO item in dict.Keys)
        {
            if (dict[item] != 2)
            {
                if (dict[item] == 1 && tenpaiTile == null)
                {
                    tenpaiTile = item;
                    continue;
                }

                return false;
            }
        }

        _tenpaiNeed.Clear();
        _tenpaiNeed.Add(tenpaiTile);
        return true;
    }

    /// <summary>
    /// 국사무쌍 체크
    /// </summary>
    /// <returns>텐파이인지 아닌지</returns>
    private bool Check_ThirteenOrphans()
    {
        Dictionary<TileSO, int> dict = new Dictionary<TileSO, int>();

        bool headInHand = false;

        for (int i = 0; i < _handTiles.Count; i++)
        {
            if (_13Tiles.Contains(_handTiles[i]))
            {
                if (dict.ContainsKey(_handTiles[i]))
                    dict[_handTiles[i]]++;
                else
                    dict.Add(_handTiles[i], 1);
            }
            else
                return false;
        }

        foreach (TileSO item in dict.Keys)
        {
            if (dict[item] > 2)
            {
                return false;
            }
            else if (dict[item] == 2)
            {
                if (headInHand)
                    return false;
                headInHand = true;
            }
        }

        _tenpaiNeed.Clear();

        if (headInHand)
        {
            for (int i = 0; i < _13Tiles.Count; i++)
            {
                if (!dict.ContainsKey(_13Tiles[i]))
                {
                    _tenpaiNeed.Add(_13Tiles[i]);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < _13Tiles.Count; i++)
            {
                _tenpaiNeed.Add(_13Tiles[i]);
            }
        }

        return true;
    }

}