using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;

public class PropertyButton : MonoBehaviour
{
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Button propertyButtonItem = null;

    private IProperty currentProperty;

    /// <summary>
    /// set the current property
    /// set property name
    /// set button callback for property with or without rooms
    /// </summary>
    /// <param name="property">selected property</param>
    /// <param name="PropertyRoomCallback">click callback for properties without rooms</param>
    /// <param name="PropertyCallback">callback for properties with rooms</param>
    public void Initialize(IProperty property, Action<IProperty> PropertyCallback, Action<Graphic> setTheme)
    {
        currentProperty = property;
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : $"Proprietatea{Environment.NewLine}{property.Name}";
        propertyButtonItem.onClick.AddListener(() => PropertyCallback(property));

        setTheme(propertyButtonItem.GetComponent<Graphic>());
        setTheme(propertyName.GetComponent<Graphic>());
    }
}