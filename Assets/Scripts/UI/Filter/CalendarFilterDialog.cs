using System;
using UnityEngine;

public class CalendarFilterDialog : MonoBehaviour
{
    [SerializeField]
    private PropertyDropdown propertyDropdown = null;
    [SerializeField]
    private IntegerSelector roomCapacitySelector = null;
    [SerializeField]
    private IntegerSelector singleBedCountSelector = null;
    [SerializeField]
    private IntegerSelector doubleBedCountSelector = null;

    private RoomFilter filter = null;
    private Action<RoomFilter> doneCallback;

    public void Show(RoomFilter filter, Action<RoomFilter> doneCallback)
    {
        propertyDropdown.Initialize();
        this.filter = filter;
        this.doneCallback = doneCallback;

        propertyDropdown.SelectedProperty = PropertyDataManager.GetProperty(filter?.PropertyID);
        roomCapacitySelector.Value = filter?.RoomCapacity ?? 0;
        singleBedCountSelector.Value = filter?.SingleBeds ?? 0;
        doubleBedCountSelector.Value = filter?.DoubleBeds ?? 0;
    }

    public void ResetFilter()
    {
        filter.Reset();
    }

    public void CloseDialog()
    {
        filter.PropertyID = propertyDropdown.SelectedProperty?.ID;
        filter.RoomCapacity = roomCapacitySelector.Value;
        filter.SingleBeds = singleBedCountSelector.Value;
        filter.DoubleBeds = doubleBedCountSelector.Value;

        doneCallback?.Invoke(filter);
        filter = null;
        doneCallback = null;
    }
}
