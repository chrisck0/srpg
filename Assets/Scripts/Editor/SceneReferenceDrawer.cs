using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    private const string PROP_SCENE_ASSET = "_sceneAsset";
    private const string PROP_SCENE_PATH = "_scenePath";
    private const string PROP_SCENE_NAME = "_sceneName";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty sceneAssetProp = property.FindPropertyRelative(PROP_SCENE_ASSET);
        if (sceneAssetProp == null)
        {
            EditorGUI.LabelField(position, label.text, "SceneReference : 씬 에셋 필드 없음");
            return;
        }

        DrawSceneAssetField(position, property, sceneAssetProp, label);
    }

    private void DrawSceneAssetField(Rect position, SerializedProperty property,
        SerializedProperty sceneAssetProp, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();

        Object newAsset = EditorGUI.ObjectField(position, label, sceneAssetProp.objectReferenceValue,
            typeof(SceneAsset), false);

        if (EditorGUI.EndChangeCheck())
        {
            sceneAssetProp.objectReferenceValue = newAsset;
            SyncStrings(property, newAsset as SceneAsset);
        }

        EditorGUI.EndProperty();
    }

    private void SyncStrings(SerializedProperty property, SceneAsset sceneAsset)
    {
        SerializedProperty pathProp = property.FindPropertyRelative(PROP_SCENE_PATH);
        SerializedProperty nameProp = property.FindPropertyRelative(PROP_SCENE_NAME);

        if (sceneAsset == null)
        {
            pathProp.stringValue = string.Empty;
            nameProp.stringValue = string.Empty;
            return;
        }

        string path = AssetDatabase.GetAssetPath(sceneAsset);
        pathProp.stringValue = path;
        nameProp.stringValue = Path.GetFileNameWithoutExtension(path);
    }
}
