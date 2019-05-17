using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private Transform roomAdminScreenTransform = null;
    [SerializeField]
    private GameObject propertyPrefabButton = null;
    [SerializeField]
    private Transform addPropertyButton = null;
    [SerializeField]
    private Transform propertyInfoContent = null;
    private List<GameObject> propertyButtons = new List<GameObject>();
    private int index = 0;

    public void InstantiateProperties()
    {
        foreach (var propertyButton in propertyButtons)
        {
            Destroy(propertyButton);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton = Instantiate(propertyPrefabButton, propertyInfoContent);
            propertyButton.GetComponent<PropertyButton>().layoutContent = propertyInfoContent.GetComponent<RectTransform>();
            propertyButton.GetComponent<PropertyButton>().Initialize(property, OpenPropertyAdminScreen);
            propertyButton.GetComponent<PropertyButton>().navigator = navigator;
            propertyButton.GetComponent<PropertyButton>().roomAdminScreenTransform = roomAdminScreenTransform;
            propertyButtons.Add(propertyButton);
            index++;
        }
        addPropertyButton.SetSiblingIndex(index);
    }

    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }
}
