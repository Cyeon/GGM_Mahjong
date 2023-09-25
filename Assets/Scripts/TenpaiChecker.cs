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
    /// ġ���� üũ
    /// </summary>
    /// <returns>���������� �ƴ���</returns>
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
    /// ���繫�� üũ
    /// </summary>
    /// <returns>���������� �ƴ���</returns>
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

        // ����� �����ϱ� ������ �� ��Ͽ� �����ؾ� �մϴ�
        List<TileSO> hand = remaining_tiles.OrderBy(x => x.TileType).ThenBy(x => x.TileNumber).ToList();

        // ��� = ù ��° �Ű������� �� ��° �Ű������� ���� ���� �������� ����
        // ���� ������ �¸��ε�, �ڵ� ����� 3���� �������� �������� 1�� �ƴϰų� �ڵ� ����� 13���� ũ�� 
        // Ȥ�� ������ �¸��� �ƴѵ�, �ڵ� ����� 3���� �������� �������� 2�� �ƴϰų� �ڵ� ����� 14���� ũ�� 
        if ((tenpai_only && (hand.Count % 3 != 1 || hand.Count > 13)) || // pons/kans/chi�� Ÿ�� ���� 3��ŭ �����ϹǷ� �׻� ��� 3 + 1 Ÿ���� ������ �ڵ忡 ��� �־�� �մϴ�(������ ��� + 2)
            (!tenpai_only && (hand.Count % 3 != 2 || hand.Count > 14)))
            return readings;
        else if (hand.Count == 1) // ���� �ϳ��� Ÿ���� ���Ҵٸ� then �츮�� �̱� Ÿ�� �� ��ٸ��� �ս��ϴ� 
        {
            TileSO t = hand[0];
            TilePair pair = new TilePair(t, t, true);

            HandReading reading = new HandReading(melds, pair);
            AppendReading(ref readings, reading);

            return readings;
        }  // ���� �츮�� ���� �ڵ�(�������ô���) �� ������ �ִٸ�, �츮�� ������ �� Ÿ���� ���� ���ƾ߸� �մϴ� 
        else if (hand.Count == 2) // If we have a winning hand, then our last two tiles must be the same
        {
            if (IsSameTile(hand[0], hand[1])) // ���� Ÿ���� ������ 
            {
                TilePair pair = new TilePair(hand[0], hand[1]); // �� ������ 

                HandReading reading = new HandReading(melds, pair);
                //if (reading.valid_keishiki) // ����������, 5��° Ÿ���� ��ٸ�����(Ÿ���� �� �����) üũ�ؿ� 
                AppendReading(ref readings, reading); // �׷��ٸ� �б� �߰� <??  
            }

            return readings;
        }

        // �ڵ� �������� ���ư� 
        for (int i = 0; i < hand.Count; i++)
        {
            if (i != 0 && IsSameTile(hand[i], hand[i - 1])) // i�� 0�� �ƴϰ� hand i �� ���� �Ŷ� ���� Ÿ�� Ÿ���̸� ��Ƽ��(��ü����?)
                continue;

            // �츮 Ÿ�Ϸ� ���ֵ��̸� ���� �� �ִ��� Ȯ���մϴ�
            TileSO tile = hand[i];

            List<TileSO> copy = new List<TileSO>(); // �츮�� ���� �� �ֵ��̸� ������ �츮 ���� ��� Ÿ���� �����ϴ� ���
            List<TileSO> meld = new List<TileSO>(); // �̰� �Ƹ� ��? �װǵ� �ٵ� �̹����� �츮�� ã�� �� ��Ƶδ� �����ε�   

            // �ڵ��� Ÿ���� �� ���� 
            foreach (TileSO t in hand)
            {
                if (meld.Count < 3 && IsSameTile(t, tile)) // ���� meld ����� ä������ ���� ���¿��� ���� for �� ���� Ÿ���̶� foreach Ÿ���̶� ������ 
                    meld.Add(t); // meld�� �ְ�
                else
                    copy.Add(t); // �ƴϸ� copy�� 
            }

            if (meld.Count == 3) // meld�� 3�̸� 
            {
                TileMeld m = new TileMeld(meld[0], meld[1], meld[2], true); // �װɷ� �ϳ� ����µ� �� ����¥�� �ϳ��� �����ִ� �ǵ�  

                List<TileMeld> new_melds = new List<TileMeld>();

                foreach (var item in melds)
                {
                    new_melds.Add(item);
                }

                //new_melds.Add_all(melds); // �� �̹� ���� �� �͵� �� �־��ְ� 
                new_melds.Add(m); // �̹��� ã�� ���� �־��ְ� 
                // �ٽ� ��ͷ� ȣ���ϴµ�
                AppendReadingList(ref readings, hand_reading_recursion(copy, new_melds, tenpai_only, early_return)); // ���� ī�Ƕ� ��� �ٽ� �־��� 

                if (early_return && readings.Count > 0) // �� �����̰� ������ ����� 0���� ũ�� ������ ����
                    return readings; // �ٵ� ����� ���� 
            }
            // ���� Ÿ���� ���а� Ÿ���� 6���� �۰ų� ������(�� 6������ üũ�ϳĸ� 6->7�̰� 7 8 9 �ϸ� üũ �Ǵϱ� )
            if (tile.TileType != TileType.Word && tile.TileNumber <= 7)
            {
                // Ÿ���� ���� ���� ���ڷ� ���� ���� �� �ִ��� Ȯ���մϴ�(�ߺ� �� ������ �����ϱ� ���� ���� ���� ���ڸ� ���)
                TileSO one_more = null;
                TileSO two_more = null;
                copy.Clear(); // ī�� Ŭ�����ؼ� ��Ȱ�� �� �� �ְ� 
                bool isFirst = true;
                foreach (TileSO t in hand) // ���� �����ϴ� �� ����� ���� �־��ְ� ������ copy���־��ִ� �ݺ��� 
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

                if (one_more != null && two_more != null) // �� �� �� ��������� 
                {
                    TileMeld m = new TileMeld(tile, one_more, two_more); // Ÿ��, ����� ������ �־��� 

                    List<TileMeld> new_melds = new List<TileMeld>(); // �� ��� ����Ʈ ����� 
                    foreach (var item in melds)
                    {
                        new_melds.Add(item);
                    }
                    //new_melds.Add_all(melds); // ���� ���� �ְ� 
                    new_melds.Add(m); // �̹��� ã�� ���� �־��ְ� 

                    // �߰� ���� ȣ���� 
                    // �ٵ� �ű⿡ �� �Լ��� ��ü������ �ٽ� ȣ���ؿ� (����)
                    AppendReadingList(ref readings, hand_reading_recursion(copy, new_melds, tenpai_only, early_return));

                    if (early_return && readings.Count > 0) // �� �����̸鼭 ���� ����� 0���� ũ�� 
                        return readings; // ������ ��ȯ�Ѥ� ��
                }
            }

            // ���� �Ӹ� ã�´� ���ε� 
            // ������ �ɼ��� ���� ã�� ������ ������ ��ٸ��� �ִ��� Ȯ���ϴ� ���Դϴ�(�̰��� �տ� Ÿ���� 4�� ���� �ְ� ������ �¸��� ��쿡�� �����մϴ�)
            // Last option is to find a pair, and see if we are waiting for our last combination (this can only be if we have 4 tiles left in the hand, and are tenpai only)
            if (hand.Count == 4)
            {
                int s = hand.Count;
                TileSO t = null;
                TileSO n1 = null, n2 = null;
                if (IsSameTile(hand[(i + 1) % s], hand[(i + 2) % s])) // �ƹ�ư 1��Ÿ�� �ϰ� 2�� Ÿ�� Ÿ�� ������ 
                {
                    n1 = hand[(i + 1) % s];
                    n2 = hand[(i + 2) % s];
                    t = hand[(i + 3) % s];
                }
                else if (IsSameTile(hand[(i + 1) % s], hand[(i + 3) % s])) // 1 3 ������ 
                {
                    n1 = hand[(i + 1) % s];
                    t = hand[(i + 2) % s];
                    n2 = hand[(i + 3) % s];
                }
                else if (IsSameTile(hand[(i + 2) % s], hand[(i + 3) % s])) // 2 3 ������ 
                {
                    t = hand[(i + 1) % s];
                    n1 = hand[(i + 2) % s];
                    n2 = hand[(i + 3) % s];
                }  // ���� Ÿ���� Ÿ�ϵ��� n1 n2 �� �־��ְ� ���� �� t�� �ִ´� 
                // ��״� Ÿ���� �츮�� �ٸ��� ���� Ÿ�� T1 S1���� �ϳ����̴ϱ� 
                // ��ġ�ϴ� �Ӹ� Ÿ���� ã���ִ� �Ŷ�� ���� �� 
                // n1 n2�� �Ӹ� 1 �Ӹ� 2 �ΰ� 

                if (t != null) // �Ӹ��� ã������ t�� null�� �ƴϸ� 
                {
                    if (IsSameTile(t, tile)) // We have two remaining pairs // �츰 ������ �� ���� ���� �и� 
                    { // �״ϱ� Ÿ���̶� t�� ���Ƽ� �Ӹ� �� ���̸� 
                        TilePair pair = new TilePair(tile, t); // Ÿ���̶� t ���� ����� 
                        List<TileMeld> new_melds = new List<TileMeld>();
                        //new_melds.Add_all(melds);
                        foreach (var item in melds)
                        {
                            new_melds.Add(item);
                        }
                        new_melds.Add(new TileMeld(n1, n2, n1, isNeed: true)); // �� ��忡 �Ӹ� 1 + ���� �� �ϳ� ���� �� �������ΰ� �־ �ϰ� 
                        HandReading reading = new HandReading(new_melds, pair); // �Ӹ�1�� ���� ���, �Ӹ�2�� �Ӹ��� ��츦 �ڵ� �������� �ְ� 
                                                                                //if (reading.valid_keishiki) // ������ �˻縦 ��Ų �� 
                        AppendReading(ref readings, reading); // �߰� ������ �־������  �ƹ�ư �׷��� �̰� ����� ����Ʈ�� �̹� �����Ѱ� �մ� �� �ƴϸ� �־��ִ� �װǵ� 

                        pair = new TilePair(n1, n2); // ���� �� �ٸ� �ɷ� �����
                        new_melds.Clear();
                        foreach (var item in melds)
                        {
                            new_melds.Add(item);
                        }
                        //new_melds.Add_all(melds);
                        new_melds.Add(new TileMeld(tile, t, tile, isNeed: true)); // �ƹ�ư �Ӹ� 2�� ���� ���� �Ȱ��� ���ְ� 
                        reading = new HandReading(new_melds, pair); //  �ƹ�ư �־��ְ�
                                                                    //if (reading.valid_keishiki)
                        AppendReading(ref readings, reading); // �Ȱ��� �߰��˻�  

                        return readings; // �� �㿡 ����� ��ȯ 
                    } // �츮�� ���� 3������ ��ٸ��� �մ� 
                    else if (tile.IsNeighbour(t) || tile.IsSecondNeighbour(t)) // We have a pair and are waiting on the final triplet
                    { // ���� t�� Ÿ���� �ٷ� �� Ÿ���̰ų� �� ĭ ������ Ÿ���̸� 
                        TilePair pair = new TilePair(n1, n2); // n12�� �Ӹ� ��� ����� 

                        int v1 = (int)t.TileNumber;
                        int v2 = (int)tile.TileNumber;

                        TileSO t1, t2; // ���ڰ� �� ū Ÿ���� 2�� �ǵ��� ���� ���ִ� ������ 
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

                        v1 = (int)t1.TileNumber; // �׸��� Ÿ�� Ÿ���� �ٽ� �־��ְ� 
                        v2 = (int)t2.TileNumber;

                        List<TileMeld> new_melds = new List<TileMeld>(); // �� ��� �����
                        //new_melds.Add_all(melds); // ���� ������� ��� �� �־��� 
                        foreach (var item in melds)
                        {
                            new_melds.Add(item);
                        }

                        if (tile.IsSecondNeighbour(t)) // ���� ��¯���� 
                        {
                            int middle = (v1 + v2) / 2; // Need a new tile in the middle // �̵� Ÿ���� �����ش� 
                            TileSO SO = new TileSO();
                            SO.SetData(t.TileType, middle);
                            new_melds.Add(new TileMeld(t1, t2, SO, isNeed: true)); // �׸��� �� ��忡 �ְ�
                            // �� ���� �˾Ҵµ� Ÿ�� ID�� -1�̸� ������ ����ε� 
                            HandReading reading = new HandReading(new_melds, pair);
                            //if (reading.valid_keishiki) // �ƹ�ư �� ���� �˻��ϰ� �־��� 
                            AppendReading(ref readings, reading);
                        }
                        else
                        {
                            if (!t1.IsTerminalTile()) // t1�� ����а� �ƴ϶�� 
                            { // ���� ������ t1 t2�� �ְ�

                                TileSO SO = new TileSO();
                                SO.SetData(t.TileType, v1 - 1);
                                new_melds.Add(new TileMeld(t1, t2, SO, isNeed: true));

                                HandReading reading = new HandReading(new_melds, pair);
                                //if (reading.valid_keishiki)
                                AppendReading(ref readings, reading);
                            }

                            if (!t2.IsTerminalTile()) // t2�� ����а� �ƴ϶�� 
                            { // t1 t2 ������ �� �ִ´ٴ� �� 
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

                        return readings; // �׷��� �ϼ��� �и� ���� 
                    }
                }
            }
        }

        return readings;
    }


}