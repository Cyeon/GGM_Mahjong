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
        List<HandReading> tileList = hand_reading_recursion(list, new List<TileMeld>(), true, false);
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

    private static bool IsSameTile(TileSO tileOne, TileSO tileTwo)
    {
        if (tileOne.TileType == tileTwo.TileType && tileOne.TileNumber == tileTwo.TileNumber)
            return true;
        return false;
    }

    private static List<HandReading> hand_reading_recursion(List<TileSO> remaining_tiles, List<TileMeld> melds, bool tenpai_only, bool early_return)
    {
        List<HandReading> readings = new List<HandReading>();

        // 목록을 정렬하기 때문에 새 목록에 복사해야 합니다
        List<TileSO> hand = remaining_tiles.OrderBy(x => x.TileType).ThenBy(x => x.TileNumber).ToList();

        // 모드 = 첫 번째 매개변수를 두 번째 매개변수로 나눈 후의 나머지를 리턴
        // 만약 텐파이 온리인데, 핸드 사이즈가 3으로 나눴을때 나머지가 1이 아니거나 핸드 사이즈가 13보다 크면 
        // 혹은 텐파이 온리가 아닌데, 핸드 사이즈가 3으로 나눴을때 나머지가 2가 아니거나 핸드 사이즈가 14보다 크면 
        if ((tenpai_only && (hand.Count % 3 != 1 || hand.Count > 13)) || // pons/kans/chi는 타일 수를 3만큼 제거하므로 항상 모드 3 + 1 타일을 텐파이 핸드에 들고 있어야 합니다(승자일 경우 + 2)
            (!tenpai_only && (hand.Count % 3 != 2 || hand.Count > 14)))
            return readings;
        else if (hand.Count == 1) // 만약 하나의 타일이 남았다면 then 우리는 싱글 타일 페어를 기다리고 잇습니다 
        {
            TileSO t = hand[0];
            TilePair pair = new TilePair(t, t, true);

            HandReading reading = new HandReading(melds, pair);
            AppendReading(ref readings, reading);

            return readings;
        }  // 만약 우리가 위닝 핸드(가뭔데씹덕아) 를 가지고 있다면, 우리의 마지막 두 타일은 서로 같아야만 합니다 
        else if (hand.Count == 2) // If we have a winning hand, then our last two tiles must be the same
        {
            if (IsSameTile(hand[0], hand[1])) // 만약 타입이 같으면 
            {
                TilePair pair = new TilePair(hand[0], hand[1]); // 페어를 만들어요 

                HandReading reading = new HandReading(melds, pair);
                //if (reading.valid_keishiki) // 역없음인지, 5번째 타일을 기다리는지(타일을 다 썼는지) 체크해요 
                AppendReading(ref readings, reading); // 그렇다면 읽기 추가 <??  
            }

            return readings;
        }

        // 핸드 사이즈대로 돌아감 
        for (int i = 0; i < hand.Count; i++)
        {
            if (i != 0 && IsSameTile(hand[i], hand[i - 1])) // i가 0이 아니고 hand i 랑 이전 거랑 같은 타일 타입이면 컨티뉴(대체왜지?)
                continue;

            // 우리 타일로 세쌍둥이를 만들 수 있는지 확인합니다
            TileSO tile = hand[i];

            List<TileSO> copy = new List<TileSO>(); // 우리가 만들 세 쌍둥이를 제외한 우리 손의 모든 타일을 포함하는 목록
            List<TileSO> meld = new List<TileSO>(); // 이게 아마 운? 그건듯 근데 이번에는 우리가 찾는 걸 담아두는 역할인듯   

            // 핸드의 타일을 다 돈다 
            foreach (TileSO t in hand)
            {
                if (meld.Count < 3 && IsSameTile(t, tile)) // 만약 meld 사이즈가 채워지지 않은 상태에서 지금 for 문 도는 타일이랑 foreach 타일이랑 같으면 
                    meld.Add(t); // meld에 넣고
                else
                    copy.Add(t); // 아니면 copy에 
            }

            if (meld.Count == 3) // meld가 3이면 
            {
                TileMeld m = new TileMeld(meld[0], meld[1], meld[2], true); // 그걸로 하나 만드는데 걍 세개짜리 하나로 묶어주는 건듯  

                List<TileMeld> new_melds = new List<TileMeld>();

                foreach (var item in melds)
                {
                    new_melds.Add(item);
                }

                //new_melds.Add_all(melds); // 아 이미 오픈 된 것들 다 넣어주고 
                new_melds.Add(m); // 이번에 찾은 퐁도 넣어주고 
                // 다시 재귀로 호출하는데
                AppendReadingList(ref readings, hand_reading_recursion(copy, new_melds, tenpai_only, early_return)); // 남은 카피랑 멜드 다시 넣어줌 

                if (early_return && readings.Count > 0) // 얼리 리턴이고 리딩즈 사이즈가 0보다 크면 리딩즈 리턴
                    return readings; // 근데 리딩즈가 뭐지 
            }
            // 만약 타일이 수패고 타일이 6보다 작거나 같으면(왜 6까지만 체크하냐면 6->7이고 7 8 9 하면 체크 되니까 )
            if (tile.TileType != TileType.Word && tile.TileNumber <= 7)
            {
                // 타일이 가장 낮은 숫자로 행을 만들 수 있는지 확인합니다(중복 행 순열을 생략하기 위해 가장 낮은 숫자만 사용)
                TileSO one_more = null;
                TileSO two_more = null;
                copy.Clear(); // 카피 클리어해서 재활용 할 수 있게 
                bool isFirst = true;
                foreach (TileSO t in hand) // 대충 연속하는 거 원모어 투모어에 넣어주고 나머지 copy에넣어주는 반복문 
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
                    TileMeld m = new TileMeld(tile, one_more, two_more); // 타일, 원모어 투모어로 넣어줌 

                    List<TileMeld> new_melds = new List<TileMeld>(); // 새 멜드 리스트 만들고 
                    foreach (var item in melds)
                    {
                        new_melds.Add(item);
                    }
                    //new_melds.Add_all(melds); // 기존 멜드들 넣고 
                    new_melds.Add(m); // 이번에 찾은 연속 넣어주고 

                    // 추가 리딩 호출함 
                    // 근데 거기에 이 함수를 자체적으로 다시 호출해요 (왜지)
                    AppendReadingList(ref readings, hand_reading_recursion(copy, new_melds, tenpai_only, early_return));

                    if (early_return && readings.Count > 0) // 얼리 리턴이면서 리딩 사이즈가 0보다 크면 
                        return readings; // 리딩을 반환한ㄷ ㅏ
                }
            }

            // 대충 머리 찾는단 뜻인듯 
            // 마지막 옵션은 쌍을 찾고 마지막 조합을 기다리고 있는지 확인하는 것입니다(이것은 손에 타일이 4개 남아 있고 텐파이 온리인 경우에만 가능합니다)
            // Last option is to find a pair, and see if we are waiting for our last combination (this can only be if we have 4 tiles left in the hand, and are tenpai only)
            if (hand.Count == 4)
            {
                int s = hand.Count;
                TileSO t = null;
                TileSO n1 = null, n2 = null;
                if (IsSameTile(hand[(i + 1) % s], hand[(i + 2) % s])) // 아무튼 1번타일 하고 2번 타일 타입 같으면 
                {
                    n1 = hand[(i + 1) % s];
                    n2 = hand[(i + 2) % s];
                    t = hand[(i + 3) % s];
                }
                else if (IsSameTile(hand[(i + 1) % s], hand[(i + 3) % s])) // 1 3 같으면 
                {
                    n1 = hand[(i + 1) % s];
                    t = hand[(i + 2) % s];
                    n2 = hand[(i + 3) % s];
                }
                else if (IsSameTile(hand[(i + 2) % s], hand[(i + 3) % s])) // 2 3 같으면 
                {
                    t = hand[(i + 1) % s];
                    n1 = hand[(i + 2) % s];
                    n2 = hand[(i + 3) % s];
                }  // 같은 타입의 타일들을 n1 n2 에 넣어주고 남는 걸 t에 넣는다 
                // 얘네는 타입이 우리랑 다르게 각각 타일 T1 S1마다 하나씩이니까 
                // 일치하는 머리 타일을 찾아주는 거라고 보면 됨 
                // n1 n2는 머리 1 머리 2 인것 

                if (t != null) // 머리가 찾아져서 t가 null이 아니면 
                {
                    if (IsSameTile(t, tile)) // We have two remaining pairs // 우린 가졌다 두 개의 남은 패를 
                    { // 그니까 타일이랑 t도 같아서 머리 두 쌍이면 
                        TilePair pair = new TilePair(tile, t); // 타일이랑 t 페어로 만들고 
                        List<TileMeld> new_melds = new List<TileMeld>();
                        //new_melds.Add_all(melds);
                        foreach (var item in melds)
                        {
                            new_melds.Add(item);
                        }
                        new_melds.Add(new TileMeld(n1, n2, n1, isNeed: true)); // 뉴 멜드에 머리 1 + 없는 거 하나 같은 거 오름패인거 넣어서 하고 
                        HandReading reading = new HandReading(new_melds, pair); // 머리1이 퐁인 경우, 머리2가 머리인 경우를 핸드 리딩으로 넣고 
                                                                                //if (reading.valid_keishiki) // 역없음 검사를 시킨 뒤 
                        AppendReading(ref readings, reading); // 추가 리딩에 넣어버린다  아무튼 그래서 이게 리딩즈가 리스트고 이미 동일한게 잇는 게 아니면 넣어주는 그건듯 

                        pair = new TilePair(n1, n2); // 이제 페어를 다른 걸로 만들고
                        new_melds.Clear();
                        foreach (var item in melds)
                        {
                            new_melds.Add(item);
                        }
                        //new_melds.Add_all(melds);
                        new_melds.Add(new TileMeld(tile, t, tile, isNeed: true)); // 아무튼 머리 2가 퐁인 경우로 똑같이 해주고 
                        reading = new HandReading(new_melds, pair); //  아무튼 넣어주고
                                                                    //if (reading.valid_keishiki)
                        AppendReading(ref readings, reading); // 똑같이 추가검사  

                        return readings; // 그 담에 리딩즈를 반환 
                    } // 우리는 페어와 3연속을 기다리고 잇다 
                    else if (tile.IsNeighbour(t) || tile.IsSecondNeighbour(t)) // We have a pair and are waiting on the final triplet
                    { // 만약 t가 타일의 바로 옆 타일이거나 한 칸 떨어진 타일이면 
                        TilePair pair = new TilePair(n1, n2); // n12로 머리 페어 만들고 

                        int v1 = (int)t.TileNumber;
                        int v2 = (int)tile.TileNumber;

                        TileSO t1, t2; // 숫자가 더 큰 타일이 2가 되도록 대입 해주는 이프문 
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

                        v1 = (int)t1.TileNumber; // 그리고 타일 타입을 다시 넣어주고 
                        v2 = (int)t2.TileNumber;

                        List<TileMeld> new_melds = new List<TileMeld>(); // 뉴 멜드 만들고
                        //new_melds.Add_all(melds); // 기존 멜드즈의 모든 걸 넣어줌 
                        foreach (var item in melds)
                        {
                            new_melds.Add(item);
                        }

                        if (tile.IsSecondNeighbour(t)) // 만약 간짱대기면 
                        {
                            int middle = (v1 + v2) / 2; // Need a new tile in the middle // 미드 타일을 구해준다 
                            TileSO SO = new TileSO();
                            SO.SetData(t.TileType, middle);
                            new_melds.Add(new TileMeld(t1, t2, SO, isNeed: true)); // 그리고 뉴 멜드에 넣고
                            // 아 이제 알았는데 타일 ID가 -1이면 오름패 취급인듯 
                            HandReading reading = new HandReading(new_melds, pair);
                            //if (reading.valid_keishiki) // 아무튼 역 없음 검사하고 넣어줌 
                            AppendReading(ref readings, reading);
                        }
                        else
                        {
                            if (!t1.IsTerminalTile()) // t1이 노두패가 아니라면 
                            { // 대충 오름패 t1 t2로 넣고

                                TileSO SO = new TileSO();
                                SO.SetData(t.TileType, v1 - 1);
                                new_melds.Add(new TileMeld(t1, t2, SO, isNeed: true));

                                HandReading reading = new HandReading(new_melds, pair);
                                //if (reading.valid_keishiki)
                                AppendReading(ref readings, reading);
                            }

                            if (!t2.IsTerminalTile()) // t2가 노두패가 아니라면 
                            { // t1 t2 오름패 로 넣는다는 뜻 
                                new_melds.Clear();
                                foreach (var item in melds)
                                {
                                    new_melds.Add(item);
                                }
                                //new_melds.Add_all(melds);
                                TileSO SO = new TileSO();
                                SO.SetData(t.TileType, v2 + 1);
                                new_melds.Add(new TileMeld(t1, t2, SO, isNeed: true));

                                HandReading reading = new HandReading(new_melds, pair);
                                //if (reading.valid_keishiki)
                                AppendReading(ref readings, reading);
                            }
                        }

                        return readings; // 그래서 완성된 패를 리턴 
                    }
                }
            }
        }

        return readings;
    }


}