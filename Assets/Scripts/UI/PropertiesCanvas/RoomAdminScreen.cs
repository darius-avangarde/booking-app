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
    private ToggleDialog toggleDialog = null;
    [SerializeField]
    private Text propertyRoomTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private RectTransform roomNameInputFieldTransform = null;
    [SerializeField]
    private RectTransform multipleRoomsOptionsPanel = null;
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
    private Toggle singleRoomToggle = null;
    [SerializeField]
    private Toggle multipleRoomsToggle = null;
    [SerializeField]
    private Button deleteButton = null;
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

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
        deleteButton.onClick.AddListener(() => DeleteRoom());
        defaultRoomName = roomNameInputFieldTransform.offsetMax;

        toggleDialogOptions.TitleMessage = Constants.ADD_MULTIPLE_ROOMS;
        toggleDialogOptions.ConfirmText = Constants.CONFIRM;
        toggleDialogOptions.CancelText = Constants.DELETE_CANCEL;
        toggleDialogOptions.SetOptions(
            new ToggleOption(Constants.REPLACE_ROOMS, SaveMultipleRooms),
            new ToggleOption(Constants.ADD_OVER_ROOMS, SaveOverCurrentRooms)
            );

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
    }

    public void SetCurrentPropertyRoom(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        Initialize();
    }

    public void Initialize()
    {
        if (currentRoom != null)
        {
            propertyRoomTitle.text = currentProperty.Name;
            roomNameCache = currentRoom.Name ?? string.Empty;
            roomNameInputField.text = roomNameCache;
            singleRoomToggle.isOn = true;
            multipleRoomsOptionsPanel.sizeDelta = new Vector2(multipleRoomsOptionsPanel.sizeDelta.x, 0);
            if (ImageDataManager.PropertyPhotos.ContainsKey(currentProperty.ID))
            {
                propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
                backgroundImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
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
                roomInfoInputPanel.SetActive(true);
                propertyRoomTitle.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(false);
            }
            else
            {
                roomInfoInputPanel.SetActive(false);
                propertyRoomTitle.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(true);
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
        if (singleRoomToggle.isOn)
        {
            if (!string.IsNullOrEmpty(roomNameInputField.text))
            {
                foreach (var room in currentProperty.Rooms)
                {
                    if (roomNameInputField.text.Trim().ToLower() == room.Name.Trim().ToLower())
                    {
                        errorMessage.text = Constants.ERR_ROOM_TAKEN;
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
                    SaveRoom();
                }
            }
            else
            {
                errorMessage.text = Constants.ERR_ROOM_NAME;
            }
        }
        else if (multipleRoomsToggle.isOn)
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
    }

    private void SaveRoom()
    {
        OnRoomNameValueChanged(roomNameInputField.text);
        OnRoomPriceValueChanged(roomPriceInputField.text);
        OnSingleBedsChanged(roomSingleBedQuantityInputField.text);
        OnDoubleBedsChanged(roomDoubleBedQuantityInputField.text);
        if (currentProperty.GetRoom(currentRoom.ID) == null)
        {
            currentProperty.SaveRoom(currentRoom);
        }
        navigator.GoBack();
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
        if (!string.IsNullOrEmpty(multipleFloorsField.text))
        {
            floors = int.Parse(multipleFloorsField.text);
        }
        int rooms = int.Parse(multipleNrRoomsField.text);
        ResetError();
        currentProperty.FloorRooms = rooms;
        if (floors > 0)
        {
            for (int j = 1; j <= rooms; j++)
            {
                IRoom newRoom = currentProperty.AddRoom();
                newRoom.Name = $"{multiplePrefixField.text} {j}";
                newRoom.RoomNumber = j;
                newRoom.Multiple = true;
                currentProperty.SaveRoom(newRoom);
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
                    currentProperty.SaveRoom(newRoom);
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
                currentProperty.SaveRoom(newRoom);
            }
        }
        navigator.GoBack();
    }

    public void SaveOverCurrentRooms()
    {
        int previousFloors = currentProperty.MultipleRooms.Count() / currentProperty.FloorRooms;
        int previousRooms = currentProperty.FloorRooms;
        int floors = 0;
        if (!string.IsNullOrEmpty(multipleFloorsField.text))
        {
            floors = int.Parse(multipleFloorsField.text);
        }
        int rooms = int.Parse(multipleNrRoomsField.text);
        currentProperty.FloorRooms = previousRooms + rooms;
        ResetError();
        if (floors > 0)
        {
            int maxFloor = Mathf.Max(previousFloors, floors);
            for (int j = previousRooms + 1; j <= previousRooms + rooms; j++)
            {
                IRoom newRoom = currentProperty.AddRoom();
                newRoom.Name = $"{multiplePrefixField.text} {j}";
                newRoom.RoomNumber = j;
                newRoom.Multiple = true;
                currentProperty.SaveRoom(newRoom);
            }
            for (int i = 1; i < maxFloor; i++)
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
                        currentProperty.SaveRoom(newRoom);
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
                        currentProperty.SaveRoom(newRoom);
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
                currentProperty.SaveRoom(newRoom);
            }
        }
        navigator.GoBack();
    }

    public void SelectSingleRoom()
    {
        if (singleRoomToggle.isOn)
        {
            ResetError();
            StopAllCoroutines();
            roomNameInputField.interactable = true;
            roomNameInputField.text = roomNameCache;
            StartCoroutine(HideNameInput(defaultRoomName));
            StartCoroutine(ExpandMultipleOptions(false, multipleRoomsOptionsPanel.offsetMax));
        }
    }

    public void SelectMultipleRooms()
    {
        if (multipleRoomsToggle.isOn)
        {
            ResetError();
            StopAllCoroutines();
            roomNameCache = roomNameInputField.text;
            roomNameInputField.text = Constants.MULTIPLE_ROOMS;
            roomNameInputField.interactable = false;
            StartCoroutine(HideNameInput(new Vector2(defaultRoomName.x + 150f, defaultRoomName.y)));
            StartCoroutine(ExpandMultipleOptions(true, new Vector2(multipleRoomsOptionsPanel.sizeDelta.x, 532)));
        }
    }

    public void DeleteRoom()
    {
        modalDialogOptions.Message = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID).Count() > 0 ? Constants.DELETE_ROOM_RESERVATIONS : Constants.DELETE_ROOM;
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
    }

    public void ResetError()
    {
        errorMessage.text = string.Empty;
        canSave = true;
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

    private IEnumerator ExpandMultipleOptions(bool active, Vector2 endSize)
    {
        if (active)
        {
            multipleRoomsOptionsPanel.gameObject.SetActive(true);
        }
        float currentTime = 0;
        while (currentTime < 0.6f)
        {
            currentTime += Time.deltaTime;
            multipleRoomsOptionsPanel.sizeDelta = Vector2.Lerp(multipleRoomsOptionsPanel.sizeDelta, endSize, currentTime / 0.6f);
            yield return null;
        }
        multipleRoomsOptionsPanel.sizeDelta = endSize;
        if (!active)
        {
            multipleRoomsOptionsPanel.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideNameInput(Vector2 endSize)
    {
        float currentTime = 0;
        while (currentTime < 0.4f)
        {
            currentTime += Time.deltaTime;
            roomNameInputFieldTransform.offsetMax = Vector2.Lerp(roomNameInputFieldTransform.offsetMax, endSize, currentTime / 0.4f);
            yield return null;
        }
        roomNameInputFieldTransform.offsetMax = endSize;
    }
}
