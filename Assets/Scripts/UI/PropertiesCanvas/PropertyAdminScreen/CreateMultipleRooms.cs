using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMultipleRooms : MonoBehaviour
{
    public int multipleFloorsNumber { get; set; } = 0;
    public int[] multipleRoomsNumber { get; set; } = new int[1];

    [SerializeField]
    private PropertyAdminScreen propertyAdminScreen = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;

    private List<IRoom> roomsList = new List<IRoom>();
    private int previousFloors = 0;
    private int[] previousRooms = new int[1];
    private bool changeRooms = true;

    private void Start()
    {
        propertyAdminScreen.MultipleRooms += SaveMultipleRooms;
    }

    /// <summary>
    /// function to save all rooms from input fields
    /// </summary>
    private void SaveMultipleRooms()
    {
        if (propertyAdminScreen.CurrentProperty.Floors > 0)
        {
            previousFloors = propertyAdminScreen.CurrentProperty.Floors;
            previousRooms = new int[previousFloors + 1];
            for (int i = 0; i <= previousFloors; i++)
            {
                previousRooms[i] = propertyAdminScreen.CurrentProperty.FloorRooms[i];
            }
        }
        else
        {
            previousRooms = new int[multipleFloorsNumber + 1];
        }

        CheckRoomChanges();
    }

    /// <summary>
    /// function to check if the rooms edited are fewer than the rooms before edit
    /// if yes, the modal dialog is opened
    /// </summary>
    private void CheckRoomChanges()
    {
        if (multipleFloorsNumber < propertyAdminScreen.CurrentProperty.Floors)
        {
            changeRooms = false;
            DeleteRooms();
        }
        else
        {
            for (int i = 0; i < propertyAdminScreen.CurrentProperty.Floors; i++)
            {
                if (multipleRoomsNumber[i] < propertyAdminScreen.CurrentProperty.FloorRooms[i])
                {
                    changeRooms = false;
                    DeleteRooms();
                    break;
                }
                else
                {
                    changeRooms = true;
                }
            }
        }
        if (changeRooms)
        {
            ChangeRooms();
        }
    }

    /// <summary>
    /// function to delete unused rooms
    /// </summary>
    private void DeleteRooms()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = LocalizedText.Instance.EditPropertyRooms,
            ConfirmText = LocalizedText.Instance.ConfirmAction[0],
            CancelText = LocalizedText.Instance.ConfirmAction[1],
            ConfirmCallback = () =>
            {
                ChangeRooms();
            },
            CancelCallback = null
        });
    }

    /// <summary>
    /// function to change rooms values for a property
    /// </summary>
    private void ChangeRooms()
    {
        int previousFloors = propertyAdminScreen.CurrentProperty.Floors;
        propertyAdminScreen.CurrentProperty.Floors = multipleFloorsNumber;
        propertyAdminScreen.CurrentProperty.FloorRooms = multipleRoomsNumber;

        roomsList = new List<IRoom>();

        if (previousFloors > multipleFloorsNumber)
        {
            for (int i = previousFloors; i > multipleFloorsNumber; i--)
            {
                List<IRoom> roomsToDelete = propertyAdminScreen.CurrentProperty.Rooms.Where(r => r.Floor == i).ToList();
                DeleteFloorRooms(roomsToDelete);
            }
        }

        for (int i = 0; i <= multipleFloorsNumber; i++)
        {
            CreateFloorRooms(i);
        }

        propertyAdminScreen.CurrentProperty.SaveMultipleRooms(roomsList);
        propertyAdminScreen.navigator.GoBack();
    }

    /// <summary>
    /// create rooms for a specific floor
    /// </summary>
    /// <param name="currentFloor">floor number</param>
    private void CreateFloorRooms(int currentFloor)
    {
        propertyAdminScreen.CurrentProperty.FloorRooms[currentFloor] = multipleRoomsNumber[currentFloor];
        if (currentFloor == 0)
        {
            for (int j = previousRooms[currentFloor] + 1; j <= multipleRoomsNumber[currentFloor]; j++)
            {
                IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                newRoom.Name = $"{j}";
                newRoom.RoomNumber = j;
                newRoom.Floor = 0;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }

            for (int j = previousRooms[currentFloor] + 1; j > multipleRoomsNumber[currentFloor]; j--)
            {
                List<IRoom> roomsToDelete = propertyAdminScreen.CurrentProperty.Rooms.Where(r => r.Floor == currentFloor && r.RoomNumber == j).ToList();
                for (int i = 0; i < roomsToDelete.Count; i++)
                {
                    propertyAdminScreen.CurrentProperty.DeleteRoom(roomsToDelete[i].ID);
                }
            }
        }
        else
        {
            int previousFloorRooms = 1;
            if (previousRooms.Length - 1 >= currentFloor)
            {
                previousFloorRooms = previousRooms[currentFloor] + 1;
            }
            for (int j = previousFloorRooms; j <= multipleRoomsNumber[currentFloor]; j++)
            {
                IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                if (j < 10)
                {
                    newRoom.Name = $"{currentFloor}0{j}";
                    newRoom.RoomNumber = int.Parse($"{currentFloor}0{j}");
                }
                else
                {
                    newRoom.Name = $"{currentFloor}{j}";
                    newRoom.RoomNumber = int.Parse($"{currentFloor}{j}");
                }
                newRoom.Floor = currentFloor;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }

            for (int j = previousFloorRooms; j > multipleRoomsNumber[currentFloor]; j--)
            {
                int roomNumber = 0;
                if (j < 10)
                {
                    roomNumber = int.Parse($"{currentFloor}0{j}");
                }
                else
                {
                    roomNumber = int.Parse($"{currentFloor}{j}");
                }
                List<IRoom> roomsToDelete = propertyAdminScreen.CurrentProperty.Rooms.Where(r => r.Floor == currentFloor && r.RoomNumber == roomNumber).ToList();
                DeleteFloorRooms(roomsToDelete);
            }
        }
    }

    /// <summary>
    /// delete a list of rooms
    /// </summary>
    /// <param name="roomsToDelete">list of rooms</param>
    private void DeleteFloorRooms(List<IRoom> roomsToDelete)
    {
        for (int i = 0; i < roomsToDelete.Count; i++)
        {
            propertyAdminScreen.CurrentProperty.DeleteRoom(roomsToDelete[i].ID);
        }
    }
}
