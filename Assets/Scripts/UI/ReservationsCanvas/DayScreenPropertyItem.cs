﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class DayScreenPropertyItem : MonoBehaviour
{
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Text roomName = null;
    [SerializeField]
    private Text bedQuantity = null;

    public void Initialize(IProperty property, Action callback)
    {
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
