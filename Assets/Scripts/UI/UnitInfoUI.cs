using TMPro;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _unitNameText;
    [SerializeField] private TextMeshProUGUI _unitHealthText;
    [SerializeField] private TextMeshProUGUI _unitEnergyText;
    [SerializeField] private TextMeshProUGUI _unitWeaponNameText;

    public void SetData(string unitName, string unitHealth, string unitEnergy, string unitWeaponName)
    {
        _unitNameText.text = unitName;
        _unitHealthText.text = unitHealth;
        _unitEnergyText.text = unitEnergy;
        _unitWeaponNameText.text = unitWeaponName;
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
