using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyFields : MonoBehaviour
{
    [SerializeField]
    private Text proprietyName;
    [SerializeField]
    private Text propertySingleBedsRoom;
    [SerializeField]
    private Text propertyDoubleBedsRoom;
    
    public void Initialize(IProperty property)
    {
        //proprietyName.text = property.name;
    }
}
