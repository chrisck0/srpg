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
    private bool _canSelect;

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

    public void SetData(WeaponData weaponData, AttackCommandUI attackCommandUI, int currentEnergy)
    {
        _weaponData = weaponData;
        _nameText.text = weaponData.Name;
        _attackText.text = weaponData.Attack.ToString();

        if (currentEnergy < weaponData.EnergyCost)
        {
            _energyCostText.color = Color.red;
            _canSelect = false;
        }
        else
        {
            _energyCostText.color = Color.black;
            _canSelect = true;
        }
        _energyCostText.text = weaponData.EnergyCost.ToString();
        _attackCommandUI = attackCommandUI;
    }

    private void OnButtonClicked()
    {
        if (!_canSelect) return;
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
