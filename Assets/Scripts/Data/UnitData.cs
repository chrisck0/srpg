using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitData", menuName = "Data/UnitData")]
public class UnitData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private int _health;
    [SerializeField] private int _energy;
    [SerializeField] private int _move;
    [SerializeField] private int _accuracy;
    [SerializeField] private int _avoid;
    [SerializeField] private List<int> _weaponIdList;
    [SerializeField] private UnitType _unitType;

    public int Id => _id;
    public string Name => _name;
    public int Health => _health;
    public int Energy => _energy;
    public int Move => _move;
    public int Accuracy => _accuracy;
    public int Avoid => _avoid;
    public List<int> WeaponIdList => _weaponIdList;
    public UnitType UnitType => _unitType;

    public void SetData(int id, string name, int health, int energy, int move, int accuracy, int avoid, List<int> weaponIdList, UnitType unitType)
    {
        _id = id;
        _name = name;
        _health = health;
        _energy = energy;
        _move = move;
        _accuracy = accuracy;
        _avoid = avoid;
        _weaponIdList = weaponIdList;
        _unitType = unitType;
    }
}
