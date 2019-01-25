using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreen : MonoBehaviour
{
    [SerializeField]
    private Text propertyAndRoomScreenTitle = null;
    
    public void UpdatePropertyFields(IProperty currentProperty)
    {
        string propertyName = currentProperty.Name ?? Constants.defaultProperyAdminScreenName;
        propertyAndRoomScreenTitle.text = propertyName;
        InstantiateReservations();
    }

    private void InstantiateReservations()
    {

    }
}
