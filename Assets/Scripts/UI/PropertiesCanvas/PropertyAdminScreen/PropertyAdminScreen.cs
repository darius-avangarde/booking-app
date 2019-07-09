using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    public event Action<bool> MultipleRooms = delegate { };
    public event Action<bool> SetRoomsToggle = delegate { };
    public event Action CheckSave = delegate { };
    public IProperty CurrentProperty { get; set; }
    public bool CanSave { get; set; } = true;

    public Navigator navigator = null;

    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ToggleDialog toggleDialog = null;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private GameObject RoomsToggleField = null;
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
    private bool shouldGoBack = true;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        deleteButton.onClick.AddListener(() => DeleteProperty());
        calcelButton.onClick.AddListener(() => navigator.GoBack());

        toggleDialogOptions.TitleMessage = Constants.ADD_MULTIPLE_ROOMS;
        toggleDialogOptions.ConfirmText = Constants.CONFIRM;
        toggleDialogOptions.CancelText = Constants.DELETE_CANCEL;
        toggleDialogOptions.SetOptions(
            new ToggleOption(Constants.REPLACE_ROOMS, () => MultipleRooms(true)),
            new ToggleOption(Constants.ADD_OVER_ROOMS, () => MultipleRooms(false))
            );
    }

    /// <summary>
    /// set the current property
    /// </summary>
    /// <param name="property">selected property</param>
    public void SetCurrentProperty(IProperty property)
    {
        CurrentProperty = property;
        if (CurrentProperty.Name != null)
        {
            RoomsToggleField.SetActive(false);
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            RoomsToggleField.SetActive(true);
            deleteButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// update screen details with the current property
    /// update name, property photo and rooms toggle
    /// </summary>
    private void SetPropertyFieldsText()
    {
        propertyNameInputField.text = CurrentProperty.Name ?? "";
        if (string.IsNullOrEmpty(CurrentProperty.Name))
        {
        }
        else
        {
            SetRoomsToggle(CurrentProperty.HasRooms);
        }
        ImageDataManager.AddedPhoto = false;
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
            if (CurrentProperty.HasRooms)
            {
                if (CurrentProperty.Rooms.Count() > 0)
                {
                    shouldGoBack = false;
                    toggleDialog.Show(toggleDialogOptions);
                }
                else
                {
                    MultipleRooms(true);
                }
            }
            else
            {
                PropertyDataManager.CreatePropertyRoom(CurrentProperty);
                CurrentProperty.GetPropertyRoom().Name = CurrentProperty.Name;
            }
            if (PropertyDataManager.GetProperty(CurrentProperty.ID) == null)
            {
                PropertyDataManager.SaveProperty(CurrentProperty);
            }
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
            Message = Constants.DELETE_PROPERTY,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                PropertyDataManager.DeleteProperty(CurrentProperty.ID);
                ReservationDataManager.DeleteReservationsForProperty(CurrentProperty.ID);
                ImageDataManager.DeletePropertyPhoto(CurrentProperty.ID);
                navigator.GoBack();
                navigator.GoBack();
            },
            CancelCallback = null
        });
    }
}
