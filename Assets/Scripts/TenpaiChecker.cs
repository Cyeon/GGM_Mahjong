using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenpaiChecker : MonoBehaviour
{
    [SerializeField]
    private List<TileSO> _handTiles = new List<TileSO>();

    private List<TileSO> _tenpaiNeed = new List<TileSO>();


    private List<TileSO> _wordTiles = new List<TileSO>();

    private void Awake()
    {
        _wordTiles = Resources.Load<TileListSO>("Resource/WordTilesListSO").TileList;
    }

    void Start()
    {
        if (Check_SevenPairs())
        {
            Debug.Log("Tenpai");
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
            dict[_handTiles[i]]++;
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
            if (_handTiles[i]._tileType != TileType.Word)
                return false;

            dict[_handTiles[i]]++;
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
            for (int i = 0; i < _wordTiles.Count; i++)
            {
                if (!dict.ContainsKey(_wordTiles[i]))
                {
                    _tenpaiNeed.Add(_wordTiles[i]);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < _wordTiles.Count; i++)
            {
                _tenpaiNeed.Add(_wordTiles[i]);
            }
        }

        return true;
    }
}