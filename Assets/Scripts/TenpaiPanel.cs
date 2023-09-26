using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenpaiPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject _tilePrefab = null;
    [SerializeField]
    private Transform _hand = null;


    private List<Tile> _tileList = new List<Tile>();
    private TenpaiChecker _checker = null;


    private void Awake()
    {
        _checker = FindObjectOfType<TenpaiChecker>();
    }

    private void OnEnable()
    {
        if (_checker.TenpaiNeed.Count > 0)
        {
            TileSet();
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < _tileList.Count; i++)
        {
            Destroy(_tileList[i].gameObject);
        }
        _tileList.Clear();
    }

    public void TileSet()
    {
        for (int i = 0; i < _checker.TenpaiNeed.Count; i++)
        {
            Tile tile = Instantiate(_tilePrefab, _hand).GetComponent<Tile>();
            tile.TileSO = _checker.TenpaiNeed[i];
            tile.SetUI();
            _tileList.Add(tile);
        }
    }
}