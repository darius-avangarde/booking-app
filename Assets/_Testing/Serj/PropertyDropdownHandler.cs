using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyDropdownHandler : MonoBehaviour
{
    public Action<IProperty> OnSelectProperty;

    private Dropdown dropdownComponent = null;
    private Dictionary<Dropdown.OptionData, string> propertyOptions;
    private Dictionary<string, int> propertyDropdownOptions;
    private IProperty selectedProperty;
    private bool invokeAction = true;
    private bool onValueChangeCall = true;

    private void Awake()
    {
        dropdownComponent = GetComponent<Dropdown>();
        dropdownComponent.onValueChanged.AddListener((value) => SelectDropdown(value));
    }

    private void OnEnable ()
    {
        propertyOptions = new Dictionary<Dropdown.OptionData, string>();
        propertyDropdownOptions = new Dictionary<string, int>();
        int propertyCount = 0;
        foreach (var property in PropertyDataManager.GetProperties())
        {
            propertyOptions.Add(new Dropdown.OptionData(property.Name), property.ID);
            propertyDropdownOptions.Add(property.ID, propertyCount);
            propertyCount++;
        }
        dropdownComponent.options = propertyOptions.Keys.ToList();
        dropdownComponent.RefreshShownValue();
    }

    private void SelectDropdown(int value, bool skipInvoke = false)
    {
        if (!onValueChangeCall)
        {
            onValueChangeCall = true;
            return;
        }
        if(selectedProperty == null || selectedProperty.Name != dropdownComponent.options[value].text)
        {
            selectedProperty = PropertyDataManager.GetProperty(propertyOptions[dropdownComponent.options[value]]);
            Debug.Log(selectedProperty.Name);
        }
        if (!skipInvoke)
        {
            OnSelectProperty?.Invoke(selectedProperty);
        }
    }

    public void SelectDropdownProperty(IProperty property, bool skipInvoke = false)
    {
        SelectDropdown(propertyDropdownOptions[property.ID], skipInvoke);
        onValueChangeCall = false;
        dropdownComponent.value = propertyDropdownOptions[property.ID];
    }
}
