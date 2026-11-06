using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Build Settings - Scene List에 해당 씬이 등록되어 있어야 함.
    [SerializeField] private SceneReference _nextScene;
    [SerializeField] private DataLoader _dataLoader;
    [SerializeField] private InputActionReference _leftClickInput;
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

    public void LoadNextScene(InputAction.CallbackContext context)
    {
        if (!_nextScene.IsValid)
        {
            Debug.LogWarning("SceneReferenceSample : 씬이 연결되지 않았음");
            return;
        }

        if (!_isDataLoaded) return;

        SceneManager.LoadScene(_nextScene.SceneName);
    }

    private void DataLoaded()
    {
        _isDataLoaded = true;
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

    private void BindLoadEvents()
    {
        _dataLoader.OnDataLoaded += DataLoaded;
    }

    private void UnbindLoadEvents()
    {
        _dataLoader.OnDataLoaded -= DataLoaded;
    }
}
