using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMultipleRooms : MonoBehaviour
{
    public int multipleFloorsNumber { get; set; } = 0;
    public int multipleRoomsNumber { get; set; } = 0;

    [SerializeField]
    private PropertyAdminScreen propertyAdminScreen;

    private void Start()
    {
        propertyAdminScreen.MultipleRooms += SaveMultipleRooms;
    }

    private void SaveMultipleRooms(bool overridePrevious, bool shouldGoBack)
    {
        int previousFloors = 0;
        int previousRooms = 0;
        if (!overridePrevious)
        {
            previousFloors = propertyAdminScreen.CurrentProperty.Rooms.Count() / propertyAdminScreen.CurrentProperty.FloorRooms;
            previousRooms = propertyAdminScreen.CurrentProperty.FloorRooms;
        }
        else
        {
            foreach (var room in propertyAdminScreen.CurrentProperty.Rooms)
            {
                propertyAdminScreen.CurrentProperty.DeleteRoom(room.ID);
            }
        }
        propertyAdminScreen.CurrentProperty.FloorRooms = previousRooms + multipleRoomsNumber;
        List<IRoom> roomsList = new List<IRoom>();
        if (multipleFloorsNumber > 0)
        {
            for (int j = previousRooms + 1; j <= previousRooms + multipleRoomsNumber; j++)
            {
                IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                newRoom.Name = $"Camera {j}";
                newRoom.RoomNumber = j;
                newRoom.Floor = 0;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }
            for (int i = 1; i <= multipleFloorsNumber; i++)
            {
                if (i < previousFloors)
                {
                    for (int j = previousRooms + 1; j <= previousRooms + multipleRoomsNumber; j++)
                    {
                        IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                        if (j < 10)
                        {
                            newRoom.Name = $"Camera {i}0{j}";
                            newRoom.RoomNumber = int.Parse($"{i}0{j}");
                        }
                        else
                        {
                            newRoom.Name = $"Camera {i}{j}";
                            newRoom.RoomNumber = int.Parse($"{i}{j}");
                        }
                        newRoom.Floor = i;
                        newRoom.Multiple = true;
                        roomsList.Add(newRoom);
                    }
                }
                else
                {
                    for (int j = 1; j <= multipleRoomsNumber; j++)
                    {
                        IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                        if (j < 10)
                        {
                            newRoom.Name = $"Camera {i}0{j}";
                            newRoom.RoomNumber = int.Parse($"{i}0{j}");
                        }
                        else
                        {
                            newRoom.Name = $"Camera {i}{j}";
                            newRoom.RoomNumber = int.Parse($"{i}{j}");
                        }
                        newRoom.Floor = i;
                        newRoom.Multiple = true;
                        roomsList.Add(newRoom);
                    }
                }
            }
        }
        else
        {
            for (int j = previousRooms + 1; j <= previousRooms + multipleRoomsNumber; j++)
            {
                IRoom newRoom = propertyAdminScreen.CurrentProperty.AddRoom();
                newRoom.Name = $"Camera {j}";
                newRoom.RoomNumber = j;
                newRoom.Floor = 0;
                newRoom.Multiple = true;
                roomsList.Add(newRoom);
            }
        }
        propertyAdminScreen.CurrentProperty.SaveMultipleRooms(roomsList);
        if (shouldGoBack)
        {
            propertyAdminScreen.navigator.GoBack();
        }
    }
}
