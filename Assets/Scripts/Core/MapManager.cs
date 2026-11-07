using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private TileType[,] _tileMap;
    private UnitBase[,] _unitMap;

    public UnitBase[,] UnitMap => _unitMap;

    private void OnEnable() => BindSpawnEvents();
    private void OnDisable() => UnbindSpawnEvents();

    private void SetMap()
    {
        _tileMap = _spawnManager.TileMap;
        _unitMap = _spawnManager.UnitMap;
    }

    private void BindSpawnEvents()
    {
        _spawnManager.OnStageLoaded += SetMap;
    }

    private void UnbindSpawnEvents()
    {
        _spawnManager.OnStageLoaded -= SetMap;
    }
}
