using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Data/TileData")]
public class TileData : ScriptableObject
{
    [SerializeField] private TileType _tileType;
    [SerializeField] private string _name;
    [SerializeField] private int _cost;

    public TileType TileType => _tileType;
    public string Name => _name;
    public int Cost => _cost;

    public void SetData(TileType tileType, string name, int cost)
    {
        _tileType = tileType;
        _name = name;
        _cost = cost;
    }
}
