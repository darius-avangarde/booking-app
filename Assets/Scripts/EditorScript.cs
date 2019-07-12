using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;


public class EditorScript : EditorWindow
{
    string myString = "Hey";
    string recordButton = "Button";
    public List<GameObject> textList = new List<GameObject>();
   

    [MenuItem("Window/My Editor")]
    static void Init()
    {
        EditorScript window = (EditorScript)EditorWindow.GetWindow(typeof(EditorScript));
        window.Show();
    }

    public void FindTexts()
    {
        textList.Clear();
        Debug.Log("Clicked Button");
        foreach (var myText in GameObject.FindGameObjectsWithTag("text"))
        {

            textList.Add(myText);
        }
    }
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);
        if (GUILayout.Button("Button"))
        {
            FindTexts();
        }
    }
}
