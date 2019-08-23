using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomToggleHandler : MonoBehaviour
{
    [SerializeField]
    private PropertyAdminScreen propertyAdminScreenComponent;
    [SerializeField]
    private Toggle withRoomsToggle = null;
    [SerializeField]
    private Toggle withoutRoomsToggle = null;
    [SerializeField]
    private Button saveButton = null;
    [SerializeField]
    private GameObject addRoomsField = null;

    private bool currentHasRooms = true;

    private void OnEnable()
    {
        withRoomsToggle.isOn = false;
        withoutRoomsToggle.isOn = false;
        addRoomsField.SetActive(false);
    }

    private void SetToggle(bool hasRooms)
    {
        if (hasRooms)
        {
            withRoomsToggle.isOn = true;
            withoutRoomsToggle.isOn = false;
            currentHasRooms = true;
            addRoomsField.SetActive(true);
        }
        else
        {
            withoutRoomsToggle.isOn = true;
            withRoomsToggle.isOn = false;
            currentHasRooms = false;
            addRoomsField.SetActive(false);
        }
    }

    private void GetCurrentToggles()
    {
        propertyAdminScreenComponent.CurrentProperty.HasRooms = currentHasRooms;
    }

    public void SetPropertyRooms()
    {
        if (withRoomsToggle.isOn)
        {
            currentHasRooms = true;
            //saveButton.interactable = true;
        }
        else if(withoutRoomsToggle.isOn)
        {
            currentHasRooms = false;
            //saveButton.interactable = true;
        }
        else
        {
            //saveButton.interactable = false;
        }
    }
}
