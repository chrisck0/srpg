using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private UnitBase _unit;
    [SerializeField] private UnitData _unitData;
    [SerializeField] private int _health;
    [SerializeField] private int _energy;

    public event Action<int> OnHealthChanged;

    public UnitData UnitData => _unitData;
    public int Health => _health;
    public int Energy => _energy;

    private void Awake() => CacheComponents();

    public void InitStat(UnitData unitData)
    {
        _unitData = unitData;
        _health = _unitData.Health;
        _energy = _unitData.Energy;
    }

    public void AddEnergy(int num)
    {
        _energy += num;

        _energy = Mathf.Clamp(_energy, 0, _unitData.Energy);
    }

    public void AddHealth(int num)
    {
        _health += num;
        _health = Mathf.Clamp(_health, 0, _unitData.Health);

        if (_health == 0)
        {
            _unit.Die();
        }
        OnHealthChanged?.Invoke(_health);
    }

    private void CacheComponents()
    {
        _unit = GetComponent<UnitBase>();
    }
}
