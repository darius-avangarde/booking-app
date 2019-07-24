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
            LocalizationManager.Instance.ReadFromCSV();//("D:\\Booking\\Assets\\Resources\\TextsFileEx.csv");
             myEditor.GetTexts();
        }
        DrawDefaultInspector();
    }
}
#endif