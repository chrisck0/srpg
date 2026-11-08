using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    [SerializeField] private List<UnitData> _unitDataList;
    [SerializeField] private List<WeaponData> _weaponDataList;
    [SerializeField] private List<TileData> _tileDataList;
    [SerializeField] private List<StageData> _stageDataList;

    private Dictionary<int, UnitData> _unitDataDict = new();
    private Dictionary<int, WeaponData> _weaponDataDict = new();
    private Dictionary<TileType, TileData> _tileDataDict = new();
    private Dictionary<int, StageData> _stageDataDict = new();

    public Dictionary<int, UnitData> UnitDataDict => _unitDataDict;
    public Dictionary<int, WeaponData> WeaponDataDict => _weaponDataDict;
    public Dictionary<TileType, TileData> TileDataDict => _tileDataDict;
    public Dictionary<int, StageData> StageDataDict => _stageDataDict;

    private void Awake() => InitDictionary();

    private void InitDictionary()
    {
        foreach (UnitData unitData in _unitDataList)
        {
            _unitDataDict[unitData.Id] = unitData;
        }

        foreach (WeaponData weaponData in _weaponDataList)
        {
            _weaponDataDict[weaponData.Id] = weaponData;
        }

        foreach (TileData tileData in _tileDataList)
        {
            _tileDataDict[tileData.TileType] = tileData; 
        }

        foreach (StageData stageData in _stageDataList)
        {
            _stageDataDict[stageData.StageId] = stageData;
        }
    }
}
