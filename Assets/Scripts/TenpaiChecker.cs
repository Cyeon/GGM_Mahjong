using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandReading
{
    public TilePair _pair = null;
    public List<TileMeld> _tileMelds = new List<TileMeld>(4);

    public HandReading(List<TileMeld> melds, TilePair pair)
    {
        _pair = pair;
        _tileMelds = melds;
    }
}

public class TenpaiChecker : MonoBehaviour
{
    [SerializeField]
    private List<TileSO> _handTiles = new List<TileSO>();
    [SerializeField]
    private TileListSO _13TileListSO = null;

    private List<TileSO> _tenpaiNeed = new List<TileSO>();


    private List<TileSO> _13Tiles = new List<TileSO>();

    public GameObject tenpai;

    private void Awake()
    {
        _13Tiles = _13TileListSO.TileList;
    }

    void Start()
    {
        if (_handTiles.Count == 13)
        {
            Check_AllTenpai(_handTiles);
        }
    }

    public void Check_AllTenpai(List<TileSO> list)
    {
        if (list.Count < 13)
        {
            Debug.Log("HandTile < 13");
            return;
        }
        _tenpaiNeed.Clear();

        if (Check_SevenPairs(list))
        {
            Debug.Log("Seven Pair Tenpai");
            PrintNeedTile();
        }
        else
            Debug.Log("No Seven Pair Tenpai");
        _tenpaiNeed.Clear();

        if (Check_ThirteenOrphans(list))
        {
            Debug.Log("13 word Tenpai");
            PrintNeedTile();
        }
        else
            Debug.Log("No 13 word Tenpai");
        _tenpaiNeed.Clear();

        if (Check_Tenpai(list))
        {
            Debug.Log("TENPAI");
            PrintNeedTile();
            tenpai.SetActive(true);
        }
        else
        {
            Debug.Log("NO TENPAI");
            tenpai.SetActive(false);
        }

    }

    private void PrintNeedTile()
    {
        for (int i = 0; i < _tenpaiNeed.Count; i++)
        {
            if (_tenpaiNeed[i] == null) continue;
            Debug.Log("Need Tile " + _tenpaiNeed[i].TileType + " " + _tenpaiNeed[i].TileNumber);
        }
    }

