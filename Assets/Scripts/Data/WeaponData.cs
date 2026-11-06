using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private int _attack;
    [SerializeField] private int _energyCost;
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private WeaponRangeShape _weaponRangeShape;
    [SerializeField] private int _minRange;
    [SerializeField] private int _maxRange;

    public int Id => _id;
    public string Name => _name;
    public int Attack => _attack;
    public int EnergyCost => _energyCost;
    public WeaponType WeaponType => _weaponType;
    public int MinRange => _minRange;
    public int MaxRange => _maxRange;
    public WeaponRangeShape WeaponRangeShape => _weaponRangeShape;

    public void SetData(int id, string name, int attack, int energyCost, WeaponType weaponType, int minRange, int maxRange, WeaponRangeShape weaponRangeShape)
    {
        _id = id;
        _name = name;
        _attack = attack;
        _energyCost = energyCost;
        _weaponType = weaponType;
        _minRange = minRange;
        _maxRange = maxRange;
        _weaponRangeShape = weaponRangeShape;
    }
}
