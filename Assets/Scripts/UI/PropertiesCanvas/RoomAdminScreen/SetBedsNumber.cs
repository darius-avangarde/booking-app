using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBedsNumber : MonoBehaviour
{
    [SerializeField]
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;

    private int SingleBedsNr;
    private int DoubleBedsNr;

    private void Awake()
    {
        roomAdminScreen.GetBedsNumber += GetCurrentBeds;
        roomAdminScreen.SetBedsNumber += SetCurrentBeds;
    }

    private void SetCurrentBeds(int singleBeds, int doubleBeds)
    {
        SingleBedsNr = singleBeds;
        DoubleBedsNr = doubleBeds;
        roomSingleBedQuantityInputField.text = SingleBedsNr.ToString();
        roomDoubleBedQuantityInputField.text = DoubleBedsNr.ToString();
    }

    private void GetCurrentBeds()
    {
        roomAdminScreen.CurrentRoom.SingleBeds = SingleBedsNr;
        roomAdminScreen.CurrentRoom.DoubleBeds = DoubleBedsNr;
    }

    public void OnSingleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomSingleBedQuantityInputField.text = "";
            return;
        }
        SingleBedsNr = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void OnDoubleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomDoubleBedQuantityInputField.text = "";
            return;
        }
        DoubleBedsNr = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
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
}
