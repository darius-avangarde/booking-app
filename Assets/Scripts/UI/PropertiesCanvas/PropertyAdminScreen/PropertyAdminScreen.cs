using System;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    public event Action<int, int[]> SetMultipleRoomsFields = delegate { };
    public event Action MultipleRooms = delegate { };
    public event Action CheckSave = delegate { };
    public IProperty CurrentProperty { get; set; }
    public bool CanSave { get; set; } = true;
    public Navigator navigator;

    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ToggleDialog toggleDialog = null;
    [SerializeField]
    private ReservationsCalendarManager reservationCallendar = null;
    [SerializeField]
    private PropertyDropdownHandler propertyDropdownHandler = null;
    [SerializeField]
    private NavScreen propertiesScreen = null;
    [SerializeField]
    private NavScreen propertyAdminScreen = null;
    [SerializeField]
    private SetPropertyTypeDropdown setPropertyTypeDropdown = null;
    [SerializeField]
    private SetBedsNumber setBedsNumber = null;
    [SerializeField]
    private Text propertyScreenTitle = null;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private GameObject multipleRoomsField = null;
    [SerializeField]
    private Button saveButton = null;
    [SerializeField]
    private Button deleteButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;
    [SerializeField]
    private Text errorMessage = null;
    [SerializeField]
    private RectTransform multipleRoomFieldsRect;

    private Dictionary<string, Dropdown.OptionData> FloorsOptions;
    private Dictionary<int, Dropdown.OptionData> roomsOptions;
    private ToggleDialogOptions toggleDialogOptions = new ToggleDialogOptions();
    private bool withRooms = true;
    private bool shouldGoBack = true;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        deleteButton.onClick.AddListener(() => DeleteProperty());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
        setPropertyTypeDropdown.ChangePropertyType += SetPropertyType;
    }

    /// <summary>
    /// set the current property
    /// </summary>
    /// <param name="property">selected property</param>
    public void OpenPropertyAdminScreen(IProperty property)
    {
        CurrentProperty = property;
        if (CurrentProperty != null)
        {
            deleteButton.gameObject.SetActive(true);
            if (CurrentProperty.HasRooms)
            {
                setPropertyTypeDropdown.SetDropdownOptions(0, 1);
            }
            else
            {
                setPropertyTypeDropdown.SetDropdownOptions(2, 3);
            }
            setPropertyTypeDropdown.SetPropertyType((int)property.PropertyType);
        }
        else
        {
            deleteButton.gameObject.SetActive(false);
            setPropertyTypeDropdown.SetDropdownOptions(0, 3);
            setPropertyTypeDropdown.SetPropertyType(0);
        }

        SetPropertyFieldsText();
        navigator.GoTo(propertyAdminScreen);
    }

    /// <summary>
    /// update screen details with the current property
    /// update name, multiple rooms field and rooms toggle
    /// </summary>
    private void SetPropertyFieldsText()
    {
        if (CurrentProperty != null)
        {
            propertyNameInputField.text = CurrentProperty.Name;
            propertyScreenTitle.text = string.IsNullOrEmpty(CurrentProperty.Name) ? LocalizedText.Instance.PropertyHeader[0] : LocalizedText.Instance.PropertyHeader[1];

            if (CurrentProperty.HasRooms)
            {
                setPropertyTypeDropdown.CurrentPropertyType = CurrentProperty.PropertyType;
                if (CurrentProperty.Floors >= 0)
                {
                    SetMultipleRoomsFields?.Invoke(CurrentProperty.Floors, CurrentProperty.FloorRooms);
                }
            }
            else
            {
                setPropertyTypeDropdown.CurrentPropertyType = CurrentProperty.PropertyType - 2;
                setBedsNumber.SetCurrentBeds(new Vector2Int(CurrentProperty.GetPropertyRoom().SingleBeds, CurrentProperty.GetPropertyRoom().DoubleBeds));
            }
            setBedsNumber.gameObject.SetActive(!CurrentProperty.HasRooms);
        }
        else
        {
            setBedsNumber.SetCurrentBeds(Vector2Int.zero);
            propertyNameInputField.text = "";
            setPropertyTypeDropdown.CurrentPropertyType = 0;
            setPropertyTypeDropdown.SetPropertyType(0);
            SetMultipleRoomsFields?.Invoke(0, new int[1]);
        }
    }

    private void SetPropertyType(PropertyDataManager.PropertyType propertyType)
    {
        if (CurrentProperty != null && !CurrentProperty.HasRooms)
        {
            propertyType += 2;
        }

        switch (propertyType)
        {
            case PropertyDataManager.PropertyType.hotel:
                withRooms = true;
                break;
            case PropertyDataManager.PropertyType.guesthouse:
                withRooms = true;
                break;
            case PropertyDataManager.PropertyType.villa:
                withRooms = false;
                break;
            case PropertyDataManager.PropertyType.cabin:
                withRooms = false;
                break;
            default:
                withRooms = false;
                break;
        }
        multipleRoomsField.SetActive(withRooms);
        setBedsNumber.gameObject.SetActive(!withRooms);
        LayoutRebuilder.ForceRebuildLayoutImmediate(multipleRoomFieldsRect);
    }

    /// <summary>
    /// save information for current property
    /// </summary>
    public void SaveProperty()
    {
        CheckSave();
        if (CanSave)
        {
            if (withRooms)
            {
                shouldGoBack = false;
                if (CurrentProperty == null)
                {
                    CurrentProperty = PropertyDataManager.AddProperty(propertyNameInputField.text, withRooms, setPropertyTypeDropdown.CurrentPropertyType);
                }
                else
                {
                    CurrentProperty.Name = propertyNameInputField.text;
                    CurrentProperty.HasRooms = withRooms;
                    CurrentProperty.PropertyType = setPropertyTypeDropdown.CurrentPropertyType;
                }
                MultipleRooms();
            }
            else
            {
                shouldGoBack = true;
                if (CurrentProperty == null)
                {
                    CurrentProperty = PropertyDataManager.AddProperty(propertyNameInputField.text, withRooms, setPropertyTypeDropdown.CurrentPropertyType);
                }
                else
                {
                    CurrentProperty.Name = propertyNameInputField.text;
                    CurrentProperty.HasRooms = withRooms;
                }
                if (CurrentProperty.GetPropertyRoom() == null)
                {
                    PropertyDataManager.CreatePropertyRoom(CurrentProperty);
                    CurrentProperty.GetPropertyRoom().Name = CurrentProperty.Name;
                }
                else
                {

                    CurrentProperty.PropertyType = setPropertyTypeDropdown.CurrentPropertyType + 2;

                }

                ///
                Vector2Int beds = setBedsNumber.GetCurrentBeds();
                CurrentProperty.GetPropertyRoom().SingleBeds = beds.x;
                CurrentProperty.GetPropertyRoom().DoubleBeds = beds.y;
                ///

                PropertyDataManager.SavePropertyData();
            }

            propertyDropdownHandler.UpdateDropdown();
            reservationCallendar.SetPropertyToBeLoadedOnEnable(CurrentProperty);
            if (shouldGoBack)
            {
                navigator.GoBack();
            }
        }
    }

    /// <summary>
    /// delete current property
    /// open modal dialog canvas
    /// </summary>
    private void DeleteProperty()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = LocalizedText.Instance.ConfirmDelete[1],
            ConfirmText = LocalizedText.Instance.ConfirmAction[0],
            CancelText = LocalizedText.Instance.ConfirmAction[1],
            ConfirmCallback = () =>
            {
                bool hasRooms = CurrentProperty.HasRooms;
                PropertyDataManager.DeleteProperty(CurrentProperty.ID);
                ReservationDataManager.DeleteReservationsForProperty(CurrentProperty.ID);
                propertyDropdownHandler.UpdateDropdown();
                reservationCallendar.SetPropertyToBeLoadedOnEnable(null);
                if (hasRooms)
                {
                    navigator.GoBackTo(propertiesScreen);
                }
                else
                {
                    navigator.GoBack();
                }
            },
            CancelCallback = null
        });
    }
}
