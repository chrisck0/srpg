using System;
using UnityEngine;
#if UNITY_EDITOR // 유니티 에디터에서만 동작하도록 하는 구문. (전처리기)
using UnityEditor;
#endif

[Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _sceneAsset;
#endif

    [SerializeField] private string _scenePath;
    [SerializeField] private string _sceneName;

    public string ScenePath => _scenePath;
    public string SceneName => _sceneName;
    public bool IsValid => !string.IsNullOrEmpty(_sceneName);

    public void OnBeforeSerialize() => SyncFromSceneAsset();

    public void OnAfterDeserialize()
    {
    }

    private void SyncFromSceneAsset()
    {
#if UNITY_EDITOR
        if (_sceneAsset == null)
        {
            _scenePath = string.Empty;
            _sceneName = string.Empty;
            return;
        }

        _scenePath = AssetDatabase.GetAssetPath(_sceneAsset);
        _sceneName = System.IO.Path.GetFileNameWithoutExtension(_scenePath);
#endif
    }
}
