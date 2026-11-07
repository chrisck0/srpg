using UnityEngine;
using System.Collections.Generic;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private List<WeaponData> _weaponDataList = new();
    [SerializeField] private int _health;
    [SerializeField] private int _energy;

    public UnitData UnitData => _unitData;
    public List<WeaponData> WeaponDataList => _weaponDataList; 
    public int Health => _health;
    public int Energy => _energy;

    private void Awake() => InitStat();

    private void InitStat()
    {
        _health = _unitData.Health;
        _energy = _unitData.Energy;
    }

    public void AddWeapon(WeaponData weaponData)
    {
        _weaponDataList.Add(weaponData);
    }

    public void AddEnergy(int num)
    {
        _energy += num;

        _energy = Mathf.Clamp(_energy, 0, _unitData.Energy);
    }
}
