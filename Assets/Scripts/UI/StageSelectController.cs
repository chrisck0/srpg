using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    [SerializeField] private SceneReference _nextScene;
    [SerializeField] private StageData _stageData;
    [SerializeField] private Button _stageButton;

    private void OnEnable()
    {
        BindButtonEvents();
    }

    private void OnDisable()
    {
        UnbindButtonEvents();
    }

    private void LoadScene()
    {
        GameManager.Instance.SetStageData(_stageData);
        SceneManager.LoadScene(_nextScene.SceneName);
    }

    private void BindButtonEvents()
    {
        _stageButton.onClick.AddListener(LoadScene);
    }

    private void UnbindButtonEvents()
    {
        _stageButton.onClick.RemoveListener(LoadScene);
    }
}
