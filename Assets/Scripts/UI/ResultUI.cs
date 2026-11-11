using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private SceneController _nextStageSceneController;
    [SerializeField] private SceneController _mainMenuSceneController;
    [SerializeField] private GameObject _gameClearText;
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private TextMeshProUGUI _killedEnemyText;
    [SerializeField] private TextMeshProUGUI _lostPlayerText;
    [SerializeField] private Button _nextStageButton;
    [SerializeField] private Button _mainMenuButton;

    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    public void SetData(bool isGameClear, int turn, int killedEnemy, int lostPlayer)
    {
        if (isGameClear)
        {
            _gameClearText.SetActive(true);
            _gameOverText.SetActive(false);
        }
        else
        {
            _gameClearText.SetActive(false);
            _gameOverText.SetActive(true);
        }

        _turnText.text = turn.ToString();
        _killedEnemyText.text = killedEnemy.ToString();
        _lostPlayerText.text = lostPlayer.ToString();

        gameObject.SetActive(true);
    }

    private void LoadNextStage()
    {
        _nextStageSceneController.LoadNextScene();
    }

    private void LoadMainMenu()
    {
        _mainMenuSceneController.LoadNextScene();
    }

    private void BindButtonEvents()
    {
        _nextStageButton.onClick.AddListener(LoadNextStage);
        _mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void UnbindButtonEvents()
    {
        _nextStageButton.onClick.RemoveListener(LoadNextStage);
        _mainMenuButton.onClick.RemoveListener(LoadMainMenu);
    }
}
