#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(LocalizedText)), CanEditMultipleObjects]
public class LanguageEditorScript : Editor
{
    [SerializeField]
    private LocalizedText myEditor;
    string recordButton = "ButtonLanguage";

    public override void OnInspectorGUI()
    {

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myEditor = (LocalizedText)target;
        if (GUILayout.Button("ButtonLanguage"))
        {
            myEditor.GetTexts();
        }
        DrawDefaultInspector();
    }
}
#endif