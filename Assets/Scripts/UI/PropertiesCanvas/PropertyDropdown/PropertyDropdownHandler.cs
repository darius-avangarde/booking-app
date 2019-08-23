using System;
using System.Linq;
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

    private void Awake()
    {
        dropdownComponent = GetComponent<Dropdown>();
        dropdownComponent.onValueChanged.AddListener((value) => SelectDropdown(value));
        UpdateDropdown();
    }

    private void OnDestroy()
    {
        OnSelectProperty = null;
    }

    /// <summary>
    /// function set to property dropdown
    /// select a property from the list and invoke callback for selecting if skipInvoke is false
    /// </summary>
    /// <param name="value">dropdown value</param>
    /// <param name="skipInvoke">bool to skip invoking the property callback</param>
    private void SelectDropdown(int value, bool skipInvoke = false)
    {
        if (selectedProperty == null || !selectedProperty.Name.Equals(dropdownComponent.options[value].text))
        {
            selectedProperty = PropertyDataManager.GetProperty(propertyOptions[dropdownComponent.options[value]]);
        }
        if (!skipInvoke)
        {
            OnSelectProperty?.Invoke(selectedProperty);
        }
    }

    /// <summary>
    /// update dropdown properties list
    /// </summary>
    public void UpdateDropdown()
    {
        propertyOptions = new Dictionary<Dropdown.OptionData, string>();
        propertyDropdownOptions = new Dictionary<string, int>();
        List<IProperty> properties = PropertyDataManager.GetProperties().ToList();
        for (int i = 0; i < properties.Count; i++)
        {
            propertyOptions.Add(new Dropdown.OptionData(properties[i].Name), properties[i].ID);
            propertyDropdownOptions.Add(properties[i].ID, i);
        }
        dropdownComponent.options = propertyOptions.Keys.ToList();
        dropdownComponent.RefreshShownValue();
    }

    /// <summary>
    /// function to manually select a property from the dropdown
    /// </summary>
    /// <param name="property">property to select</param>
    /// <param name="skipInvoke">bool to skip invoking the property callback</param>
    public void SelectDropdownProperty(IProperty property, bool skipInvoke = true)
    {
        dropdownComponent.onValueChanged.RemoveAllListeners();
        SelectDropdown(propertyDropdownOptions[property.ID], skipInvoke);
        dropdownComponent.value = propertyDropdownOptions[property.ID];
        dropdownComponent.onValueChanged.AddListener((value) => SelectDropdown(value));
    }
}
