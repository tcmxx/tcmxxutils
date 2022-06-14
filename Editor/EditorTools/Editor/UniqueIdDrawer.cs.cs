using UnityEditor;
using UnityEngine;
using System;


[CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
public class UniqueIdentifierDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        //check if the gameobject associated is a prefab
        GameObject go = ((MonoBehaviour)prop.serializedObject.targetObject).gameObject;
#if UNITY_2018_3_OR_NEWER
        bool isPrefab = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null || EditorUtility.IsPersistent(go);
#else
        bool isPrefab = EditorUtility.IsPersistent(go);
#endif
        // Generate a unique ID, defaults to an empty string if nothing has been serialized yet and it is not a prefab
        if (isPrefab)
        {
            prop.stringValue = "";
        }
        else if (!isPrefab && prop.stringValue == "")
        {
            Guid guid = Guid.NewGuid();
            prop.stringValue = guid.ToString();
        }

        // Place a label so it can't be edited by accident
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, prop, label);
    }

    void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)
    {
        //EditorGUI.PropertyField(position, prop, label);
        //EditorGUI.TextField(position, label, new GUIContent(prop.stringValue));
        EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
    }
}