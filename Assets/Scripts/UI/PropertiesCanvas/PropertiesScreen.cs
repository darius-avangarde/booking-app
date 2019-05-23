using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private Transform roomAdminScreenTransform = null;
    [SerializeField]
    private Transform roomScreenTransform = null;
    [SerializeField]
    private GameObject propertyWithRoomsPrefab = null;
    [SerializeField]
    private GameObject propertyWithoutRoomsPrefab = null;
    [SerializeField]
    private Transform addPropertyButton = null;
    [SerializeField]
    private RectTransform propertyInfoContent = null;
    private List<GameObject> propertyButtons = new List<GameObject>();
    private int index = 0;

    public void InstantiateProperties()
    {
        foreach (var propertyButton in propertyButtons)
        {
            Destroy(propertyButton);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            if (property.HasRooms)
            {
                propertyButton = Instantiate(propertyWithRoomsPrefab, propertyInfoContent);
            }
            else
            {
                propertyButton = Instantiate(propertyWithoutRoomsPrefab, propertyInfoContent);
            }
            propertyButton.GetComponent<PropertyButton>().Initialize(property, navigator, confirmationDialog, propertyInfoContent, roomAdminScreenTransform, roomScreenTransform, OpenPropertyAdminScreen, DeleteProperty);
            propertyButtons.Add(propertyButton);
            index++;
        }
        addPropertyButton.SetSiblingIndex(index);
    }

    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    public void DeleteProperty(IProperty property)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Ștergeți proprietatea?",
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                PropertyDataManager.DeleteProperty(property.ID);
                ReservationDataManager.DeleteReservationsForProperty(property.ID);
                InstantiateProperties();
            },
            CancelCallback = null
        });
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }
}
