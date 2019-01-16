using UnityEngine;
using UnityEngine.UI;
using System;

public class PropertyFields : MonoBehaviour
{
    [SerializeField]
    private Text propertyName;
    
    public void Initialize(IProperty property, Action callback)
    {
        propertyName.text = property.Name;
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
