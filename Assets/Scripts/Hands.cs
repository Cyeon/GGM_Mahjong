using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hands : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> _handTileObjects = new List<GameObject>();
    private List<Tile> _handTiles = new List<Tile>();

    [SerializeField]
    private GameObject _getTile = null;

    private void GameStart()
    {
        for (int i = 0; i < _handTileObjects.Count; i++)
        {
            _handTiles.Add(GameManager.Instance.PickUp());
        }

        TileSort();

        for (int i = 0; i < _handTileObjects.Count; i++)
        {
            _handTileObjects[i].transform.GetChild(1).GetComponent<Image>().sprite = _handTiles[i].TileSO.TileSprite;
        }
    }

    private void TileSort()
    {
        List<Tile> list = _handTiles;
        int gap = list.Count / 2;
        while (gap > 0)
        {
            for (int i = gap; i < list.Count; i++)
            {
                Tile temp = list[i];
                int j = i;
                while (j >= gap && ShouldSwap(list[j - gap], temp))
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

    private bool ShouldSwap(Tile a, Tile b)
    {
        if (a != null && b != null)
        {
            if (a.TileSO.TileType == b.TileSO.TileType)
            {
                return a.TileSO.TileNumber < b.TileSO.TileNumber;
            }
            else if (a.TileSO.TileType < b.TileSO.TileType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
}
