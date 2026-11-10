using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoUI : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI _playerUnitNameText;
    [SerializeField] private TextMeshProUGUI _playerUnitWeaponText;
    [SerializeField] private TextMeshProUGUI _playerUnitEnergyCostText;
    [SerializeField] private Image _playerUnitImage;
    [SerializeField] private Image _playerResultHealthGauge;
    [SerializeField] private Image _playerCurrentHealthGauge;
    [SerializeField] private TextMeshProUGUI _playerResultHealthText;
    [SerializeField] private TextMeshProUGUI _playerCurrentHealthText;
    [SerializeField] private TextMeshProUGUI _playerEnergyText;
    [SerializeField] private TextMeshProUGUI _playerAttackText;
    [SerializeField] private TextMeshProUGUI _playerProbabilityText;

    [Header("Enemy Info")]
    [SerializeField] private TextMeshProUGUI _enemyUnitNameText;
    [SerializeField] private TextMeshProUGUI _enemyUnitWeaponText;
    [SerializeField] private TextMeshProUGUI _enemyUnitEnergyCostText;
    [SerializeField] private Image _enemyUnitImage;
    [SerializeField] private Image _enemyResultHealthGauge;
    [SerializeField] private Image _enemyCurrentHealthGauge;
    [SerializeField] private TextMeshProUGUI _enemyResultHealthText;
    [SerializeField] private TextMeshProUGUI _enemyCurrentHealthText;
    [SerializeField] private TextMeshProUGUI _enemyEnergyText;
    [SerializeField] private TextMeshProUGUI _enemyAttackText;
    [SerializeField] private TextMeshProUGUI _enemyProbabilityText;

    public void SetData(UnitStat playerStat, UnitStat enemyStat,
        WeaponData playerWeaponData, WeaponData enemyWeaponData,
        bool canReturnAttack, bool isPlayerPhase)
    {
        // Player Info
        _playerUnitNameText.text = playerStat.UnitData.Name.ToString();
        _playerUnitWeaponText.text = playerWeaponData.Name.ToString();
        _playerUnitEnergyCostText.text = playerWeaponData.EnergyCost.ToString();
        // _playerUnitImage TODO: 나중에 추가

        int playerMaxHealth = playerStat.UnitData.Health;
        int playerCurrentHealth = playerStat.Health;
        int playerResultHealth = playerCurrentHealth;

        if (canReturnAttack && isPlayerPhase
            || !isPlayerPhase)
        {
            playerResultHealth = playerStat.Health - enemyWeaponData.Attack;
        }

        _playerResultHealthGauge.transform.localScale = new Vector3(
            (float) playerResultHealth / playerMaxHealth,
            _playerResultHealthGauge.transform.localScale.y,
            _playerResultHealthGauge.transform.localScale.z);

        _playerCurrentHealthGauge.transform.localScale = new Vector3(
            (float) playerCurrentHealth / playerMaxHealth,
            _playerCurrentHealthGauge.transform.localScale.y,
            _playerCurrentHealthGauge.transform.localScale.z);

        _playerResultHealthText.text = playerResultHealth.ToString();
        _playerCurrentHealthText.text = playerCurrentHealth.ToString();

        _playerEnergyText.text = playerStat.Energy.ToString();

        if (!canReturnAttack && !isPlayerPhase)
        {
            _playerAttackText.text = "-";
            _playerProbabilityText.text = "-";
        }
        else
        {
            _playerAttackText.text = playerWeaponData.Attack.ToString();
            _playerProbabilityText.text = ProbabilityCalculator.GetAttackProbability(playerStat, enemyStat).ToString();
        }

        // Enemy Info
        _enemyUnitNameText.text = enemyStat.UnitData.Name.ToString();
        _enemyUnitWeaponText.text = enemyWeaponData.Name.ToString();
        _enemyUnitEnergyCostText.text = enemyWeaponData.EnergyCost.ToString();
        // _enemyUnitImage TODO: 나중에 추가

        int enemyMaxHealth = enemyStat.UnitData.Health;
        int enemyCurrentHealth = enemyStat.Health;
        int enemyResultHealth = enemyCurrentHealth;

        if (canReturnAttack && !isPlayerPhase
            || isPlayerPhase)
        {
            enemyResultHealth = enemyStat.Health - playerWeaponData.Attack;
        }

        _enemyResultHealthGauge.transform.localScale = new Vector3(
            (float) enemyResultHealth / enemyMaxHealth,
            _enemyResultHealthGauge.transform.localScale.y,
            _enemyResultHealthGauge.transform.localScale.z);

        _enemyCurrentHealthGauge.transform.localScale = new Vector3(
            (float) enemyCurrentHealth / enemyMaxHealth,
            _enemyCurrentHealthGauge.transform.localScale.y,
            _enemyCurrentHealthGauge.transform.localScale.z);

        _enemyResultHealthText.text = enemyResultHealth.ToString();
        _enemyCurrentHealthText.text = enemyCurrentHealth.ToString();

        _enemyEnergyText.text = enemyStat.Energy.ToString();

        if (!canReturnAttack && isPlayerPhase)
        {
            _enemyAttackText.text = "-";
            _enemyProbabilityText.text = "-";
        }
        else
        {
            _enemyAttackText.text = enemyWeaponData.Attack.ToString();
            _enemyProbabilityText.text = ProbabilityCalculator.GetAttackProbability(enemyStat, playerStat).ToString();
        }
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
