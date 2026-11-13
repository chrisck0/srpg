using TMPro;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _unitNameText;
    [SerializeField] private TextMeshProUGUI _unitHealthText;
    [SerializeField] private TextMeshProUGUI _unitEnergyText;
    [SerializeField] private TextMeshProUGUI _unitWeaponNameText;

    public void SetData(UnitBase selectedUnit)
    {
        _unitNameText.text = selectedUnit.UnitStat.UnitData.Name;
        _unitHealthText.text = $"{selectedUnit.UnitStat.Health} / {selectedUnit.UnitStat.UnitData.Health}";
        _unitEnergyText.text = $"{selectedUnit.UnitStat.Energy} / {selectedUnit.UnitStat.UnitData.Energy}";
        _unitWeaponNameText.text = selectedUnit.Weapon.WeaponDataList[0].Name;
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
