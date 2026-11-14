using UnityEngine;
using UnityEngine.UI;

public class EnemyDangerRangeUI : MonoBehaviour
{
    [SerializeField] private Image _dangerRangeImage;
    [SerializeField] private EnemyAI _enemyAI;

    private void Awake() => ToggleDangerRangeImage(false);
    private void OnEnable() => BindEnemyAIEvents();
    private void OnDisable() => UnbindEnemyAIEvents();

    private void ToggleDangerRangeImage(bool isShowingDanger)
    {
        _dangerRangeImage.gameObject.SetActive(isShowingDanger);
    }

    private void BindEnemyAIEvents()
    {
        _enemyAI.OnDangerRangeVisibilityChanged += ToggleDangerRangeImage;
    }

    private void UnbindEnemyAIEvents()
    {
        _enemyAI.OnDangerRangeVisibilityChanged -= ToggleDangerRangeImage;
    }
}
