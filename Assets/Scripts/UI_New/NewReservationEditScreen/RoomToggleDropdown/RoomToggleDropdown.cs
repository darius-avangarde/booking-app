using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomToggleDropdown : MonoBehaviour, IClosable
{
    [SerializeField]
    private DropDownAnimation dropdownAnimator;
    [SerializeField]
    private ScrollRect slectablesScrollrect;

    [SerializeField]
    private Text selectedText;
    [SerializeField]
    private RectTransform contentRect;
    [SerializeField]
    private GameObject roomDropdownParent;
    [SerializeField]
    private GameObject selectionButtonParent;
    [SerializeField]
    private GameObject selectabletRoomPrefab;
    [SerializeField]
    private GameObject closeButton;

    private List<IRoom> roomList;
    private UnityAction<List<IRoom>> callback;

    private bool isSelecting = false;

    private List<RoomToggleObject> pool = new List<RoomToggleObject>();
    private List<IRoom> currentSelection = new List<IRoom>();

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            CreateNewRoomObject();
            pool[i].DisableObject();
        }
    }

    public void InitializeRoomDropdown(UnityAction<List<IRoom>> selectionCallback, List<IRoom> allRooms, List<IRoom> selectedRooms = null)
    {
        callback = selectionCallback;
        roomList = allRooms;
        if(selectedRooms != null && selectedRooms.Count > 0)
        {
            currentSelection = selectedRooms;
            isSelecting = selectedRooms.Count > 1;
            selectedText.text = selectedRooms.Count > 1 ? $"{selectedRooms.Count} camere alese." : selectedRooms[0].Name;

        }
        else
        {
            selectedText.text = Constants.CHOOSE;
            isSelecting = false;
            currentSelection.Clear();
        }

        UpdateRoomObjects();
    }

    public void OpenRoomDropdown()
    {
        InputManager.CurrentlyOpenClosable = this;
        closeButton.SetActive(true);
        dropdownAnimator.MaxNumberOfItems = Mathf.Min(7, roomList.Count);
        roomDropdownParent.SetActive(true);
        selectionButtonParent.SetActive(currentSelection.Count > 0);
        UpdateRoomObjects();
    }

    public void Close()
    {
        InputManager.CurrentlyOpenClosable = null;
        closeButton.SetActive(false);
        roomDropdownParent.SetActive(false);
    }

    public void AcceptSelection()
    {
        currentSelection.Clear();
        foreach (RoomToggleObject rto in pool)
        {
            if(rto.IsOn)
                currentSelection.Add(rto.ObjectRoom);
        }

        selectedText.text = currentSelection.Count > 1 ? $"{currentSelection.Count} camere alese." : currentSelection[0].Name;
        callback?.Invoke(currentSelection);
        Close();
    }

    private void UpdateRoomObjects()
    {
        while(pool.Count < roomList.Count)
        {
            CreateNewRoomObject();
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if(i >= roomList.Count)
                pool[i].DisableObject();

            bool toggleOn = (currentSelection.Exists(r => r.ID == roomList[i].ID));

            pool[i].UpdateObject(OnObjectTap, OnObjectToggle, roomList[i], toggleOn);
        }
    }

    private void CreateNewRoomObject()
    {
        pool.Add(Instantiate(selectabletRoomPrefab, contentRect).GetComponent<RoomToggleObject>());
    }

    private void OnObjectTap(IRoom room, RoomToggleObject toggleObject)
    {
        if(!isSelecting)
        {
            currentSelection.Clear();
            currentSelection.Add(room);
            callback?.Invoke(currentSelection);
            selectedText.text = currentSelection.Count > 1 ? $"{currentSelection.Count} camere alese." : currentSelection[0].Name;
            Close();
        }
        else
        {
            toggleObject.IsOn = !toggleObject.IsOn;
            OnObjectToggle(toggleObject);
        }
    }

    private void OnObjectToggle(RoomToggleObject toggleObject)
    {
        isSelecting = pool.FindAll(i => i.IsOn).Count != 0;
        selectionButtonParent.SetActive(isSelecting);
    }

}
