using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private PropertyRoomScreen propertyRoomScreen = null;
    [SerializeField]
    private ToggleDialog toggleDialog = null;
    [SerializeField]
    private InfoBox infoDialog = null;
    [SerializeField]
    private Text propertyRoomTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private GameObject roomsCreationText = null;
    [SerializeField]
    private Text nrRoomsText = null;
    [SerializeField]
    private RectTransform roomNameInputFieldTransform = null;
    [SerializeField]
    private InputField multiplePrefixField = null;
    [SerializeField]
    private InputField multipleFloorsField = null;
    [SerializeField]
    private InputField multipleNrRoomsField = null;
    [SerializeField]
    private GameObject roomInfoInputPanel = null;
    [SerializeField]
    private InputField roomPriceInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private AspectRatioFitter propertyImageAspectFitter = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private AspectRatioFitter backgroundImageAspectFitter = null;
    [SerializeField]
    private Toggle floorsToggle = null;
    [SerializeField]
    private Button deleteButton = null;
    [SerializeField]
    private Button infoButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;
    [SerializeField]
    private Text errorMessage = null;

    private ToggleDialogOptions toggleDialogOptions = new ToggleDialogOptions();
    private ConfirmationDialogOptions modalDialogOptions = new ConfirmationDialogOptions();
    private IProperty currentProperty;
    private IRoom currentRoom;
    private List<IRoom> MultipleRooms = new List<IRoom>();
    private Vector2 defaultRoomName;
    private string roomNameCache;
    private int SingleBedsNr;
    private int DoubleBedsNr;
    private bool canSave = true;
    private bool fromProperty = false;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
        deleteButton.onClick.AddListener(() => DeleteRoom());
        infoButton.onClick.AddListener(() => ShowInfo());
        defaultRoomName = roomNameInputFieldTransform.offsetMax;

        modalDialogOptions.Message = Constants.DELETE_ROOM;
        modalDialogOptions.ConfirmText = Constants.DELETE_CONFIRM;
        modalDialogOptions.CancelText = Constants.DELETE_CANCEL;
        modalDialogOptions.ConfirmCallback = () =>
        {
            currentProperty.DeleteRoom(currentRoom.ID);
            ReservationDataManager.DeleteReservationsForRoom(currentRoom.ID);
            navigator.GoBack();
            navigator.GoBack();
        };
        modalDialogOptions.CancelCallback = null;

        toggleDialogOptions.TitleMessage = Constants.ADD_MULTIPLE_ROOMS;
        toggleDialogOptions.ConfirmText = Constants.CONFIRM;
        toggleDialogOptions.CancelText = Constants.DELETE_CANCEL;
        toggleDialogOptions.SetOptions(
            new ToggleOption(Constants.REPLACE_ROOMS, SaveMultipleRooms),
            new ToggleOption(Constants.ADD_OVER_ROOMS, SaveOverCurrentRooms)
            );
    }

    /// <summary>
    /// set the current property
    /// this function is called from property screen only
    /// </summary>
    /// <param name="property">selected property</param>
    public void SetCurrentProperty(IProperty property)
    {
        fromProperty = true;
        currentProperty = property;
        currentRoom = currentProperty.AddRoom();
        Initialize();
    }

    /// <summary>
    /// set the current property and room
    /// </summary>
    /// <param name="property">selected property</param>
    public void SetCurrentPropertyRoom(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        Initialize();
    }

    /// <summary>
    /// initialize current property photo
    /// initialize input fields
    /// initialize beds information
    /// </summary>
    public void Initialize()
    {
        if (currentRoom != null)
        {
            propertyRoomTitle.text = currentProperty.Name;
            if (ImageDataManager.PropertyPhotos.ContainsKey(currentProperty.ID))
            {
                propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
                backgroundImage.sprite = (Sprite)ImageDataManager.BlurPropertyPhotos[currentProperty.ID];
                backgroundImage.gameObject.SetActive(true);
            }
            else
            {
                propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
                backgroundImage.gameObject.SetActive(false);
            }
            propertyImageAspectFitter.aspectRatio = backgroundImageAspectFitter.aspectRatio = (float)propertyImage.sprite.texture.width / propertyImage.sprite.texture.height;
            if (string.IsNullOrEmpty(currentRoom.Name))
            {
                roomNameInputField.gameObject.SetActive(false);
                roomsCreationText.SetActive(true);
                roomInfoInputPanel.SetActive(true);
                propertyRoomTitle.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(false);
                infoButton.gameObject.SetActive(true);
                floorsToggle.isOn = true;
                multipleFloorsField.text = "1";
            }
            else
            {
                roomNameInputField.gameObject.SetActive(true);
                roomsCreationText.SetActive(false);
                roomNameInputField.text = currentRoom.Name;
                roomInfoInputPanel.SetActive(false);
                propertyRoomTitle.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(true);
                infoButton.gameObject.SetActive(false);
            }
            roomPriceInputField.text = currentRoom.Price ?? Constants.PRICE;
            roomSingleBedQuantityInputField.text = currentRoom.SingleBeds.ToString();
            roomDoubleBedQuantityInputField.text = currentRoom.DoubleBeds.ToString();
            SingleBedsNr = currentRoom.SingleBeds;
            DoubleBedsNr = currentRoom.DoubleBeds;
        }
    }

    public void SaveChanges()
    {
        if (!string.IsNullOrEmpty(multipleNrRoomsField.text) && int.Parse(multipleNrRoomsField.text) > 0)
        {
            if (currentProperty.MultipleRooms.Count() > 0)
            {
                OpenToggleDialog();
            }
            else
            {
                if (string.IsNullOrEmpty(multipleNrRoomsField.text))
                {
                    errorMessage.text = Constants.ERR_ROOM_NUMBER;
                    canSave = false;
                }
                else
                {
                    ResetError();
                }
                foreach (var room in currentProperty.Rooms)
                {
                    if ($"{multiplePrefixField.text} {1}".Trim().ToLower() == room.Name.Trim().ToLower())
                    {
                        errorMessage.text = Constants.ERR_MULTIPLE_ROOMS_NAME;
                        canSave = false;
                        break;
                    }
                    else
                    {
                        ResetError();
                    }
                }
                if (canSave)
                {
                    SaveMultipleRooms();
                }
            }
        }
        else
        {
            errorMessage.text = Constants.ERR_ROOM_NULL_NUMBER;
        }
    }

    private void SaveMultipleRooms()
    {
        if (currentProperty.MultipleRooms.Count() > 0)
        {
            foreach (var room in currentProperty.MultipleRooms)
            {
                currentProperty.DeleteRoom(room.ID);
            }
        }
        int floors = 0;
        if (!string.IsNullOrEmpty(multipleFloorsField.text) && floorsToggle.isOn)
        {
            floors = int.Parse(multipleFloorsField.text);
        }
        int rooms = int.Parse(multipleNrRoomsField.text);
        ResetError();
        currentProperty.FloorRooms = rooms;
        List<IRoom> roomsList = new List<IRoom>();
        if (floors > 0)
        {
            for (int j = 1; j <= rooms; j++)
            {
                IRoom newRoom = currentProperty.AddRoom();
                newRoom.Name = $"{multiplePrefixField.text} {j}";
                newRoom.RoomNumber = j;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }
            for (int i = 1; i < floors; i++)
            {
                for (int j = 1; j <= rooms; j++)
                {
                    IRoom newRoom = currentProperty.AddRoom();
                    if (j < 10)
                    {
                        newRoom.Name = $"{multiplePrefixField.text} {i}0{j}";
                        newRoom.RoomNumber = int.Parse($"{i}0{j}");
                    }
                    else
                    {
                        newRoom.Name = $"{multiplePrefixField.text} {i}{j}";
                        newRoom.RoomNumber = int.Parse($"{i}{j}");
                    }
                    newRoom.Multiple = true;
                    roomsList.Add(newRoom);
                }
            }
        }
        else
        {
            for (int j = 1; j <= rooms; j++)
            {
                IRoom newRoom = currentProperty.AddRoom();
                newRoom.Name = $"{multiplePrefixField.text} {j}";
                newRoom.RoomNumber = j;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }
        }
        currentProperty.SaveMultipleRooms(roomsList);
        navigator.GoBack();
        if (fromProperty)
        {
            navigator.GoBack();
            OpenPropertyRoomScreen();
        }
    }

    public void SaveOverCurrentRooms()
    {
        int previousFloors = currentProperty.MultipleRooms.Count() / currentProperty.FloorRooms;
        int previousRooms = currentProperty.FloorRooms;
        int floors = 0;
        if (!string.IsNullOrEmpty(multipleFloorsField.text) && floorsToggle.isOn)
        {
            floors = int.Parse(multipleFloorsField.text);
        }
        int rooms = int.Parse(multipleNrRoomsField.text);
        currentProperty.FloorRooms = previousRooms + rooms;
        ResetError();
        List<IRoom> roomsList = new List<IRoom>();
        if (floors > 1)
        {
            for (int j = previousRooms + 1; j <= previousRooms + rooms; j++)
            {
                IRoom newRoom = currentProperty.AddRoom();
                newRoom.Name = $"{multiplePrefixField.text} {j}";
                newRoom.RoomNumber = j;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }
            for (int i = 1; i < floors; i++)
            {
                if (i < previousFloors)
                {
                    for (int j = previousRooms + 1; j <= previousRooms + rooms; j++)
                    {
                        IRoom newRoom = currentProperty.AddRoom();
                        if (j < 10)
                        {
                            newRoom.Name = $"{multiplePrefixField.text} {i}0{j}";
                            newRoom.RoomNumber = int.Parse($"{i}0{j}");
                        }
                        else
                        {
                            newRoom.Name = $"{multiplePrefixField.text} {i}{j}";
                            newRoom.RoomNumber = int.Parse($"{i}{j}");
                        }
                        newRoom.Multiple = true;
                        roomsList.Add(newRoom);
                    }
                }
                else
                {
                    for (int j = 1; j <= rooms; j++)
                    {
                        IRoom newRoom = currentProperty.AddRoom();
                        if (j < 10)
                        {
                            newRoom.Name = $"{multiplePrefixField.text} {i}0{j}";
                            newRoom.RoomNumber = int.Parse($"{i}0{j}");
                        }
                        else
                        {
                            newRoom.Name = $"{multiplePrefixField.text} {i}{j}";
                            newRoom.RoomNumber = int.Parse($"{i}{j}");
                        }
                        newRoom.Multiple = true;
                        roomsList.Add(newRoom);
                    }
                }
            }
        }
        else
        {
            for (int j = previousRooms + 1; j <= previousRooms + rooms; j++)
            {
                IRoom newRoom = currentProperty.AddRoom();
                newRoom.Name = $"{multiplePrefixField.text} {j}";
                newRoom.RoomNumber = j;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }
        }
        currentProperty.SaveMultipleRooms(roomsList);
        navigator.GoBack();
        if (fromProperty)
        {
            navigator.GoBack();
            OpenPropertyRoomScreen();
        }
    }

    public void SetFloors()
    {
        if (floorsToggle.isOn)
        {
            nrRoomsText.text = "Camere/Etaj*";
            multipleFloorsField.interactable = true;
        }
        else
        {
            nrRoomsText.text = "Camere*";
            multipleFloorsField.interactable = false;
        }
    }

    public void DeleteRoom()
    {
        modalDialogOptions.Message = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID).Count() > 0 ? Constants.DELETE_ROOM_RESERVATIONS : Constants.DELETE_ROOM;
        modalDialogOptions.ConfirmText = Constants.DELETE_CONFIRM;
        modalDialogOptions.CancelText = Constants.DELETE_CANCEL;
        modalDialogOptions.ConfirmCallback = () =>
        {
            currentProperty.DeleteRoom(currentRoom.ID);
            ReservationDataManager.DeleteReservationsForRoom(currentRoom.ID);
            navigator.GoBack();
            navigator.GoBack();
        };
        modalDialogOptions.CancelCallback = null;
        confirmationDialog.Show(modalDialogOptions);
    }

    public void DefaultValues()
    {
        roomNameInputField.text = string.Empty;
        roomPriceInputField.text = string.Empty;
        roomSingleBedQuantityInputField.text = string.Empty;
        roomDoubleBedQuantityInputField.text = string.Empty;
        errorMessage.text = string.Empty;
        multiplePrefixField.text = string.Empty;
        multipleFloorsField.text = string.Empty;
        multipleNrRoomsField.text = string.Empty;
        currentProperty = null;
        currentRoom = null;
        canSave = true;
        fromProperty = false;
    }

    public void ResetError()
    {
        errorMessage.text = string.Empty;
        canSave = true;
    }

    public void ShowInfo()
    {
        infoDialog.Show($"<b>Prefix:</b>{Environment.NewLine}Este adăugat înainte de numărul camerei{Environment.NewLine}{Environment.NewLine}<b>Etaje:</b>{Environment.NewLine}Reprezintă numărul de etaje ale proprietății, inclusiv parterul{Environment.NewLine}{Environment.NewLine}<b>Camere/Etaj:</b>{Environment.NewLine}Reprezintă numărul de camere ale unui etaj{Environment.NewLine}{Environment.NewLine}*Câmpurile marcate cu steluță sunt obligatorii.");
    }

    private void OpenToggleDialog()
    {
        toggleDialog.Show(toggleDialogOptions);
    }

    public void OnRoomNameValueChanged(string value)
    {
        currentRoom.Name = string.IsNullOrEmpty(value) ? Constants.NEW_ROOM : value;
    }

    public void OnRoomPriceValueChanged(string value)
    {
        currentRoom.Price = string.IsNullOrEmpty(value) ? "" : value;
    }

    public void OnSingleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomSingleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.SingleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void OnDoubleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomDoubleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.DoubleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void IncrementSingleBedQuantity()
    {
        roomSingleBedQuantityInputField.text = (++SingleBedsNr).ToString();
    }

    public void DecrementSingleBedQuantity()
    {
        if (roomSingleBedQuantityInputField.text != "0")
        {
            roomSingleBedQuantityInputField.text = (--SingleBedsNr).ToString();
        }
    }

    public void IncrementDoubleBedQuantity()
    {
        roomDoubleBedQuantityInputField.text = (++DoubleBedsNr).ToString();
    }

    public void DecrementDoubleBedQuantity()
    {
        if (roomDoubleBedQuantityInputField.text != "0")
        {
            roomDoubleBedQuantityInputField.text = (--DoubleBedsNr).ToString();
        }
    }

    private void OpenPropertyRoomScreen()
    {
        propertyRoomScreen.ScrollToTop();
        propertyRoomScreen.SetCurrentProperty(currentProperty);
        navigator.GoTo(propertyRoomScreen.GetComponent<NavScreen>());
    }
}
