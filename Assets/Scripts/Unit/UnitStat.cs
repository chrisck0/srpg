using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

public class UnitStat : MonoBehaviour
{
    [SerializeField] private UnitBase _unit;
    [SerializeField] private UnitData _data;
    [SerializeField] private int _health;
    [SerializeField] private int _energy;

    public event Action<int> OnHealthChanged;

    public UnitData Data => _data;
    public int Health => _health;
    public int Energy => _energy;

    private void Awake() => CacheComponents();

    public void InitStat(UnitData data)
    {
        _data = data;
        _health = _data.Health;
        _energy = _data.Energy;
    }

    public void AddEnergy(int num)
    {
        _energy += num;

        _energy = Mathf.Clamp(_energy, 0, _data.Energy);
    }

    public void AddHealth(int num)
    {
        _health += num;
        _health = Mathf.Clamp(_health, 0, _data.Health);

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
