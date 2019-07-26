using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class ThemeManager : MonoBehaviour
{
    [SerializeField]
    private SettingsManager setMode;
    [SerializeField]
    private ColorsData dataColor;
    [SerializeField]
    private Toggle themeToggle;
    public List<Graphic> TextList = new List<Graphic>();
    public List<Graphic> BackgroundList = new List<Graphic>();
    public List<Graphic> SeparatorList = new List<Graphic>();
    public List<Graphic> ItemList = new List<Graphic>();
    private static ThemeManager instance;
    public static ThemeManager Instance { get { return instance; } }
    private int statusColor;
    void Start()
    {
        setMode = new SettingsManager();
        setMode.ReadData();
        statusColor = setMode.ReadData().settings.themeStatus;
        if (statusColor == 0)
        {
            themeToggle.isOn = true;
        }
        else
        {
            themeToggle.isOn = false;
        }
        //Debug.Log(statusColor+ "----theme sts");

    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void SelectTheme()
    {
        foreach (var item in TextList)
        {
            //Debug.Log(item.name);
            SetItemColor(item, dataColor.textDark, dataColor.textLight);
        }
        foreach (var item in BackgroundList)
        {
            SetItemColor(item, dataColor.darkBackground, dataColor.lightBackground);
        }
        foreach (var item in SeparatorList)
        {
            SetItemColor(item, dataColor.separatorDark, dataColor.separatorLight);
        }
        foreach (var item in ItemList)
        {
            SetItemColor(item, dataColor.ItemDark, dataColor.ItemLight);
        }
    }

    public void AddItems(Graphic myObject)
    {
        if (myObject.tag == "ItemBackground" && myObject != null)
        {
            ItemList.Add(myObject);
            //SelectTheme();
            //SetColor(myObject);
        }
        if (myObject.tag == "TextIcons" && myObject != null)
        {
            TextList.Add(myObject);
            //SelectTheme();
            
        }
        if (myObject.tag == "Separator" && myObject != null)
        {
            SeparatorList.Add(myObject);
            //SelectTheme();
        }
        if (myObject.tag == "Background" && myObject != null)
        {
            BackgroundList.Add(myObject);
            //SelectTheme();
        }
        SetColor(myObject);
    }

    public void SetColor(Graphic items)
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

    private void SetItemColor(Graphic items, Color dark, Color light)
    {
        if (themeToggle.isOn)
        {
            items.color = dark;
            setMode.DataElements.settings.themeStatus = 0;
            setMode.WriteData();
        }
        else
        {
            items.color = light;

            setMode.DataElements.settings.themeStatus = 1;
            setMode.WriteData();
        }
    }

    public void SetShadow(GameObject item)
    {
        var shadow = item.GetComponent<Shadow>();
        if (themeToggle.isOn)
        {
            shadow.effectColor = dataColor.separatorDark;
        }
        else
        {
            shadow.effectColor = dataColor.separatorLight;
        }
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
            var graphicItem = myText.GetComponent<Graphic>();
            TextList.Add(graphicItem);
        }
        //TextList = GameObject.FindGameObjectsWithTag("TextIcons").ToList();
        foreach (var myBg in GameObject.FindGameObjectsWithTag("Background"))
        {
            var graphicItem = myBg.GetComponent<Graphic>();
            BackgroundList.Add(graphicItem);
        }
        foreach (var separator in GameObject.FindGameObjectsWithTag("Separator"))
        {
            var graphicItem = separator.GetComponent<Graphic>();
            SeparatorList.Add(graphicItem);
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("ItemBackground"))
        {
            var graphicItem = item.GetComponent<Graphic>();
            ItemList.Add(graphicItem);

        }
    }
}