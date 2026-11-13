using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _stageSelectUI;
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;

    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnbindButtonEvents();

    private void GameStart()
    {
        gameObject.SetActive(false);
        _stageSelectUI.SetActive(true);
    }

    private void Settings()
    {
        Debug.Log("Settings");
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void BindButtonEvents()
    {
        _gameStartButton.onClick.AddListener(GameStart);
        _settingsButton.onClick.AddListener(Settings);
        _quitButton.onClick.AddListener(Quit);
    }

    private void UnbindButtonEvents()
    {
        _gameStartButton.onClick.RemoveListener(GameStart);
        _settingsButton.onClick.RemoveListener(Settings);
        _quitButton.onClick.RemoveListener(Quit);
    }
}
