using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [SerializeField]
    private ColorsData dataColor;
    [SerializeField]
    private Toggle themeToggle;
    public bool togggleInteraction;
    public List<GameObject> MyText { get; set; }
    public List<GameObject> MyBackground { get; set; }
    public List<GameObject> MySeparator { get; set; }
    public List<GameObject> MyItem { get; set; }

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
            SetItemColor(item, dataColor.textDark, dataColor.textLight);
        }
        foreach (var item in MyBackground)
        {
            SetItemColor(item, dataColor.darkBackground, dataColor.lightBackground);
        }
        foreach (var item in MySeparator)
        {
            SetItemColor(item, dataColor.separatorDark, dataColor.separatorLight);
        }
        foreach (var item in MyItem)
        {
            SetItemColor(item, dataColor.ItemDark, dataColor.ItemLight);
        }
    }

    public void AddItems(GameObject myObject)
    {
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
            SetItemColor(items, dataColor.ItemDark, dataColor.ItemLight);

        }
        if (items.tag == "TextIcons" && items != null)
        {
            SetItemColor(items, dataColor.textDark, dataColor.textLight);

        }
        if (items.tag == "Separator" && items != null)
        {
            SetItemColor(items, dataColor.separatorDark, dataColor.separatorLight);

        }
        if (items.tag == "Background" && items != null)
        {
            SetItemColor(items, dataColor.darkBackground, dataColor.lightBackground);

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

        if (themeToggle.isOn)
        {
            colorBG.color = dark;
        }
        else
        {
            colorBG.color = light;
        }
    }
}
