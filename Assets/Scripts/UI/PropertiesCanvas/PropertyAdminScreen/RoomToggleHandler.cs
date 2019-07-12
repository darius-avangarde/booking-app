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

    private bool currentHasRooms = true;

    private void Awake()
    {
        propertyAdminScreenComponent.SetRoomsToggle += SetToggle;
        propertyAdminScreenComponent.GetRoomsToggle += GetCurrentToggles;
    }

    private void OnEnable()
    {
        HasRoomsToggle.isOn = false;
        NoRoomsToggle.isOn = false;
        AddRoomsField.SetActive(false);
        HasRoomsToggleBackground.color = Color.grey;
        NoRoomsToggleBackground.color = Color.grey;
    }

    private void SetToggle(bool hasRooms)
    {
        if (hasRooms)
        {
            HasRoomsToggle.isOn = true;
            NoRoomsToggle.isOn = false;
            currentHasRooms = true;
            AddRoomsField.SetActive(true);
        }
        else
        {
            NoRoomsToggle.isOn = true;
            HasRoomsToggle.isOn = false;
            currentHasRooms = false;
            AddRoomsField.SetActive(false);
        }
    }

    private void GetCurrentToggles()
    {
        propertyAdminScreenComponent.CurrentProperty.HasRooms = currentHasRooms;
    }

    public void SetPropertyRooms()
    {
        if (HasRoomsToggle.isOn)
        {
            currentHasRooms = true;
            saveButton.interactable = true;
            HasRoomsToggleBackground.color = Color.green;
            NoRoomsToggleBackground.color = Color.grey;
        }
        else if(NoRoomsToggle.isOn)
        {
            currentHasRooms = false;
            saveButton.interactable = true;
            HasRoomsToggleBackground.color = Color.grey;
            NoRoomsToggleBackground.color = Color.green;
        }
        else
        {
            saveButton.interactable = false;
        }
    }
}
