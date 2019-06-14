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
    private Image backgroundImage = null;
    [SerializeField]
    private Image propertyImage = null;
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
            foreach (var room in currentProperty.Rooms)
            {
                if (roomNameInputField.text.Trim().ToLower() == room.Name.Trim().ToLower())
                {
                    errorMessage.text = "Exista deja o camera cu acest nume";
                    canSave = false;
                    break;
                }
                else
                {
                    errorMessage.text = string.Empty;
                    canSave = true;
                }
            }
            if (canSave)
            {
                SaveRoom();
            }
        }
        else if (multipleRoomsToggle.isOn)
        {
            if (string.IsNullOrEmpty(multipleNrRoomsField.text))
            {
                errorMessage.text = "Trebuie sa adaugati un numar de camere";
                canSave = false;
            }
            else
            {
                errorMessage.text = string.Empty;
                canSave = true;
            }
            foreach (var room in currentProperty.Rooms)
            {
                if ($"{multiplePrefixField.text} {1}".Trim().ToLower() == room.Name.Trim().ToLower())
                {
                    errorMessage.text = "Exista deja camere cu acest nume, schimbati sau adaugati un prefix";
                    canSave = false;
                    break;
                }
                else
                {
                    errorMessage.text = string.Empty;
                    canSave = true;
                }
            }
            if (canSave)
            {
                SaveMultipleRooms();
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
        int floors = 0;
        if (!string.IsNullOrEmpty(multipleFloorsField.text))
        {
            floors = int.Parse(multipleFloorsField.text);
        }
        int rooms = int.Parse(multipleNrRoomsField.text);
        if (rooms > 0)
        {
            errorMessage.text = string.Empty;
            if (floors > 0)
            {
                for (int j = 1; j <= rooms; j++)
                {
                    IRoom newRoom = currentProperty.AddRoom();
                    newRoom.Name = $"{multiplePrefixField.text} {j}";
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
                        }
                        else
                        {
                            newRoom.Name = $"{multiplePrefixField.text} {i}{j}";
                        }
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
                    currentProperty.SaveRoom(newRoom);
                }
            }
            navigator.GoBack();
        }
        else
        {
            errorMessage.text = "Numarul de camere nu poate sa fie 0";
        }
    }

    public void SelectSingleRoom()
    {
        if (singleRoomToggle.isOn)
        {
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
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID).Count() > 0 ? Constants.DELETE_ROOM_RESERVATIONS : Constants.DELETE_ROOM,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                currentProperty.DeleteRoom(currentRoom.ID);
                ReservationDataManager.DeleteReservationsForRoom(currentRoom.ID);
                navigator.GoBack();
                navigator.GoBack();
            },
            CancelCallback = null
        });
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
