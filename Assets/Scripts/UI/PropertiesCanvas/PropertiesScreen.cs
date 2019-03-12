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
    private IProperty currentProperty;

    public void InstantiateProperties()
    {
        foreach (var propertyButton in propertyButtons)
        {
            Destroy(propertyButton);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton = Instantiate(propertyPrefabButton, propertyInfoContent);
            propertyButton.GetComponent<PropertyButton>().Initialize(property, OpenPropertyAdminScreen);
            propertyButtons.Add(propertyButton);
        }
    }

    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        currentProperty = property;
    }

    public void OpenPropertyAdminScreen()
    {
        OpenPropertyAdminScreen(currentProperty);
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }
}
