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
    private Image withRoomsToggleBackground = null;
    [SerializeField]
    private Image ithoutRoomsToggleBackground = null;
    [SerializeField]
    private Button saveButton = null;
    [SerializeField]
    private GameObject addRoomsField = null;

    private bool currentHasRooms = true;

    private void Awake()
    {
        propertyAdminScreenComponent.SetRoomsToggle += SetToggle;
        propertyAdminScreenComponent.GetRoomsToggle += GetCurrentToggles;
    }

    private void OnEnable()
    {
        withRoomsToggle.isOn = false;
        withoutRoomsToggle.isOn = false;
        addRoomsField.SetActive(false);
        withRoomsToggleBackground.color = Color.grey;
        ithoutRoomsToggleBackground.color = Color.grey;
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
            saveButton.interactable = true;
            withRoomsToggleBackground.color = Color.green;
            ithoutRoomsToggleBackground.color = Color.grey;
        }
        else if(withoutRoomsToggle.isOn)
        {
            currentHasRooms = false;
            saveButton.interactable = true;
            withRoomsToggleBackground.color = Color.grey;
            ithoutRoomsToggleBackground.color = Color.green;
        }
        else
        {
            saveButton.interactable = false;
        }
    }
}
