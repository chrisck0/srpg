using UnityEngine;

[RequireComponent(typeof(UnitStat))]
public abstract class UnitBase : MonoBehaviour, IPoolable
{
    private ObjectPool<UnitBase> _objectPool;
    protected UnitStat _unitStat;
    protected int _health;
    protected int _energy;

    public UnitStat UnitStat => _unitStat;
    public int Health => _health;
    public int Energy => _energy;
    public abstract UnitType UnitType { get; }


    protected void Awake() => CacheComponents();

    protected void CacheComponents()
    {
        _unitStat = GetComponent<UnitStat>();
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
