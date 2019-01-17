using UnityEngine;
using UnityEngine.UI;
using System;

public class PropertyItem : MonoBehaviour
{
    [SerializeField]
    private Text propertyName;
    
    public void Initialize(IProperty property, Action callback)
    {
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
