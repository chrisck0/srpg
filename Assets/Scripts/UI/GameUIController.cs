using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameWinLoseConditionUI _gameWinLoseConditionUI;
    [SerializeField] private PhaseUI _phaseUI;
    [SerializeField] private ResultUI _resultUI;

    public GameWinLoseConditionUI GameWinLoseConditionUI => _gameWinLoseConditionUI;
    public PhaseUI PhaseUI => _phaseUI;
    public ResultUI ResultUI => _resultUI;

    public void DeactivateAll()
    {
        _gameWinLoseConditionUI.gameObject.SetActive(false);
        _phaseUI.gameObject.SetActive(false);
        _resultUI.gameObject.SetActive(false);
    }
}
