using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestEdit : MonoBehaviour
{
    public ReservationEditScreen resEd;

    public GameObject itemPrefab;

    public ConfirmationDialog cd;



    public Transform clientContent;
    public Transform roomContent;
    public Transform resContent;


    public List<GameObject> goList = new List<GameObject>();

    private ConfirmationDialogOptions opt;

    private void Start() {
        opt = new ConfirmationDialogOptions();
        opt.AditionalCallback = true;
        opt.ConfirmCallback = () => Debug.Log("Done callback 1");
        opt.ConfirmCallbackSecond = () => Debug.Log("Done callback 2");
        opt.CancelCallback = () => Debug.Log("Done callback cancel");
        opt.ConfirmText = "c 1";
        opt.ConfirmTextSecond = "c 2";
        opt.CancelText = " cancel";

        // IProperty pr = PropertyDataManager.GetProperties().Where(p => p.HasRooms && p.Rooms.Count() > 1).ToList()[0];
        // ReservationDataManager.AddReservation(pr.Rooms.ToList()[1], ClientDataManager.GetClients().ToList()[5], DateTime.Today.AddDays(6), DateTime.Today.AddDays(10));
    }

    public void RefreshLists()
    {
        DestroyChildren();
        foreach(IClient c in ClientDataManager.GetClients())
        {
            CreateObject(() => resEd.OpenAddReservation(c, (r) => Debug.Log(r.ID)), c.Name, clientContent, Color.cyan);
        }

        foreach(IProperty p in PropertyDataManager.GetProperties())
        {
            if(p.HasRooms)
            {
                CreateObject(() => resEd.OpenAddReservation(DateTime.Today, DateTime.Today.AddDays(3), p.Rooms.ToList(), (res) => Debug.Log("confirm callback")), "all rooms in" + p.Name, roomContent, Color.yellow);
            }
            foreach(IRoom r in p.Rooms)
            {
                List<IRoom> roomlist = new List<IRoom>();
                roomlist.Add(r);
                CreateObject(() => resEd.OpenAddReservation(DateTime.Today, DateTime.Today.AddDays(3), roomlist, (res) => Debug.Log("confirm callback")), r.Name, roomContent, Color.white);
            }
        }


        foreach(IReservation r in ReservationDataManager.GetReservations())
        {
            CreateObject(() => resEd.OpenEditReservation(r, (res) => Debug.Log("Reservation for: \n" + res.CustomerName), () => Debug.Log("Deleted res callback")), "Reservation for: \n" + r.CustomerName + "\n with" + r.RoomIDs.Count + "rooms", resContent, Color.grey);
        }
    }

    public void CreateObject(UnityAction clickAction, string message, Transform parent, Color setColor)
    {
        GameObject g = Instantiate(itemPrefab, parent);
        goList.Add(g);
        g.GetComponent<TestObject>().Initialize(clickAction, message, setColor);
    }

    public void OpenModalConfig()
    {
        opt.AditionalCallback = !opt.AditionalCallback;
        cd.Show(opt);
    }

    public void DestroyChildren()
    {
        foreach(GameObject go in goList)
        {
            Destroy(go);
        }

        goList.Clear();
    }



}
