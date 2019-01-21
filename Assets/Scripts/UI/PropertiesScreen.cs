using System.Collections.Generic;
using UINavigation;
using UnityEngine;

public class PropertiesScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private GameObject propertyPrefabButton = null;
    [SerializeField]
    private Transform propertyInfoContent = null;
    private List<GameObject> propertyButtons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        InstantiateProperties();
    }

    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().UpdatePropertyFields(property);
    }
    
    public void InstantiateProperties()
    {
        foreach (var propertyButton in propertyButtons)
        {
            Destroy(propertyButton);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton = Instantiate(propertyPrefabButton, propertyInfoContent);
            propertyButton.GetComponent<PropertyItem>().Initialize(property, () => OpenPropertyAdminScreen(property));
            propertyButtons.Add(propertyButton);
        }
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().UpdatePropertyFields(property);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }
}
