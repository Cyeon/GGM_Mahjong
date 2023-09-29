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

    private static List<TileSO> _tenpaiNeed = new List<TileSO>();
    public List<TileSO> TenpaiNeed => _tenpaiNeed;

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
            tenpai.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("No Seven Pair Tenpai");
            tenpai.SetActive(false);
        }
        _tenpaiNeed.Clear();

        if (Check_ThirteenOrphans(list))
        {
            Debug.Log("13 word Tenpai");
            PrintNeedTile();
            tenpai.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("No 13 word Tenpai");
            tenpai.SetActive(false);
        }
        _tenpaiNeed.Clear();

        if (Check_Tenpai(list))
        {
            Debug.Log("TENPAI");
            PrintNeedTile();
            tenpai.SetActive(true);
            return;
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
            {
                bool isAdd = false;

                if (list[i].TileNumber == 5 && list[i].TileType != TileType.Word)
                {
                    foreach (TileSO item in dict.Keys)
                    {
                        if (GameManager.Instance.IsSameTile(item, list[i]))
                        {
                            dict[item]++;
                            isAdd = true;
                            break;
                        }
                    }
                }

                if (!isAdd)
                    dict.Add(list[i], 1);
            }
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
        List<HandReading> tileList = HandReadingRecursion(list, new List<TileMeld>(), true, false);
        if (tileList.Count > 0)
        {
            foreach (HandReading reading in tileList)
            {
                if (reading._pair._isNeed)
                {
                    if (!_tenpaiNeed.Contains(reading._pair._pairTwo))
                    {
                        _tenpaiNeed.Add(reading._pair._pairTwo);
                    }
                }
                for (int i = 0; i < reading._tileMelds.Count; i++)
                {
                    if (reading._tileMelds[i]._isNeed)
                    {
                        if (!_tenpaiNeed.Contains(reading._tileMelds[i]._needTile))
                        {
                            _tenpaiNeed.Add(reading._tileMelds[i]._needTile);
                        }

                    }
                }
            }

            return true;
        }
        return false;
    }

    private static void AppendReadingList(ref List<HandReading> readings, List<HandReading> appendList)
    {
        foreach (HandReading item in appendList)
        {
            AppendReading(ref readings, item);
        }
    }

    private static void AppendReading(ref List<HandReading> readings, HandReading read)
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

    private static List<HandReading> HandReadingRecursion(List<TileSO> remainTileList, List<TileMeld> melds, bool tenpaiOnly, bool earlyReturn)
    {
        List<HandReading> readings = new List<HandReading>();

        // 복사하여 목록 정렬
        List<TileSO> hand = remainTileList.OrderBy(x => x.TileType).ThenBy(x => x.TileNumber).ToList();

        // 텐파이 뿐만 아니라 화료도 체크하기 위해서 tenpaiOnly 사용, 13과 14의 핸드 타일 경우의 수 나눔
        if ((tenpaiOnly && (hand.Count % 3 != 1 || hand.Count > 13)) ||
            (!tenpaiOnly && (hand.Count % 3 != 2 || hand.Count > 14)))
            return readings;
        else if (hand.Count == 1) // 머리 패 하나 기다리고 있는 경우
        {
            TileSO t = hand[0];
            TilePair pair = new TilePair(t, t, true);

            HandReading reading = new HandReading(melds, pair);
            AppendReading(ref readings, reading);

            return readings;
        }
        else if (hand.Count == 2) // 이건 패 14개인 경우에 머리 완성시키는 용도
        {
            if (GameManager.Instance.IsSameTile(hand[0], hand[1])) // 만약 타입이 같으면 
            {
                TilePair pair = new TilePair(hand[0], hand[1]);

                HandReading reading = new HandReading(melds, pair);
                //if (reading.valid_keishiki) // 역없음 or 없는 타일 기다리는지 체크
                AppendReading(ref readings, reading);
            }

            return readings;
        }

        // 핸드 사이즈대로 돌아감 
        for (int i = 0; i < hand.Count; i++)
        {
            if (i != 0 && GameManager.Instance.IsSameTile(hand[i], hand[i - 1])) // 이전 타일과 같은 타일이면 컨티뉴 
                continue;

            TileSO tile = hand[i];

            List<TileSO> meld = new List<TileSO>(); // 몸통 담을 리스트 
            List<TileSO> copy = new List<TileSO>(); // 몸통 만들고 남은 타일 리스트

            // 핸드 타일을 다 돌아 tile과 같으면 meld에 넣고 아니면 copy에 넣어준다 
            // 그리하여 같은 것이 3개 모인 몸통을 찾는다
            // ex. (2, 2, 2) 같은 경우
            foreach (TileSO t in hand)
            {
                if (meld.Count < 3 && GameManager.Instance.IsSameTile(t, tile))
                    meld.Add(t);
                else
                    copy.Add(t);
            }

            if (meld.Count == 3) // 몸통 찾았으면 meld에 추가해줌
            {
                TileMeld m = new TileMeld(meld[0], meld[1], meld[2]);

                List<TileMeld> newMelds = new List<TileMeld>();

                foreach (var item in melds)
                {
                    newMelds.Add(item);
                }

                newMelds.Add(m);

                // 다시 재귀로 호출
                AppendReadingList(ref readings, HandReadingRecursion(copy, newMelds, tenpaiOnly, earlyReturn));

                if (earlyReturn && readings.Count > 0) // 만약 패 완성되면 리턴  
                    return readings;
            }
            // 1~7 까지의 수패인 경우
            // 연속하는 행을 만들 수 있는지 체크하는 구간
            // ex. (1, 2, 3) or (5, 6, 7) 등
            if (tile.TileType != TileType.Word && tile.TileNumber <= 7)
            {

                TileSO one_more = null;
                TileSO two_more = null;
                copy.Clear();

                bool isFirst = true;

                foreach (TileSO t in hand) // tile이랑 연속하는지 숫자 비교해서 차이나는 대로 one two에 넣어줌
                {
                    if (t == tile && isFirst)
                    {
                        isFirst = false;
                        continue;
                    }
                    if (t.TileType == tile.TileType && t.TileNumber - tile.TileNumber == 1 && one_more == null)
                        one_more = t;
                    else if (t.TileType == tile.TileType && t.TileNumber - tile.TileNumber == 2 && two_more == null)
                        two_more = t;
                    else
                        copy.Add(t);
                }

                if (one_more != null && two_more != null) // 둘 다 안 비어잇으면 
                {
                    TileMeld m = new TileMeld(tile, one_more, two_more); // 묶어줌

                    List<TileMeld> newMelds = new List<TileMeld>();
                    foreach (var item in melds)
                    {
                        newMelds.Add(item);
                    }
                    newMelds.Add(m); // 이번에 찾은 연속 넣어주고 

                    // 재귀 돌리고
                    AppendReadingList(ref readings, HandReadingRecursion(copy, newMelds, tenpaiOnly, earlyReturn));

                    if (earlyReturn && readings.Count > 0) // 패 완성되었으면 리턴
                        return readings;
                }
            }

            // 만약 남은 손패가 4개인 경우 머리 or 완성할 몸통 찾는 구간
            if (hand.Count == 4)
            {
                int s = hand.Count;
                TileSO t = null;
                TileSO n1 = null, n2 = null;

                // n1, n2가 머리고 같은 타일 찾아서 넣어주는 과정 
                if (GameManager.Instance.IsSameTile(hand[(i + 1) % s], hand[(i + 2) % s])) // 아무튼 1번타일 하고 2번 타일 타입 같으면 
                {
                    n1 = hand[(i + 1) % s];
                    n2 = hand[(i + 2) % s];
                    t = hand[(i + 3) % s];
                }
                else if (GameManager.Instance.IsSameTile(hand[(i + 1) % s], hand[(i + 3) % s])) // 1 3 같으면 
                {
                    n1 = hand[(i + 1) % s];
                    t = hand[(i + 2) % s];
                    n2 = hand[(i + 3) % s];
                }
                else if (GameManager.Instance.IsSameTile(hand[(i + 2) % s], hand[(i + 3) % s])) // 2 3 같으면 
                {
                    t = hand[(i + 1) % s];
                    n1 = hand[(i + 2) % s];
                    n2 = hand[(i + 3) % s];
                }

                if (t != null) // 머리가 찾아져서 t가 null이 아니면 
                {
                    if (GameManager.Instance.IsSameTile(t, tile)) // 남은 두 개의 패 비교
                    {
                        // 기다리는 4개의 패가 (2, 2) , (4, 4) 처럼 두 개의 머리를 만들 수 있는 경우 

                        TilePair pair = new TilePair(tile, t); // 같으면 머리 만들어서 넣어주기
                        List<TileMeld> newMelds = new List<TileMeld>();

                        foreach (var item in melds)
                        {
                            newMelds.Add(item);
                        }

                        newMelds.Add(new TileMeld(n1, n2, n1, isNeed: true));
                        HandReading reading = new HandReading(newMelds, pair);
                        //if (reading.valid_keishiki) // 역없음 검사를 시킨 뒤 
                        AppendReading(ref readings, reading);

                        pair = new TilePair(n1, n2); // n1 n2로도 페어 만들어서 넣어줌 
                        newMelds.Clear();
                        foreach (var item in melds)
                        {
                            newMelds.Add(item);
                        }
                        //newMelds.Add_all(melds);
                        newMelds.Add(new TileMeld(tile, t, tile, isNeed: true));
                        reading = new HandReading(newMelds, pair);
                        //if (reading.valid_keishiki)
                        AppendReading(ref readings, reading);

                        return readings; // 완성 패 리턴 
                    }
                    else if (tile.IsNeighbour(t) || tile.IsSecondNeighbour(t)) // 기다리는 패 4개가 머리 하나와 몸통을 만들 수 있는 하나라면 
                    {
                        TilePair pair = new TilePair(n1, n2); // n1, n2로 머리 페어 만들고 

                        int v1 = (int)t.TileNumber;
                        int v2 = (int)tile.TileNumber;

                        TileSO t1, t2; // 숫자가 더 큰 타일이 2가 되도록 대입 해주는 if
                        if (v1 < v2)
                        {
                            t1 = t;
                            t2 = tile;
                        }
                        else
                        {
                            t1 = tile;
                            t2 = t;
                        }

                        // 남은 하나의 몸통 타일을 계산하기 위해 필요
                        v1 = (int)t1.TileNumber;
                        v2 = (int)t2.TileNumber;

                        List<TileMeld> newMelds = new List<TileMeld>();

                        foreach (var item in melds)
                        {
                            newMelds.Add(item);
                        }

                        if (tile.IsSecondNeighbour(t)) // 만약 간짱대기면 몸통 가운데에 들어갈 오름패 구해서 넣어줌 
                        {
                            int middle = (v1 + v2) / 2;
                            TileSO SO = GameManager.Instance.GetTile(t.TileType, middle);

                            newMelds.Add(new TileMeld(t1, t2, SO, isNeed: true));

                            HandReading reading = new HandReading(newMelds, pair);
                            //if (reading.valid_keishiki) // 아무튼 역 없음 검사하고 넣어줌 
                            AppendReading(ref readings, reading);
                        }
                        else
                        {

                            // 오름패, t1, t2 넣어주는 코드 
                            if (!t1.IsTerminalTile()) // t1이 노두패가 아니라면 
                            {

                                TileSO SO = GameManager.Instance.GetTile(t.TileType, v1 - 1);
                                newMelds.Add(new TileMeld(t1, t2, SO, isNeed: true));

                                HandReading reading = new HandReading(newMelds, pair);
                                //if (reading.valid_keishiki)
                                AppendReading(ref readings, reading);
                            }


                            // t1 t2 오름패 로 넣는다는 뜻 
                            if (!t2.IsTerminalTile()) // t2가 노두패가 아니라면 
                            {
                                newMelds.Clear();
                                foreach (var item in melds)
                                {
                                    newMelds.Add(item);
                                }

                                TileSO SO = GameManager.Instance.GetTile(t.TileType, v2 + 1);
                                newMelds.Add(new TileMeld(t1, t2, SO, isNeed: true));

                                HandReading reading = new HandReading(newMelds, pair);
                                //if (reading.valid_keishiki)
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

}