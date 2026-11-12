using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[RequireComponent(typeof(UnitStat), typeof(Weapon))]
public abstract class UnitBase : MonoBehaviour, IPoolable, IDamageable
{
    [SerializeField] protected UnitStat _unitStat;
    [SerializeField] protected Weapon _weapon;
    [SerializeField] protected UnitAnimationController _animationController;
    [SerializeField] protected float _moveSpeed = 8f;
    protected int _health;
    protected int _energy;
    private bool _isMoving;
    private ObjectPool<UnitBase> _objectPool;

    public Action<int> OnAttackWeapon;
    public Action OnSelect;
    public Action<bool> OnMove;
    public Action OnVictory;
    public Action OnAvoid;
    public Action OnDamage;
    public event Action<UnitBase> OnDead;

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
        OnDamage?.Invoke();
    }

    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void UnSelect()
    {
        Debug.Log("Unselect");
    }

    public void ToggleMove()
    {
        _isMoving = !_isMoving;
        OnMove?.Invoke(_isMoving);
    }

    public void Avoid()
    {
        Debug.Log($"{_unitStat.UnitData.Name} avoided");
        OnAvoid?.Invoke();
    }

    public void Die()
    {
        OnDead?.Invoke(this);
        WaitAndReturnToPool().Forget();
    }

    private async UniTask WaitAndReturnToPool()
    {
        await UniTask.Delay(2000, DelayType.Realtime);
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
