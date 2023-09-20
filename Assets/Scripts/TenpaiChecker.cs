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

    private List<HandReading> _handReadings = new List<HandReading>();

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

        if (Check_Tenpai())
        {
            Debug.Log("TENPAI");
            PrintNeedTile();
        }
        else
            Debug.Log("NO TENPAI");
    }

    private void PrintNeedTile()
    {
        for (int i = 0; i < _tenpaiNeed.Count; i++)
        {
            Debug.Log("Need Tile " + _tenpaiNeed[i]._tileType + " " + _tenpaiNeed[i]._tileNumber);
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


    private bool Check_Tenpai()
    {
        _tenpaiNeed.Clear();

        return RecursionCheck(_handTiles, new List<TileMeld>(), true).Count > 0;
    }

    private List<HandReading> RecursionCheck(List<TileSO> remainTiles, List<TileMeld> melds, bool isEarlyReturn)
    {
        remainTiles = remainTiles.OrderBy(x => x._tileType).ThenBy(x => x._tileNumber).ToList();

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

            if (tile._tileType != TileType.Word && tile._tileNumber <= 7)
            {
                TileSO first = null;
                TileSO second = null;

                copy.Clear();

                foreach (TileSO t in remainTiles)
                {
                    if (t == tile || t._tileType != tile._tileType)
                        continue;
                    if (t._tileNumber - tile._tileNumber == 1 && first == null)
                    {
                        first = t;
                    }
                    else if (t._tileNumber - tile._tileNumber == 2 && second == null)
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
                            int middle = (tile._tileNumber + t._tileNumber) / 2;

                            TileSO SO = new TileSO();
                            SO.SetData(tile._tileType, middle);
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
                            if (tile._tileNumber > t._tileNumber)
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
                            if (one._tileNumber != 1 && one._tileNumber != 9)
                            {
                                SO.SetData(one._tileType, one._tileNumber - 1);
                                tileMelds.Add(new TileMeld(one, two, SO, isNeed: true));
                                _tenpaiNeed.Add(SO);

                                HandReading reading = new HandReading(tileMelds, pair);
                                AppendReading(ref readings, reading);
                            }

                            if (two._tileNumber != 1 && two._tileNumber != 9)
                            {
                                tileMelds.Clear();
                                for (int l = 0; l < melds.Count; l++)
                                {
                                    tileMelds.Add(melds[i]);
                                }
                                SO.SetData(one._tileType, two._tileNumber + 1);

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
        if (tileOne._tileType == tileTwo._tileType && tileOne._tileNumber == tileTwo._tileNumber)
            return true;
        return false;
    }
}