using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomToggleHandler : MonoBehaviour
{
    [SerializeField]
    private PropertyAdminScreen propertyAdminScreenComponent;
    [SerializeField]
    private Toggle HasRoomsToggle = null;
    [SerializeField]
    private Toggle NoRoomsToggle = null;
    [SerializeField]
    private Image HasRoomsToggleBackground = null;
    [SerializeField]
    private Image NoRoomsToggleBackground = null;
    [SerializeField]
    private Button saveButton = null;
    [SerializeField]
    private GameObject AddRoomsField = null;

    private void Start()
    {
        HasRoomsToggle.isOn = false;
        NoRoomsToggle.isOn = false;
        AddRoomsField.SetActive(false);
        propertyAdminScreenComponent.SetRoomsToggle += SetToggle;
    }

    private void SetToggle(bool hasRooms)
    {
        if (hasRooms)
        {
            HasRoomsToggle.isOn = true;
            NoRoomsToggle.isOn = false;
            HasRoomsToggleBackground.color = Color.green;
            NoRoomsToggleBackground.color = Color.grey;
            AddRoomsField.SetActive(true);
        }
        else
        {
            NoRoomsToggle.isOn = true;
            HasRoomsToggle.isOn = false;
            HasRoomsToggleBackground.color = Color.grey;
            NoRoomsToggleBackground.color = Color.green;
            AddRoomsField.SetActive(false);
        }
    }

    public void SetPropertyRooms()
    {
        if (HasRoomsToggle.isOn)
        {
            propertyAdminScreenComponent.CurrentProperty.HasRooms = true;
            saveButton.interactable = true;
        }
        else if(NoRoomsToggle.isOn)
        {
            propertyAdminScreenComponent.CurrentProperty.HasRooms = false;
            saveButton.interactable = true;
        }
        else
        {
            saveButton.interactable = false;
        }
    }
}
