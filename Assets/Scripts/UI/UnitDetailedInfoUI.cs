using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitDetailedInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _unitNameText;
    [SerializeField] private TextMeshProUGUI _unitHealthText;
    [SerializeField] private TextMeshProUGUI _unitEnergyText;
    [SerializeField] private TextMeshProUGUI _unitMoveText;
    [SerializeField] private TextMeshProUGUI _unitAccuracyText;
    [SerializeField] private TextMeshProUGUI _unitAvoidText;
    [SerializeField] private WeaponInfoContainerUI _weaponContainerPrefab;
    [SerializeField] private Transform _verticalLayout;

    private ObjectPool<WeaponInfoContainerUI> _containerObjectPool;
    private List<WeaponInfoContainerUI> _activatedContainers = new();

    private void Awake() => CreatePool();
    private void OnDisable() => ReturnWeaponContainers();

    private void CreatePool()
    {
        List<WeaponInfoContainerUI> weaponContainers = new();
        weaponContainers.Add(_weaponContainerPrefab);
        _containerObjectPool = new ObjectPool<WeaponInfoContainerUI>(weaponContainers, _verticalLayout, false);
    }

    public void SetData(UnitBase selectedUnit)
    {
        _activatedContainers.Clear();

        SetUnitInfo(selectedUnit);

        foreach (WeaponData weaponData in selectedUnit.Weapon.WeaponDataList)
        {
            WeaponInfoContainerUI weaponContainer = _containerObjectPool.Take(_weaponContainerPrefab);
            weaponContainer.SetData(weaponData);
            _activatedContainers.Add(weaponContainer);
        }
    }

    private void SetUnitInfo(UnitBase selectedUnit)
    {
        _unitNameText.text = selectedUnit.UnitStat.UnitData.Name;
        _unitHealthText.text = selectedUnit.UnitStat.UnitData.Health.ToString();
        _unitEnergyText.text = selectedUnit.UnitStat.UnitData.Energy.ToString();
        _unitMoveText.text = selectedUnit.UnitStat.UnitData.Move.ToString();
        _unitAccuracyText.text = selectedUnit.UnitStat.UnitData.Accuracy.ToString();
        _unitAvoidText.text = selectedUnit.UnitStat.UnitData.Avoid.ToString();
    }

    private void ReturnWeaponContainers()
    {
        foreach (WeaponInfoContainerUI weaponContainer in _activatedContainers)
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
