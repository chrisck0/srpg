using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackCommandUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentEnergyText;
    [SerializeField] private WeaponCommandContainerUI _weaponContainerPrefab;
    [SerializeField] private Transform _verticalLayout;
    private ObjectPool<WeaponCommandContainerUI> _containerObjectPool;
    private List<WeaponCommandContainerUI> _activatedContainers = new();

    public Action<WeaponData> OnWeaponSelected;

    private void Awake() => CreatePool();
    private void OnDisable() => ReturnWeaponContainers();

    private void CreatePool()
    {
        List<WeaponCommandContainerUI> weaponContainers = new();
        weaponContainers.Add(_weaponContainerPrefab);
        _containerObjectPool = new ObjectPool<WeaponCommandContainerUI>(weaponContainers, _verticalLayout, false);
    }

    public void SelectWeapon(WeaponData weaponData)
    {
        OnWeaponSelected?.Invoke(weaponData);
        ReturnWeaponContainers();
    }

    public void InitWeaponInfo(UnitBase selectedUnit)
    {
        _activatedContainers.Clear();
        int currentEnergy = selectedUnit.UnitStat.Energy;
        _currentEnergyText.text = $"EN {currentEnergy} / {selectedUnit.UnitStat.UnitData.Energy}";

        foreach (WeaponData weaponData in selectedUnit.Weapon.WeaponDataList)
        {
            Debug.Log(_containerObjectPool);
            Debug.Log(_weaponContainerPrefab);
            WeaponCommandContainerUI weaponContainer = _containerObjectPool.Take(_weaponContainerPrefab);
            weaponContainer.SetData(weaponData, this, currentEnergy);
            _activatedContainers.Add(weaponContainer);
        }
    }

    private void ReturnWeaponContainers()
    {
        foreach (WeaponCommandContainerUI weaponContainer in _activatedContainers)
        {
            _containerObjectPool.Return(weaponContainer);
        }

        _activatedContainers.Clear();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
