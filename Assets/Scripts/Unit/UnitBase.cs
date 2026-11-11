using System;
using UnityEngine;

[RequireComponent(typeof(UnitStat), typeof(Weapon))]
public abstract class UnitBase : MonoBehaviour, IPoolable, IDamageable
{
    [SerializeField] protected UnitStat _unitStat;
    [SerializeField] protected Weapon _weapon;
    [SerializeField] protected float _moveSpeed = 8f;
    protected int _health;
    protected int _energy;
    private ObjectPool<UnitBase> _objectPool;

    public event Action<UnitBase> OnUnitDead;
    public UnitStat UnitStat => _unitStat;
    public Weapon Weapon => _weapon;
    public float MoveSpeed => _moveSpeed;
    public int Health => _health;
    public int Energy => _energy;
    public abstract UnitType UnitType { get; }

    public void ConsumeEnergy(int num)
    {
        _unitStat.AddEnergy(-num);
    }

    public void TakeDamage(int num)
    {
        _unitStat.AddHealth(-num);
    }

    public void Die()
    {
        OnUnitDead?.Invoke(this);
        ReturnToPool();
    }

    public void SetObjectPool(ObjectPool<UnitBase> objectPool)
    {
        _objectPool = objectPool;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        _objectPool.Return(this);
    }
}
