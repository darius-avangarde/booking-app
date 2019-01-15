using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UINavigation;

public class PropertyFields : MonoBehaviour
{
    [SerializeField]
    private Text propertyName;
    
    public void Initialize(IProperty property)
    {
        propertyName.text = "property.Name";
        GetComponent<Button>().onClick.AddListener(() => OpenPropertyAdminScreen(property));
    }
    private void OpenPropertyAdminScreen(IProperty property)
    {
        PropertyAdminScreen.currentProperty = property;
    }
}
