using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public List<GameObject> MyText { get; set; }
    public List<GameObject> MyBackground { get; set; }
    public List<GameObject> MySeparator { get; set; }
    public List<GameObject> MyItem { get; set; }
    [SerializeField]
    private Toggle themeToggle;
    #region ColorsTheme
    public readonly static Color darkBackground = new Color(0, 0, 0);
    public readonly static Color lightBackground = new Color(0.8862745f, 0.8862745f, 0.8862745f);
    public readonly static Color separatorDark = new Color(0.5058824f, 0.5058824f, 0.5058824f);
    public readonly static Color separatorLight = new Color(0.282353f, 0.282353f, 0.282353f);
    public readonly static Color textDark = new Color(0.9607843f, 0.9607843f, 0.9607843f);
    public readonly static Color textLight = new Color(0.2235294f, 0.2235294f, 0.2235294f);
    public readonly static Color ItemDark = new Color(0.1254902f, 0.1254902f, 0.1254902f);
    public readonly static Color ItemLight = new Color(0.945098f, 0.945098f, 0.945098f);
    #endregion
    void Start()
    {
        //#if UNITY_EDITOR
        MyText = EditorWindow.GetWindow<EditorScript>().TextList;
        MyBackground = EditorWindow.GetWindow<EditorScript>().BackgroundList;
        MySeparator = EditorWindow.GetWindow<EditorScript>().SeparatorList;
        MyItem = EditorWindow.GetWindow<EditorScript>().ItemList;

        //#endif
    }

    public void SelectTheme()
    {
        foreach (var item in MyText)
        {
            var colorText = item.GetComponent<Graphic>();
            if (!themeToggle.isOn)
            {
                colorText.color = textDark;
            }
            else
            {
                colorText.color = textLight;
            }
           
        }
        foreach (var item in MyBackground)
        {
            Debug.Log(item.name + "---- bgd");
            var colorBG = item.GetComponent<Graphic>();
            if (!themeToggle.isOn)
            {
                colorBG.color = darkBackground;
            }
            else
            { colorBG.color = lightBackground;
            }
          
        }
        foreach (var item in MySeparator)
        {
            var colorSeparator = item.GetComponent<Graphic>();
            if (!themeToggle.isOn)
            {
                colorSeparator.color = separatorDark;
            }
            else
            {
                colorSeparator.color = separatorLight;
            }
        }
        foreach (var item in MyItem)
        {
            var colorItem = item.GetComponent<Graphic>();
            if (!themeToggle.isOn)
            {
                colorItem.color = ItemDark;
            }
            else
            {
                colorItem.color = ItemLight;
            }
        }
    }
    
}
