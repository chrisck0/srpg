using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Data/TileData")]
public class TileData : ScriptableObject
{
    [SerializeField] private TileType _tileType;
    [SerializeField] private string _name;
    [SerializeField] private int _cost;
    [SerializeField] private string _description;

    public TileType TileType => _tileType;
    public string Name => _name;
    public int Cost => _cost;
    public string Description => _description;

    public void SetData(TileType tileType, string name, int cost, string description)
    {
        _tileType = tileType;
        _name = name;
        _cost = cost;
        _description = description;
    }
}
