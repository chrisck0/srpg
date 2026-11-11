using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private PhaseUI _phaseUI;

    public PhaseUI PhaseUI => _phaseUI;

    public void DeactivateAll()
    {
        _phaseUI.gameObject.SetActive(false);
    }
}
