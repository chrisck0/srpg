using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private List<WeaponData> _weaponDataList = new();
    private UnitBase _owner;

    public List<WeaponData> WeaponDataList => _weaponDataList;

    private void Awake() => CacheComponents();

    public void AddWeapon(WeaponData weaponData)
    {
        _weaponDataList.Add(weaponData);
    }

    public void Attack(WeaponData weaponData, UnitBase target)
    {
        Debug.Log($"{_owner.UnitStat.UnitData.Name} attacked {target.UnitStat.UnitData.Name} DMG {weaponData.Attack}");
        target.TakeDamage(weaponData.Attack);
    }

    public bool CanGetReturnAttack(int distance)
    {
        WeaponData weapon = WeaponDataList[0];
        if (weapon.MinRange <= distance
            && weapon.MaxRange >= distance)
        {
            return true;
        }

        return false;
    }

    private void CacheComponents()
    {
        _owner = GetComponent<UnitBase>();
    }
}
