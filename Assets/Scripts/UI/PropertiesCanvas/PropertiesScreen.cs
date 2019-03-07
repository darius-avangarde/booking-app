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
    // TODO: we don't need to call InstantiateProperties in start if we do it from NavScreen.Showing
    void Start()
    {
        InstantiateProperties();
    }

    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        // TODO: instead of updating the property fields in the property admin screen we should just set the data
        // we should minimize the amount of coupling between components (and classes in general)
        // in this case we can get away with just setting the data (property) and letting PropertyAdminScreen update itself when it needs to
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
            // TODO: we can eliminate the annonymous function here by changing PropertyItem.Initialize to take OpenPropertyAdminScreen as the second parameter
            // it already has a reference to the property
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
