using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private List<UnitBase> _unitPrefabs;
    [SerializeField] private List<TileBase> _tilePrefabs;

    private ObjectPool<UnitBase> _unitObjectPool;
    private ObjectPool<TileBase> _tileObjectPool;
    private Dictionary<UnitType, UnitBase> _unitPrefabDict = new();
    private Dictionary<TileType, TileBase> _tilePrefabDict = new();
    private Dictionary<UnitType, List<UnitBase>> _spawnedUnitDict = new();

    public Dictionary<UnitType, List<UnitBase>> SpawnedUnitDict => _spawnedUnitDict;
    public event Action OnStageLoaded;

    private void Awake()
    {
        CreatePools();
        CreatePrefabDict();
    }

    private void Start() => SpawnStage(1); // 스테이지 1
    private void CreatePools()
    {
        _unitObjectPool = new ObjectPool<UnitBase>(_unitPrefabs, new GameObject("UnitObjectPool").transform);
        _tileObjectPool = new ObjectPool<TileBase>(_tilePrefabs, new GameObject("TileObjectPool").transform);
    }
    
    private void CreatePrefabDict()
    {
        foreach (UnitBase unitPrefab in _unitPrefabs)
        {
            _unitPrefabDict[unitPrefab.UnitType] = unitPrefab;
        }

        foreach (TileBase tilePrefab in _tilePrefabs)
        {
            _tilePrefabDict[tilePrefab.TileType] = tilePrefab;
        }
    }

    private void SpawnStage(int stageId)
    {
        if (!_dataManager.StageDataDict.TryGetValue(stageId, out StageData stageData)) return;

        SpawnMap(stageData.Map);
        SpawnUnits(stageData.UnitPositionList);
        OnStageLoaded?.Invoke();
    }

    private void SpawnMap(TileType[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++) // y좌표
        {
            for (int j = 0; j < map.GetLength(1); j++) // x좌표
            {
                if (!_tilePrefabDict.TryGetValue(map[j, i], out TileBase tilePrefab)) continue;

                TileBase tile = _tileObjectPool.Take(tilePrefab);
                Vector2Int coord = new Vector2Int(j, i);

                tile.transform.position = coord.CoordToWorld(-1f);
            }
        }
    }

    private void SpawnUnits(List<UnitPosition> unitPositionList)
    {
        Dictionary<int, UnitData> unitDataDict = _dataManager.UnitDataDict;

        foreach (UnitPosition unitPosition in unitPositionList)
        {
            if (!unitDataDict.TryGetValue(unitPosition.Id, out UnitData unitData)) continue;

            if (!_unitPrefabDict.TryGetValue(unitData.UnitType, out UnitBase unitPrefab)) continue;

            UnitBase unit = _unitObjectPool.Take(unitPrefab);
            Vector2Int coord = unitPosition.Position;
            unit.transform.position = coord.CoordToWorld();
            unit.UnitStat.InitStat(unitData);

            Dictionary<int, WeaponData> weaponDataDict = _dataManager.WeaponDataDict;

            foreach (int weaponId in unitData.WeaponIdList)
            {
                if (weaponDataDict.TryGetValue(weaponId, out WeaponData weaponData))
                {
                    unit.UnitStat.AddWeapon(weaponData);
                }
            }

            AddSpawnedUnit(unitData.UnitType, unit);
        }
    }

    private void AddSpawnedUnit(UnitType unitType, UnitBase unit)
    {
        if (_spawnedUnitDict.ContainsKey(unitType))
        {
            _spawnedUnitDict[unitType].Add(unit);
        }
        else
        {
            List<UnitBase> list = new List<UnitBase>();
            _spawnedUnitDict[unitType] = list;
            _spawnedUnitDict[unitType].Add(unit);
        }
    }
}
