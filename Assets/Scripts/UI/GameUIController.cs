using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private PhaseUI _phaseUI;
    [SerializeField] private ResultUI _resultUI;

    public PhaseUI PhaseUI => _phaseUI;
    public ResultUI ResultUI => _resultUI;

    public void DeactivateAll()
    {
        _phaseUI.gameObject.SetActive(false);
        _resultUI.gameObject.SetActive(false);
    }
}
