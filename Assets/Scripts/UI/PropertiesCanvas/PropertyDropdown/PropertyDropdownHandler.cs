using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PropertyDropdownHandler : MonoBehaviour
{
    [SerializeField]
    private Dropdown dropdownComponent = null;

    private Dictionary<Dropdown.OptionData, string> propertyOptions;
    private IProperty selectedProperty;

    private void OnEnable ()
    {
        propertyOptions = new Dictionary<Dropdown.OptionData, string>();
        foreach (var property in PropertyDataManager.GetProperties())
        {
            propertyOptions.Add(new Dropdown.OptionData(property.Name), property.ID);
        }
        dropdownComponent.options = propertyOptions.Keys.ToList();
        dropdownComponent.RefreshShownValue();
    }

    public void SelectDropdown(int value)
    {
        if(selectedProperty == null || selectedProperty.Name != dropdownComponent.options[value].text)
        {
            selectedProperty = PropertyDataManager.GetProperty(propertyOptions[dropdownComponent.options[value]]);
        }
    }
}

