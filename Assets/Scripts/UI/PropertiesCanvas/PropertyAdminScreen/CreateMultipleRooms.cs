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

    private void SaveMultipleRooms(bool shouldGoBack)
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

    private void DeleteRooms()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Urmeaza sa se stearga cateva camere, se vor sterge si rezervarile acestora.",
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                ChangeRooms();
            },
            CancelCallback = null
        });
    }

    private void ChangeRooms()
    {
        propertyAdminScreen.CurrentProperty.Floors = multipleFloorsNumber;
        propertyAdminScreen.CurrentProperty.FloorRooms = multipleRoomsNumber;

        roomsList = new List<IRoom>();

        for (int i = 0; i <= multipleFloorsNumber; i++)
        {
            CreateFloorRooms(i);
        }

        propertyAdminScreen.CurrentProperty.SaveMultipleRooms(roomsList);
        propertyAdminScreen.navigator.GoBack();
    }

    private void CreateFloorRooms(int currentFloor)
    {
        propertyAdminScreen.CurrentProperty.FloorRooms[currentFloor] = multipleRoomsNumber[currentFloor];
        if (currentFloor == 0)
        {
            for (int j = previousRooms[currentFloor] + 1; j <= multipleRoomsNumber[currentFloor]; j++)
            {
                IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                newRoom.Name = $"Camera {j}";
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
            for (int j = previousRooms[currentFloor] + 1; j <= multipleRoomsNumber[currentFloor]; j++)
            {
                IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                if (j < 10)
                {
                    newRoom.Name = $"Camera {currentFloor}0{j}";
                    newRoom.RoomNumber = int.Parse($"{currentFloor}0{j}");
                }
                else
                {
                    newRoom.Name = $"Camera {currentFloor}{j}";
                    newRoom.RoomNumber = int.Parse($"{currentFloor}{j}");
                }
                newRoom.Floor = currentFloor;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }

            for (int j = previousRooms[currentFloor] + 1; j > multipleRoomsNumber[currentFloor]; j--)
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
                for (int i = 0; i < roomsToDelete.Count; i++)
                {
                    propertyAdminScreen.CurrentProperty.DeleteRoom(roomsToDelete[i].ID);
                }
            }
        }
    }
}
