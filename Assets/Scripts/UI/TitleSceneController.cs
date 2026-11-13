using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private InputActionReference _leftClickInput;
    [SerializeField] private SceneReference _nextScene;

    private void OnEnable()
    {
        BindInputActions();
    }

    private void OnDisable()
    {
        UnbindInputActions();
    }

    private void LoadNextScene(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(_nextScene.SceneName);
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
