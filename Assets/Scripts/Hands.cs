using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Hands : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> _handTileObjects = new List<GameObject>();
    private List<TileSO> _handTiles = new List<TileSO>();

    [SerializeField]
    private GameObject _getTile = null;

    [SerializeField]
    public Button restartBtn;
    public Button restartBtn2;

    public UnityEvent<List<TileSO>> uEvent = null;
    public GameObject tenpai;
    private void Start()
    {
        GameStart();
        restartBtn.onClick.AddListener(Restart);
        restartBtn2.onClick.AddListener(Restart);
    }

    private void Restart()
    {
        GameManager.Instance.TileInit();
        tenpai.SetActive(false);
        GameStart();
    }

    private void GameStart()
    {
        _handTiles.Clear();
        for (int i = 0; i < _handTileObjects.Count; i++)
        {
            _handTiles.Add(GameManager.Instance.PickUp());
        }

        TileSort();
    }

    public void TileSort()
    {
        TileSort1();
        TileSort2();

        for (int i = 0; i < _handTileObjects.Count; i++)
        {
            _handTileObjects[i].GetComponent<Tile>().TileSO = _handTiles[i];
            _handTileObjects[i].transform.Find("Image").GetComponent<Image>().sprite = _handTiles[i].TileSprite;
        }
    }

    private void TileSort1()
    {
        List<TileSO> list = _handTiles;
        int gap = list.Count / 2;
        while (gap > 0)
        {
            for (int i = gap; i < list.Count; i++)
            {
                TileSO temp = list[i];
                int j = i;
                while (j >= gap && ShouldSwap1(list[j - gap], temp))
                {
                    list[j] = list[j - gap];
                    j -= gap;
                }
                list[j] = temp;
            }
            gap /= 2;
        }

        _handTiles = list;
    }

    private bool ShouldSwap1(TileSO a, TileSO b)
    {
        if (a != null && b != null)
        {
            return a.TileType > b.TileType;
        }

        return false;
    }

    private void TileSort2()
    {
        List<TileSO> list = _handTiles;
        int gap = list.Count / 2;
        while (gap > 0)
        {
            for (int i = gap; i < list.Count; i++)
            {
                TileSO temp = list[i];
                int j = i;
                while (j >= gap && ShouldSwap2(list[j - gap], temp))
                {
                    list[j] = list[j - gap];
                    j -= gap;
                }
                list[j] = temp;
            }
            gap /= 2;
        }

        _handTiles = list;
    }

    private bool ShouldSwap2(TileSO a, TileSO b)
    {
        if (a != null && b != null)
        {
            if (a.TileType == b.TileType)
            {
                return a.TileNumber > b.TileNumber;
            }
        }

        return false;
    }

    public void RemoveTile(TileSO tile)
    {
        _handTiles.Remove(tile);
        _handTiles.Add(GameManager.Instance.PickUp());
        uEvent?.Invoke(_handTiles);
        TileSort();
    }
}
