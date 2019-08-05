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
        }
    }

    private Dictionary<PropertyDataManager.PropertyType, string> propertyTypeName = new Dictionary<PropertyDataManager.PropertyType, string>();
    private Dictionary<string, Sprite> propertyIconsDictionary = new Dictionary<string, Sprite>();
    private string[] propertyTypeText = { "Hotel", "Pensiune", "Vilă", "Cabană" };

    private void Awake()
    {
        SetDictionaries();
        SetDropdownOptions();
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
        foreach (var icon in propertyTypeIcons)
        {
            propertyIconsDictionary.Add(icon.name.ToLower(), icon);
        }
        for (int i = 0; i <= (int)PropertyDataManager.PropertyType.cabin; i++)
        {
            propertyTypeName[(PropertyDataManager.PropertyType)i] = propertyTypeText[i];
        }
    }

    /// <summary>
    /// set dropdown options with text and icons
    /// for every item in the enum, find the coresponding icon and text using dictionaries
    /// </summary>
    private void SetDropdownOptions()
    {
        propertyTypeDropdown.options.Clear();
        for (int i = 0; i <= (int)PropertyDataManager.PropertyType.cabin; i++)
        {
            propertyTypeName[(PropertyDataManager.PropertyType)i] = propertyTypeText[i];
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = propertyTypeText[i];
            foreach (var icon in propertyIconsDictionary)
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
