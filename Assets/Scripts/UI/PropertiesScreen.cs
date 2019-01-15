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
        IProperty property = PropertyDataManager.AddProperty();
    }

    private void InstantiateProperties()
    {
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton = Instantiate(propertyPrefabButton, propertyInfoContent);
            propertyButton.GetComponent<PropertyFields>().Initialize(property);
        }
    }
}
