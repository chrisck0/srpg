using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    [SerializeField] private List<UnitData> _unitDataList;
    [SerializeField] private List<WeaponData> _weaponDataList;
    [SerializeField] private List<StageData> _stageDataList;

    private Dictionary<int, UnitData> _unitDataDict;
    private Dictionary<int, WeaponData> _weaponDataDict;
    private Dictionary<int, StageData> _stageDataDict;

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

        foreach (StageData stageData in _stageDataList)
        {
            _stageDataDict[stageData.StageId] = stageData;
        }
    }
}
