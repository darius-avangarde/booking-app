using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyDropdown : MonoBehaviour
{
    [SerializeField]
    private Dropdown propertiesDropdown = null;

    private List<IProperty> propertyOptions = new List<IProperty>();

    public IProperty SelectedProperty
    {
        get => propertyOptions[propertiesDropdown.value];
        set => propertiesDropdown.value = propertyOptions.IndexOf(value);
    }
    public PropertyUnityEvent OnSelectionChanged;

    private void OnEnable()
    {
        Initialize();
    }

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
    }

    public void ClearSelection()
    {
        propertiesDropdown.value = 0;
    }
}
