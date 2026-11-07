using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponContainerUI : MonoBehaviour, IPoolable
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _energyCostText;
    private AttackCommandUI _attackCommandUI;
    private WeaponData _weaponData;

    public void Activate()
    {
        gameObject.SetActive(true);
        BindButtonEvents();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        UnbindButtonEvents();
    }

    public void ReturnToPool()
    {
        
    }

    public void SetData(WeaponData weaponData, AttackCommandUI attackCommandUI)
    {
        _weaponData = weaponData;
        _nameText.text = weaponData.name;
        _attackText.text = weaponData.Attack.ToString();
        _energyCostText.text = weaponData.EnergyCost.ToString();
        _attackCommandUI = attackCommandUI;
    }

    private void OnButtonClicked()
    {
        _attackCommandUI.SelectWeapon(_weaponData);
    }

    private void BindButtonEvents()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void UnbindButtonEvents()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }
}
