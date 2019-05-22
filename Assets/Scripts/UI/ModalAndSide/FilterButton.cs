using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour
{
    [SerializeField]
    private Text roomCapacityText = null;
    //[SerializeField]
    //private Text roomPriceText = null;
    [SerializeField]
    private Text singleBedText = null;
    [SerializeField]
    private Text doubleBedText = null;
    [SerializeField]
    private Text propertyInfoText = null;

    public void UpdateFilterButtonText(RoomFilter filter)
    {
        string propertyInfo = "";
        string roomCapacityInfo = Constants.Persoane + filter.RoomCapacity.ToString();
        string singleBedInfo = Constants.SingleBed + filter.SingleBeds;
        string doubleBedInfo = Constants.DoubleBed + filter.DoubleBeds;

        if (!string.IsNullOrEmpty(filter.PropertyID))
        {
            propertyInfo = Constants.Proprietate + PropertyDataManager.GetProperty(filter.PropertyID).Name;
        }

        propertyInfoText.text = propertyInfo;
        roomCapacityText.text = roomCapacityInfo;
        //roomPriceText.text = roomPriceInfo;
        singleBedText.text = singleBedInfo;
        doubleBedText.text = doubleBedInfo;
    }
}
