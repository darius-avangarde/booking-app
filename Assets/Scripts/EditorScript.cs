using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;


public class EditorScript : EditorWindow
{
    string myString = "Hey";
    string recordButton = "MyButton";
    public List<GameObject> textList = new List<GameObject>();
    private ThemeManager manager;

    [MenuItem("Window/My Editor")]
    static void Init()
    {
        EditorScript window = (EditorScript)EditorWindow.GetWindow(typeof(EditorScript));
        window.Show();
    }

    public void FindTexts()
    {
      
        Debug.Log("Clicked Button");
        foreach (GameObject myText in GameObject.FindGameObjectsWithTag("tagtext"))
        {

            textList.Add(myText);
        }
        foreach (var item in textList)
        {
            var ex = item.GetComponent<Text>();
            ex.color =  Color.red;
        }
    }
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);
        if (GUILayout.Button("MyButton"))
        {
            FindTexts();
        }
    }
}
