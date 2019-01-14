using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesScreen : MonoBehaviour
{
    [SerializeField]
    public GameObject propertyPrefabButton;
    [SerializeField]
    public Transform propertyInfoContent; 
    // Start is called before the first frame update
    void Start()
    {
        InstantiateProperties();
    }

    public void AddPropertyItem()
    {
        PropertyData data = PropertyDataManager.GetPropertyData();
        Property propertyItem = new Property();
        data.properties.Add(propertyItem);
        PropertyDataManager.SetPropertyData(data);
    }

    private void InstantiateProperties()
    {
        //if (PropertyDataManager.GetPropertyData().properties != null)
        {
            foreach (var property in PropertyDataManager.GetPropertyData().properties)
            {
                GameObject propertyButton = Instantiate(propertyPrefabButton, propertyInfoContent);
                propertyButton.GetComponent<PropertyFields>().Initialize(property);
            }
        }
    }
}
