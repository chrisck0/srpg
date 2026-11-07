using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommandUI : MonoBehaviour
{
    [SerializeField] private WeaponContainerUI _weaponContainerPrefab;
    [SerializeField] private Transform _verticalLayout;
    private ObjectPool<WeaponContainerUI> _weaponContainerObjectPool;
    private List<WeaponContainerUI> _activatedWeaponContainers = new();

    public Action<WeaponData> OnWeaponSelected;

    private void Awake() => CreatePool();
    private void OnDisable() => ReturnWeaponContainers();

    private void CreatePool()
    {
        List<WeaponContainerUI> weaponContainers = new();
        weaponContainers.Add(_weaponContainerPrefab);
        _weaponContainerObjectPool = new ObjectPool<WeaponContainerUI>(weaponContainers, _verticalLayout, false);
    }

    public void SelectWeapon(WeaponData weaponData)
    {
        OnWeaponSelected?.Invoke(weaponData);
        ReturnWeaponContainers();
    }

    public void InitWeaponInfo(List<WeaponData> weaponDataList)
    {
        _activatedWeaponContainers.Clear();

        foreach (WeaponData weaponData in weaponDataList)
        {
            WeaponContainerUI weaponContainer = _weaponContainerObjectPool.Take(_weaponContainerPrefab);
            weaponContainer.SetData(weaponData, this);
            _activatedWeaponContainers.Add(weaponContainer);
        }
    }

    private void ReturnWeaponContainers()
    {
        foreach (WeaponContainerUI weaponContainer in _activatedWeaponContainers)
        {
            _weaponContainerObjectPool.Return(weaponContainer);
        }

        _activatedWeaponContainers.Clear();
    }
}
