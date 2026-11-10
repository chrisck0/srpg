using UnityEngine;

public class CommandUIController : MonoBehaviour
{
    [SerializeField] private UnitCommandUI _unitCommandUI;
    [SerializeField] private AttackCommandUI _attackCommandUI;
    [SerializeField] private TileInfoUI _tileInfoUI;
    [SerializeField] private UnitInfoUI _unitInfoUI;
    [SerializeField] private BattleInfoUI _battleInfoUI;

    public UnitCommandUI UnitCommandUI => _unitCommandUI;
    public AttackCommandUI AttackCommandUI => _attackCommandUI;
    public TileInfoUI TileInfoUI => _tileInfoUI;
    public UnitInfoUI UnitInfoUI => _unitInfoUI;
    public BattleInfoUI BattleInfoUI => _battleInfoUI;

    public void DeactivateAll()
    {
        _unitCommandUI.gameObject.SetActive(false);
        _attackCommandUI.gameObject.SetActive(false);
        _tileInfoUI.gameObject.SetActive(false);
        _unitInfoUI.gameObject.SetActive(false);
        _battleInfoUI.gameObject.SetActive(false);
    }
}
