using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPropertyTypeDropdown : MonoBehaviour
{
    public event Action<PropertyDataManager.PropertyType> ChangePropertyType = delegate { };

    [SerializeField]
    private Dropdown propertyTypeDropdown = null;
    [SerializeField]
    private Sprite[] propertyTypeIcons;

    public PropertyDataManager.PropertyType CurrentPropertyType
    {
        get => (PropertyDataManager.PropertyType)propertyTypeDropdown.value;
        set
        {
            propertyTypeDropdown.value = (int)value;
            propertyTypeDropdown.RefreshShownValue();
        }
    }

    private Dictionary<PropertyDataManager.PropertyType, string> propertyTypeName = new Dictionary<PropertyDataManager.PropertyType, string>();
    private Dictionary<string, Sprite> propertyIconsDictionary = new Dictionary<string, Sprite>();
    private string[] propertyTypeText = { "Hotel", "Pensiune", "Vilă", "Cabană" };

    private void Start()
    {
        SetDictionaries();
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => SetDictionaries());
    }

    public void SetPropertyType(int value)
    {
        ChangePropertyType?.Invoke((PropertyDataManager.PropertyType)value);
    }

    /// <summary>
    /// set every icon to a dictionary
    /// for loop goes from 0 to last element from PropertyType enum wich is cabin
    /// PropertyType enum is found in PropertyDataManager
    /// </summary>
    private void SetDictionaries()
    {
        propertyIconsDictionary.Clear();
        for (int i = 0; i < propertyTypeIcons.Length; i++)
        {
            propertyIconsDictionary.Add(propertyTypeIcons[i].name.ToLower(), propertyTypeIcons[i]);
        }
        for (int i = 0; i <= (int)PropertyDataManager.PropertyType.cabin; i++)
        {
            propertyTypeName[(PropertyDataManager.PropertyType)i] = LocalizedText.Instance.PropertyType[i];//propertyTypeText[i];
        }
    }

    /// <summary>
    /// set dropdown options with text and icons
    /// for every item in the enum, find the coresponding icon and text using dictionaries
    /// if a property with rooms is selected the dropdown options should update only with hotel and guesthouse
    /// if a property without rooms is selected the dropdown options should update only with villa and cabin
    /// </summary>
    public void SetDropdownOptions(int optionStart, int optionEnd)
    {
        propertyTypeDropdown.options.Clear();
        for (int i = optionStart; i <= optionEnd; i++)
        {
            propertyTypeName[(PropertyDataManager.PropertyType)i] = LocalizedText.Instance.PropertyType[i];
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = LocalizedText.Instance.PropertyType[i];
            foreach (KeyValuePair<string, Sprite> icon in propertyIconsDictionary)
            {
                if (icon.Key == ((PropertyDataManager.PropertyType)i).ToString())
                {
                    newOption.image = icon.Value;
                }
            }
            propertyTypeDropdown.options.Add(newOption);
        }
    }
}
