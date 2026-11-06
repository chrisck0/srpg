using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReferenceSample : MonoBehaviour
{
    // Build Settings - Scene List에 해당 씬이 등록되어 있어야 함.
    [SerializeField] private SceneReference _nextScene;

    public void LoadNextScene()
    {
        if (!_nextScene.IsValid)
        {
            Debug.LogWarning("SceneReferenceSample : 씬이 연결되지 않았음");
            return;
        }

        SceneManager.LoadScene(_nextScene.SceneName);
    }
}
