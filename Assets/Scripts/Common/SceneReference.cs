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
}