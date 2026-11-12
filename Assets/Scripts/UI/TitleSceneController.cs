using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private DataLoader _dataLoader;
    [SerializeField] private InputActionReference _leftClickInput;
    [SerializeField] private SceneReference _nextScene;
    private bool _isDataLoaded;

    private void OnEnable()
    {
        BindInputActions();
        BindLoadEvents();
    }

    private void OnDisable()
    {
        UnbindInputActions();
        UnbindLoadEvents();
    }

    private void LoadNextScene(InputAction.CallbackContext context)
    {
        if (!_isDataLoaded) return;

        SceneManager.LoadScene(_nextScene.SceneName);
    }

    private void DataLoaded()
    {
        _isDataLoaded = true;
    }

    private void BindLoadEvents()
    {
        _dataLoader.OnDataLoaded += DataLoaded;
    }

    private void UnbindLoadEvents()
    {
        _dataLoader.OnDataLoaded -= DataLoaded;
    }

    private void BindInputActions()
    {
        _leftClickInput.action.Enable();
        _leftClickInput.action.performed += LoadNextScene;
    }

    private void UnbindInputActions()
    {
        _leftClickInput.action.Disable();
        _leftClickInput.action.performed -= LoadNextScene;
    }
}
