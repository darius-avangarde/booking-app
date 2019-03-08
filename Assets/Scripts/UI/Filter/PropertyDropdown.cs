using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyDropdown : MonoBehaviour
{
    [SerializeField]
    private Dropdown propertiesDropdown = null;

    [SerializeField]
    private Button clearButton = null;

    private List<IProperty> propertyOptions = new List<IProperty>();

    public IProperty SelectedProperty
    {
        get => propertyOptions[propertiesDropdown.value];
        set => propertiesDropdown.value = propertyOptions.IndexOf(value);
    }

    public PropertyUnityEvent OnSelectionChanged;

    public void Initialize()
    {
        propertyOptions = new List<IProperty>()
        {
            null // first option is "don't filter by property"
        };
        propertyOptions.AddRange(PropertyDataManager.GetProperties());

        propertiesDropdown.ClearOptions();
        propertiesDropdown.AddOptions(propertyOptions.ConvertAll<string>(p => p?.Name));
    }

    public void HandleValueChanged()
    {
        OnSelectionChanged.Invoke(SelectedProperty);

        if (SelectedProperty == null)
        {
            clearButton.interactable = false;
        }
        else if (!clearButton.interactable)
        {
            clearButton.interactable = true;
        }
    }

    public void ClearSelection()
    {
        propertiesDropdown.value = 0;
    }
}
