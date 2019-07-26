using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [SerializeField]
    private  GameObject[] parents;
    [SerializeField]
    private SettingsManager setMode;
    [SerializeField]
    private ColorsData dataColor;
    public bool IsDarkTheme => themeToggle.isOn;
    public ColorsData ThemeColor => dataColor;
    [SerializeField]
    private Toggle themeToggle;
    public List<Graphic> TextList = new List<Graphic>();
    public List<Graphic> BackgroundList = new List<Graphic>();
    public List<Graphic> SeparatorList = new List<Graphic>();
    public List<Graphic> ItemList = new List<Graphic>();
    public List<Shadow> ShadowList = new List<Shadow>();
    private static ThemeManager instance;
    public static ThemeManager Instance { get { return instance; } }
    private int statusColor;
    public UnityAction<bool> OnThemeChanged;

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
        SelectTheme();
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
        foreach (Graphic item in TextList)
        {
            SetItemColor(dataColor.textDark, dataColor.textLight, item);
        }
        foreach (Graphic item in BackgroundList)
        {
            SetItemColor(dataColor.darkBackground, dataColor.lightBackground, item);
        }
        foreach (Graphic item in SeparatorList)
        {
            SetItemColor(dataColor.separatorDark, dataColor.separatorLight, item);
        }
        foreach (Graphic item in ItemList)
        {
            SetItemColor(dataColor.ItemDark, dataColor.ItemLight, item);
        }
        foreach (Shadow item in ShadowList)
        {
            SetItemColor(dataColor.separatorDark, dataColor.separatorLight, null, item);
        }

    }

    public void AddItems(params Graphic[] myObjects)
    {
        foreach (Graphic myObject in myObjects)
        {
            if (myObject.tag == "ItemBackground" && myObject != null)
            {
                ItemList.Add(myObject); //SelectTheme();
            }
            if (myObject.tag == "TextIcons" && myObject != null)
            {
                TextList.Add(myObject);
            }
            if (myObject.tag == "Separator" && myObject != null)
            {
                SeparatorList.Add(myObject);
            }
            if (myObject.tag == "Background" && myObject != null)
            {
                BackgroundList.Add(myObject);
            }
            SetColor(myObject);
        }
    }

    public void SetColor(Graphic items)
    {
        if (items.tag == "ItemBackground" && items != null)
        {
            SetItemColor(dataColor.ItemDark, dataColor.ItemLight, items);
        }
        if (items.tag == "TextIcons" && items != null)
        {
            SetItemColor(dataColor.textDark, dataColor.textLight, items);
        }
        if (items.tag == "Separator" && items != null)
        {
            SetItemColor(dataColor.separatorDark, dataColor.separatorLight, items);
        }
        if (items.tag == "Background" && items != null)
        {
            SetItemColor(dataColor.darkBackground, dataColor.lightBackground, items);
        }
    }

    private void SetItemColor(Color dark, Color light, Graphic items = null, Shadow myShadow = null)
    {
        if (themeToggle.isOn)
        {

            if (items != null)
            {
                items.color = dark;
            }
            if (myShadow != null)
            {
                myShadow.effectColor = dark;
            }

            setMode.DataElements.settings.themeStatus = 0;
            setMode.WriteData();
        }
        else
        {
            if (items != null)
            {
                items.color = light;
            }
            if (myShadow != null)
            {
                myShadow.effectColor = light;
            }
            setMode.DataElements.settings.themeStatus = 1;
            setMode.WriteData();
        }
        OnThemeChanged?.Invoke(themeToggle.isOn);
    }

    public void SetShadow(GameObject item)
    {
        Shadow shadow = item.GetComponent<Shadow>();
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
        ShadowList.Clear();
        Debug.Log("Clicked Button");
        Graphic[] elements = new Graphic[700];
        foreach (var item in parents)
        {
            elements = item.GetComponentsInChildren<Graphic>(true);
            foreach (var elem in elements)
            {
                if (elem.gameObject.tag == "Background")
                {
                    BackgroundList.Add(elem);
                }
                if (elem.gameObject.tag == "TextIcons")
                {
                    TextList.Add(elem);
                }
                if (elem.gameObject.tag == "Separator")
                {
                    SeparatorList.Add(elem);
                }
                if (elem.gameObject.tag == "ItemBackground")
                {
                    ItemList.Add(elem);
                }
            }
            //Debug.Log(elements.Length);
        }
        foreach (GameObject separator in GameObject.FindGameObjectsWithTag("ItemBackground"))
        {
            Shadow shadow = separator.GetComponent<Shadow>();
            if (shadow != null)
            {
                ShadowList.Add(shadow);
            }
        }
    }
}
