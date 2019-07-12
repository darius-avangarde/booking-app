using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;


public class EditorScript : EditorWindow
{
    string recordButton = "Button";
    public List<GameObject> TextList = new List<GameObject>();
    public List<GameObject> BackgroundList = new List<GameObject>();
    public List<GameObject> SeparatorList = new List<GameObject>();
    public List<GameObject> ItemList = new List<GameObject>();

    [MenuItem("Window/My Editor")]
    static void Init()
    {
        EditorScript window = (EditorScript)EditorWindow.GetWindow(typeof(EditorScript));
        window.Show();
    }

    public void FindTexts()
    {
        TextList.Clear();
        BackgroundList.Clear();
        SeparatorList.Clear();
        ItemList.Clear();
        Debug.Log("Clicked Button");
        foreach (var myText in GameObject.FindGameObjectsWithTag("TextIcons"))
        {

            TextList.Add(myText);
        }
        foreach (var myBg in GameObject.FindGameObjectsWithTag("Background"))
        {

            BackgroundList.Add(myBg);
        }
        foreach (var separator in GameObject.FindGameObjectsWithTag("Separator"))
        {
            SeparatorList.Add(separator);
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("ItemBackground"))
        {
            ItemList.Add(item);
            Debug.Log(item.name);
        }
    }
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        if (GUILayout.Button("Button"))
        {
            FindTexts();
        }
    }
}
