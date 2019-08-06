using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    public event Action<int, int[]> SetMultipleRoomsFields = delegate { };
    public event Action MultipleRooms = delegate { };
    //public event Action<bool> SetRoomsToggle = delegate { };
    //public event Action GetRoomsToggle = delegate { };
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
    private SetPropertyTypeDropdown setPropertyTypeDropdown = null;
    [SerializeField]
    private NavScreen propertyAdminScreen = null;
    [SerializeField]
    private Text propertyScreenTitle = null;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private GameObject roomsToggleField = null;
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

        //toggleDialogOptions.TitleMessage = Constants.ADD_MULTIPLE_ROOMS;
        //toggleDialogOptions.ConfirmText = Constants.CONFIRM;
        //toggleDialogOptions.CancelText = Constants.DELETE_CANCEL;
        //toggleDialogOptions.SetOptions(
        //    new ToggleOption(Constants.REPLACE_ROOMS, () => MultipleRooms(true, true)),
        //    new ToggleOption(Constants.ADD_OVER_ROOMS, () => MultipleRooms(false, true))
        //    );
    }

    /// <summary>
    /// set the current property
    /// </summary>
    /// <param name="property">selected property</param>
    public void OpenPropertyAdminScreen(IProperty property)
    {
        CurrentProperty = property;
        if (CurrentProperty.Name != null)
        {
            deleteButton.gameObject.SetActive(true);
            multipleRoomsField.SetActive(CurrentProperty.HasRooms);
            if (CurrentProperty.HasRooms)
            {
                //roomsToggleField.SetActive(true);
                setPropertyTypeDropdown.SetDropdownOptions(0, 1);
            }
            else
            {
                //roomsToggleField.SetActive(false);
                setPropertyTypeDropdown.SetDropdownOptions(2, 3);
            }
        }
        else
        {
            deleteButton.gameObject.SetActive(false);
            setPropertyTypeDropdown.SetDropdownOptions(0, 3);
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
            propertyNameInputField.text = CurrentProperty.Name ?? "";
            propertyScreenTitle.text = string.IsNullOrEmpty(CurrentProperty.Name) ? LocalizedText.Instance.PropertyHeader[0] : LocalizedText.Instance.PropertyHeader[1];
            if (!string.IsNullOrEmpty(CurrentProperty.Name))
            {
                //SetRoomsToggle(CurrentProperty.HasRooms);
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
                }
            }
            else
            {
                setPropertyTypeDropdown.CurrentPropertyType = 0;
                setPropertyTypeDropdown.SetPropertyType(0);
                SetMultipleRoomsFields?.Invoke(0, new int[1]);
            }
        }
    }

    private void SetPropertyType(PropertyDataManager.PropertyType propertyType)
    {
        if (!CurrentProperty.HasRooms)
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
                break;
        }
        multipleRoomsField.SetActive(withRooms);
    }

    /// <summary>
    /// save information for current property
    /// </summary>
    public void SaveProperty()
    {
        CheckSave();
        if (CanSave)
        {
            CurrentProperty.Name = propertyNameInputField.text;
            CurrentProperty.HasRooms = withRooms;
            if (CurrentProperty.HasRooms)
            {
                shouldGoBack = false;
                CurrentProperty.PropertyType = setPropertyTypeDropdown.CurrentPropertyType;
                if (PropertyDataManager.GetProperty(CurrentProperty.ID) == null)
                {
                    PropertyDataManager.SaveProperty(CurrentProperty);
                    propertyDropdownHandler.UpdateDropdown();
                }
                MultipleRooms();
            }
            else
            {
                shouldGoBack = true;
                if (CurrentProperty.GetPropertyRoom() == null)
                {
                    CurrentProperty.PropertyType = setPropertyTypeDropdown.CurrentPropertyType;
                    PropertyDataManager.CreatePropertyRoom(CurrentProperty);
                    CurrentProperty.GetPropertyRoom().Name = CurrentProperty.Name;
                }
                else
                {
                    CurrentProperty.PropertyType = setPropertyTypeDropdown.CurrentPropertyType + 2;
                }
                if (PropertyDataManager.GetProperty(CurrentProperty.ID) == null)
                {
                    PropertyDataManager.SaveProperty(CurrentProperty);
                    propertyDropdownHandler.UpdateDropdown();
                }
            }
            if (shouldGoBack)
            {
                navigator.GoBack();
            }
            reservationCallendar.SetPropertyToBeLoadedOnEnable(CurrentProperty);
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
                navigator.GoBack();
                if (hasRooms)
                {
                    navigator.GoBack();
                }
                reservationCallendar.SetPropertyToBeLoadedOnEnable(null);
            },
            CancelCallback = null
        });
    }
}
