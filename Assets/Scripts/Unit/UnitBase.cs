using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected UnitStat _unitStat;
    protected int _health;
    protected int _energy;

    public UnitStat UnitStat => _unitStat;
    public int Health => _health;
    public int Energy => _energy;

    protected void Awake() => CacheComponents();

    protected void CacheComponents()
    {
        _unitStat = GetComponent<UnitStat>();
    }
}
