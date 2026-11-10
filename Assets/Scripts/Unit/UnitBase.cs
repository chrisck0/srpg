using UnityEngine;

[RequireComponent(typeof(UnitStat), typeof(Weapon))]
public abstract class UnitBase : MonoBehaviour, IPoolable, IDamageable
{
    private ObjectPool<UnitBase> _objectPool;
    protected UnitStat _unitStat;
    protected Weapon _weapon;
    protected int _health;
    protected int _energy;

    public UnitStat UnitStat => _unitStat;
    public Weapon Weapon => _weapon;
    public int Health => _health;
    public int Energy => _energy;
    public abstract UnitType UnitType { get; }


    protected void Awake() => CacheComponents();

    protected void CacheComponents()
    {
        _unitStat = GetComponent<UnitStat>();
        _weapon = GetComponent<Weapon>();
    }

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
        Debug.Log($"{_unitStat.UnitData.Name} Died");
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
