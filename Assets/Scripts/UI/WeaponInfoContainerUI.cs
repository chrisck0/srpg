using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoContainerUI : MonoBehaviour, IPoolable
{
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _energyCostText;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {

    }

    public void SetData(WeaponData weaponData)
    {
        _nameText.text = weaponData.Name;
        _attackText.text = weaponData.Attack.ToString();
        _energyCostText.text = weaponData.EnergyCost.ToString();
    }
}
