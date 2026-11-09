using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackCommandUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentEnergyText;
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

    public void InitWeaponInfo(UnitBase selectedUnit, List<WeaponData> weaponDataList)
    {
        _activatedWeaponContainers.Clear();
        int currentEnergy = selectedUnit.UnitStat.Energy;
        _currentEnergyText.text = $"EN {currentEnergy} / {selectedUnit.UnitStat.UnitData.Energy}";

        foreach (WeaponData weaponData in weaponDataList)
        {
            WeaponContainerUI weaponContainer = _weaponContainerObjectPool.Take(_weaponContainerPrefab);
            weaponContainer.SetData(weaponData, this, currentEnergy);
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

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
