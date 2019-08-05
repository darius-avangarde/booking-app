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
    public event Action<bool> MultipleRooms = delegate { };
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

    private void OnEnable()
    {
        SetPropertyFieldsText();
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
            //RoomsToggleField.SetActive(false);
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            //RoomsToggleField.SetActive(true);
            deleteButton.gameObject.SetActive(false);
        }
        SetPropertyFieldsText();
        setPropertyTypeDropdown.CurrentPropertyType = CurrentProperty.PropertyType;
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
            propertyScreenTitle.text = string.IsNullOrEmpty(CurrentProperty.Name) ? Constants.NEW_PROPERTY : Constants.EDIT_PROPERTY;
            if (!string.IsNullOrEmpty(CurrentProperty.Name))
            {
                //SetRoomsToggle(CurrentProperty.HasRooms);
                setPropertyTypeDropdown.CurrentPropertyType = CurrentProperty.PropertyType;
                if (CurrentProperty.Floors >= 0)
                {
                    SetMultipleRoomsFields?.Invoke(CurrentProperty.Floors, CurrentProperty.FloorRooms);
                }
            }
        }
    }

    private void SetPropertyType(PropertyDataManager.PropertyType propertyType)
    {
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
            CurrentProperty.PropertyType = setPropertyTypeDropdown.CurrentPropertyType;
            CurrentProperty.HasRooms = withRooms;
            if (CurrentProperty.HasRooms)
            {
                //if (CurrentProperty.Rooms.Count() > 0)
                //{
                //    shouldGoBack = false;
                //    toggleDialog.Show(toggleDialogOptions);
                //}
                //else
                //{
                    MultipleRooms(false);
                //}
            }
            else
            {
                if (CurrentProperty.GetPropertyRoom() == null)
                {
                    PropertyDataManager.CreatePropertyRoom(CurrentProperty);
                    CurrentProperty.GetPropertyRoom().Name = CurrentProperty.Name;
                }
            }
            if (PropertyDataManager.GetProperty(CurrentProperty.ID) == null)
            {
                PropertyDataManager.SaveProperty(CurrentProperty);
                propertyDropdownHandler.UpdateDropdown();
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
            Message = Constants.DELETE_PROPERTY,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
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
