using UnityEngine;
using UnityEngine.UI;
using System;

// TODO: something like PropertyButton might better indicate what this component does
public class PropertyItem : MonoBehaviour
{
    [SerializeField]
    private Text propertyName = null;

    public void Initialize(IProperty property, Action callback)
    {
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
