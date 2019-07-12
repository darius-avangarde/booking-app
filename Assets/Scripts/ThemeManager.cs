using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public List<GameObject> MyText { get; set; }
    public List<GameObject> MyProperty { get; set; }
    public List<Graphic> MyTextColor { get; set; }
    public static Color textDark = Color.black ;
    [SerializeField]
    private Toggle themeToggle;
    void Start()
    {
        //#if UNITY_EDITOR
        MyText = EditorWindow.GetWindow<EditorScript>().textList;
     
        //#endif

       /* foreach (var item in MyText)
        {
            Debug.Log(item.name + "---- from normal script");
            var colors = item.GetComponent<Graphic>();
            colors.color = textDark;
           // MyTextColor.Add(colors);

        }*/
       
    }

    public void ChangeTheme()
    {
        if (themeToggle.isOn)
        {
            DarkTheme();
        }
        else
        {
            WhiteTheme();
        }
    }

    private void DarkTheme()
    {
        foreach (var item in MyText)
        {
            Debug.Log(item.name + "---- dark");
            var colors = item.GetComponent<Graphic>();
            colors.color = textDark;
            // MyTextColor.Add(colors);

        }
    }
    private void WhiteTheme()
    {
        foreach (var item in MyText)
        {
            Debug.Log(item.name + "---- white");
            var colors = item.GetComponent<Graphic>();
            colors.color = Color.white;

        }
    }
}
