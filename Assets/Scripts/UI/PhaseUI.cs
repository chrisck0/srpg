using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseUI : MonoBehaviour
{
    [SerializeField] private Image _phaseImage;
    [SerializeField] private TextMeshProUGUI _phaseText;
    private const string PLAYER_PHASE = "Player Phase";
    private const string ENEMY_PHASE = "Enemy Phase";

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayerPhaseText()
    {
        _phaseText.text = PLAYER_PHASE;
    }

    public void SetEnemyPhaseText()
    {
        _phaseText.text = ENEMY_PHASE;
    }
}
