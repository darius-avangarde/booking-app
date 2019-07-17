#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ThemeManager)), CanEditMultipleObjects]
public class EditorScript : Editor
{
    [SerializeField]
    private ThemeManager myManager;
    string recordButton = "Button";

    public override void OnInspectorGUI()
    {

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myManager = (ThemeManager)target;
        if (GUILayout.Button("Button"))
        {
            myManager.FindTexts();
        }
        DrawDefaultInspector();
    }

    void OnEnable()
    {
       // myManager = (ThemeManager)target;
       // myManager.FindTexts();
    }
}
#endif