    /// <summary>
    /// 치또이 체크
    /// </summary>
    /// <returns>텐파이인지 아닌지</returns>
    private bool Check_SevenPairs(List<TileSO> list)
    {
        Dictionary<TileSO, int> dict = new Dictionary<TileSO, int>();

        TileSO tenpaiTile = null;

        for (int i = 0; i < list.Count; i++)
        {
            if (dict.ContainsKey(list[i]))
                dict[list[i]]++;
            else
                dict.Add(list[i], 1);
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

        _tenpaiNeed.Add(tenpaiTile);
        return true;
    }

    /// <summary>
    /// 국사무쌍 체크
    /// </summary>
    /// <returns>텐파이인지 아닌지</returns>
    private bool Check_ThirteenOrphans(List<TileSO> list)
    {
        Dictionary<TileSO, int> dict = new Dictionary<TileSO, int>();

        bool headInHand = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (_13Tiles.Contains(list[i]))
            {
                if (dict.ContainsKey(list[i]))
                    dict[list[i]]++;
                else
                    dict.Add(list[i], 1);
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


    private bool Check_Tenpai(List<TileSO> list)
    {
        List<HandReading> _list = RecursionCheck(list, new List<TileMeld>(), true);
        return RecursionCheck(list, new List<TileMeld>(), true).Count > 0;
    }

    private List<HandReading> RecursionCheck(List<TileSO> remainTiles, List<TileMeld> melds, bool isEarlyReturn)
    {
        remainTiles = remainTiles.OrderBy(x => x.TileType).ThenBy(x => x.TileNumber).ToList();

        List<HandReading> readings = new List<HandReading>();
        for (int i = 0; i < remainTiles.Count; i++)
        {
            if (i != 0 && remainTiles[i] == remainTiles[i - 1])
                continue;

            TileSO tile = remainTiles[i];

            List<TileSO> copy = new List<TileSO>();
            List<TileSO> meld = new List<TileSO>();

            foreach (TileSO t in remainTiles)
            {
                if (meld.Count < 3 && IsSameTile(t, tile))
                    meld.Add(t);
                else
                    copy.Add(t);
            }

            if (meld.Count == 3)
            {
                TileMeld tileMeld = new TileMeld(meld[0], meld[1], meld[2]);
                List<TileMeld> tileMelds = new List<TileMeld>();

                for (int l = 0; l < melds.Count; l++)
                {
                    tileMelds.Add(melds[i]);
                }
                tileMelds.Add(tileMeld);

                AppendReadingList(ref readings, RecursionCheck(copy, melds, isEarlyReturn));

                if (isEarlyReturn && readings.Count > 0)
                    return readings;
            }

            if (tile.TileType != TileType.Word && tile.TileNumber <= 7)
            {
                TileSO first = null;
                TileSO second = null;

                copy.Clear();

                foreach (TileSO t in remainTiles)
                {
                    if (t == tile || t.TileType != tile.TileType)
                        continue;
                    if (t.TileNumber - tile.TileNumber == 1 && first == null)
                    {
                        first = t;
                    }
                    else if (t.TileNumber - tile.TileNumber == 2 && second == null)
                    {
                        second = t;
                    }
                    else
                    {
                        copy.Add(t);
                    }
                }

                if (first != null && second != null)
                {
                    TileMeld tileMeld = new TileMeld(tile, first, second);

                    List<TileMeld> tileMelds = new List<TileMeld>();

                    for (int l = 0; l < melds.Count; l++)
                    {
                        tileMelds.Add(melds[i]);
                    }
                    tileMelds.Add(tileMeld);

                    AppendReadingList(ref readings, RecursionCheck(copy, tileMelds, isEarlyReturn));

                    if (isEarlyReturn && readings.Count > 0)
                    {
                        return readings;
                    }
                }
            }

            if (remainTiles.Count == 4)
            {
                int size = 4;

                TileSO t = null, n1 = null, n2 = null;

                if (IsSameTile(remainTiles[(i + 1) % size], remainTiles[(i + 2) % size]))
                {
                    n1 = remainTiles[(i + 1) % size];
                    n2 = remainTiles[(i + 2) % size];
                    t = remainTiles[(i + 3) % size];
                }
                else if (IsSameTile(remainTiles[(i + 1) % size], remainTiles[(i + 3) % size]))
                {
                    n1 = remainTiles[(i + 1) % size];
                    t = remainTiles[(i + 2) % size];
                    n2 = remainTiles[(i + 3) % size];
                }
                else if (IsSameTile(remainTiles[(i + 2) % size], remainTiles[(i + 3) % size]))
                {
                    t = remainTiles[(i + 1) % size];
                    n1 = remainTiles[(i + 2) % size];
                    n2 = remainTiles[(i + 3) % size];
                }

                if (t != null)
                {
                    if (IsSameTile(t, tile))
                    {
                        TilePair pair = new TilePair(tile, t);
                        List<TileMeld> tileMelds = new List<TileMeld>();

                        for (int l = 0; l < melds.Count; l++)
                        {
                            tileMelds.Add(melds[i]);
                        }
                        tileMelds.Add(new TileMeld(n1, n2, n1, isNeed: true));
                        _tenpaiNeed.Add(n1);

                        HandReading reading = new HandReading(tileMelds, pair);
                        // 역없음 체크를 해야하는데 일단 패스 (멘젠 리치 가정)
                        AppendReading(ref readings, reading);

                        pair = new TilePair(n1, n2);
                        tileMelds.Clear();

                        for (int l = 0; l < melds.Count; l++)
                        {
                            tileMelds.Add(melds[i]);
                        }
                        tileMelds.Add(new TileMeld(tile, t, tile, isNeed: true));
                        _tenpaiNeed.Add(tile);

                        reading = new HandReading(tileMelds, pair);
                        AppendReading(ref readings, reading);

                        return readings;
                    }
                    else if (tile.IsNeighbour(t) || tile.IsSecondNeighbour(t))
                    {
                        TilePair pair = new TilePair(n1, n2);
                        List<TileMeld> tileMelds = new List<TileMeld>();
                        for (int l = 0; l < melds.Count; l++)
                        {
                            tileMelds.Add(melds[l]);
                        }

                        if (tile.IsSecondNeighbour(t)) // 간짱대기
                        {
                            int middle = (tile.TileNumber + t.TileNumber) / 2;

                            TileSO SO = new TileSO();
                            SO.SetData(tile.TileType, middle);
                            tileMelds.Add(new TileMeld(tile, t, SO, isNeed: true));
                            _tenpaiNeed.Add(SO);
                            HandReading reading = new HandReading(tileMelds, pair);

                            // 역없음 체크 해야하는데 패스
                            AppendReading(ref readings, reading);
                        }
                        else
                        {
                            TileSO one;
                            TileSO two;
                            if (tile.TileNumber > t.TileNumber)
                            {
                                one = t;
                                two = tile;
                            }
                            else
                            {
                                one = tile;
                                two = t;
                            }

                            TileSO SO = new TileSO();
                            if (one.TileNumber != 1 && one.TileNumber != 9)
                            {
                                SO.SetData(one.TileType, one.TileNumber - 1);
                                tileMelds.Add(new TileMeld(one, two, SO, isNeed: true));
                                _tenpaiNeed.Add(SO);

                                HandReading reading = new HandReading(tileMelds, pair);
                                AppendReading(ref readings, reading);
                            }

                            if (two.TileNumber != 1 && two.TileNumber != 9)
                            {
                                tileMelds.Clear();
                                for (int l = 0; l < melds.Count; l++)
                                {
                                    tileMelds.Add(melds[i]);
                                }
                                SO.SetData(one.TileType, two.TileNumber + 1);

                                tileMelds.Add(new TileMeld(one, two, SO, isNeed: true));
                                _tenpaiNeed.Add(SO);

                                HandReading reading = new HandReading(tileMelds, pair);
                                AppendReading(ref readings, reading);
                            }
                        }
                        return readings;

                    }
                }
            }
        }
        return readings;
    }

    private void AppendReadingList(ref List<HandReading> readings, List<HandReading> appendList)
    {
        foreach (HandReading item in appendList)
        {
            AppendReading(ref readings, item);
        }
    }

    private void AppendReading(ref List<HandReading> readings, HandReading read)
    {
        foreach (HandReading item in readings)
        {
            if (item == read)
            {
                return;
            }
        }

        readings.Add(read);
    }

    private bool IsSameTile(TileSO tileOne, TileSO tileTwo)
    {
        if (tileOne.TileType == tileTwo.TileType && tileOne.TileNumber == tileTwo.TileNumber)
            return true;
        return false;
    }
}