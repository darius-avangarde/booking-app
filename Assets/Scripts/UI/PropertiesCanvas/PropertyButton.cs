using UnityEngine;
using UnityEngine.UI;
using System;

public class PropertyButton : MonoBehaviour
{
    [SerializeField]
    private Text propertyName = null;

    public void Initialize(IProperty property, Action<IProperty> callback)
    {
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        GetComponent<Button>().onClick.AddListener(() => callback(property));
    }
}
