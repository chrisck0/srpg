using UnityEngine;
using System.Collections.Generic;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private List<WeaponData> _weaponData;
    [SerializeField] private int _health;
    [SerializeField] private int _energy;

    public UnitData UnitData => _unitData;
    public int Health => _health;
    public int Energy => _energy;

    private void Awake() => InitStat();

    /// <summary>
    /// 유닛 스탯 초기화
    /// </summary>
    public void InitStat()
    {
        _health = _unitData.Health;
        _energy = _unitData.Energy;

        foreach (int weaponId in _unitData.WeaponIdList)
        {
            // _weaponData.Add()
        }
    }

    public void AddEnergy(int num)
    {
        _energy += num;

        _energy = Mathf.Clamp(_energy, 0, _unitData.Energy);
    }
}
