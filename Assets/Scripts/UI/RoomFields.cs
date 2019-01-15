using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomFields : MonoBehaviour
{
    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Text personsNumber;
    
    public void Initialize(IRoom room)
    {
        roomName.text = "room.Name";
        personsNumber.text = personsNumber.text + ": " + "room.Persons.ToString()";
    }
}
