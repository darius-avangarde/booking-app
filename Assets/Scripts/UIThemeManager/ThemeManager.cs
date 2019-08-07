using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] parents;
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
    private bool result = true;
    void Start()
    {
        // themeToggle.onValueChanged.RemoveAllListeners();
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
        StartCoroutine(SelectThemeCo());
    }

    private IEnumerator SelectThemeCo()
    {
#if UNITY_IPHONE
        Handheld.SetActivityIndicatorStyle(iOS.ActivityIndicatorStyle.Gray);
#elif UNITY_ANDROID
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
#endif
        Handheld.StartActivityIndicator();

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
        yield return new WaitForEndOfFrame();
        Verify();
        yield return new WaitForEndOfFrame();
        Handheld.StopActivityIndicator();
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

    public void AddShadow(Shadow myShadow)
    {
        ShadowList.Add(myShadow);
        SetShadow(myShadow);
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


    public void Verify()
    {

        if (themeToggle.isOn)
        {
            setMode.DataElements.settings.themeStatus = 0;
            setMode.WriteData();
        }
        else
        {
            setMode.DataElements.settings.themeStatus = 1;
            setMode.WriteData();
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

        }
        OnThemeChanged?.Invoke(themeToggle.isOn);
    }

    public void SetShadow(Shadow shadow)
    {
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
        Graphic[] elements = new Graphic[700];
        for (int i = 0; i < parents.Length; i++)
        {
            elements = parents[i].GetComponentsInChildren<Graphic>(true);
            foreach (Graphic elem in elements)
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
