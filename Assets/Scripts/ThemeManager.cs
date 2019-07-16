using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [SerializeField]
    private Toggle themeToggle;
    public bool togggleInteraction;
    public List<GameObject> MyText { get; set; }
    public List<GameObject> MyBackground { get; set; }
    public List<GameObject> MySeparator { get; set; }
    public List<GameObject> MyItem { get; set; }

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
            SetItemColor(item, textDark, textLight);
        }
        foreach (var item in MyBackground)
        {
            SetItemColor(item, darkBackground, lightBackground);
        }
        foreach (var item in MySeparator)
        {
            SetItemColor(item, separatorDark, separatorLight);
        }
        foreach (var item in MyItem)
        {
            SetItemColor(item, ItemDark, ItemLight);
        }
    }

    public void AddItems(GameObject myObject)
    {
        MyItem.Clear();
        MyText.Clear();
        MySeparator.Clear();
        MyBackground.Clear();

        if (myObject.tag == "ItemBackground" && myObject != null)
        {
            MyItem.Add(myObject);
            SelectTheme();
        }
        if (myObject.tag == "TextIcons" && myObject != null)
        {
            MyText.Add(myObject);
            SelectTheme();
        }
        if (myObject.tag == "Separator" && myObject != null)
        {
            MySeparator.Add(myObject);
            SelectTheme();
        }
        if (myObject.tag == "Background" && myObject != null)
        {
            MyBackground.Add(myObject);
            SelectTheme();
        }
    }

    public void SetColor(GameObject items)
    {

        if (items.tag == "ItemBackground" && items != null)
        {
            SetItemColor(items, ItemDark, ItemLight);

        }
        if (items.tag == "TextIcons" && items != null)
        {
            SetItemColor(items, textDark, textLight);

        }
        if (items.tag == "Separator" && items != null)
        {
            SetItemColor(items, separatorDark, separatorLight);

        }
        if (items.tag == "Background" && items != null)
        {
            SetItemColor(items, darkBackground, lightBackground);

        }
    }
    public void Verify()
    {
        if (themeToggle.isOn)
        {
            togggleInteraction = true;
        }
        else
        {
            togggleInteraction = false;
        }
        SelectTheme();
    }

    private void SetItemColor(GameObject items, Color dark, Color light)
    {
        var colorBG = items.GetComponent<Graphic>();

        if (togggleInteraction)
        {
            colorBG.color = dark;
        }
        else
        {
            colorBG.color = light;
        }
    }
}
