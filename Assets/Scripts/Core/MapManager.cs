using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    private UnitMovement _unitMovement;
    private TileBase[,] _tileMap;
    private UnitBase[,] _unitMap;

    public UnitMovement UnitMovement => _unitMovement;
    public TileBase[,] TileMap => _tileMap;
    public UnitBase[,] UnitMap => _unitMap;

    private void OnEnable() => BindSpawnEvents();
    private void OnDisable() => UnbindSpawnEvents();

    private void SetMap()
    {
        _tileMap = _spawnManager.TileMap;
        _unitMap = _spawnManager.UnitMap;

        _unitMovement = new(_tileMap, _unitMap);
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
