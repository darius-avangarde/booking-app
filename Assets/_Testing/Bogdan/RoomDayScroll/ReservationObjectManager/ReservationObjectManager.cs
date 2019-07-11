using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ReservationObjectManager : MonoBehaviour
{
    [SerializeField]
    private ReservationsCalendarManager reservationsCalendarManager;

    [Space]
    [SerializeField]
    private GameObject reservationObjectPrefab;
    [SerializeField]
    private RectTransform reservationObjectParent; //this will be a recttransform copy of the daycolumn sccrolrectContent
                                                    //or a 0 by 0 rect in the clients contents rect (need to hadle hierarchy)
    [Space]
    [SerializeField]
    private RectTransform dayColumnObjectPrefabRect;

    private List<ReservationObject> pool = new List<ReservationObject>();

    public void SweepUpdateReservations(IProperty property, UnityAction<IReservation> tapAction)
    {
        DisableAllReservationObjects();
        //get ordered by hierarchy day columns
        List<CalendarDayColumn> columns = reservationsCalendarManager.GetHierarchyOrderedDayColumns();

        DateTime minDate = columns[0].ObjectDate.Date;
        DateTime maxDate = columns[columns.Count - 1].ObjectDate.Date;

        List<IReservation> reservations = ReservationDataManager.GetActivePropertyReservations(property.ID).ToList();
        //ManagePool(reservations);

        List<IReservation> placedReservations = new List<IReservation>();

        foreach (IReservation r in reservations)
        {
            if((r.Period.Start.Date >= minDate && r.Period.Start.Date <= maxDate) || (r.Period.End.Date >= minDate && r.Period.End.Date <= maxDate))
            {
                Debug.Log(r.CustomerName + " - "
                    + r.Period.Start.ToString(Constants.DateTimePrintFormat)
                    + " - " + r.Period.End.ToString(Constants.DateTimePrintFormat)
                    + " - " + PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomIDs[0]).Name
                    );
                DrawReservation(r, columns);
            }
        }
    }

    private void DrawReservation(IReservation r, List<CalendarDayColumn> columns)
    {
        ;
        ;


        List<CalendarDayColumnObject> activeObjects = GetDateColumn(r,columns, out bool isStart).GetActiveColumnObjects();

        foreach (CalendarDayColumnObject cdco in activeObjects)
        {
            if(r.ContainsRoom(cdco.ObjectRoom.ID))
            {
                PointSize p = CalculatePositionSpan(isStart, (int)(r.Period.End.Date - r.Period.Start.Date).TotalDays, cdco.DayRectTransform);
                //TODO: Set callback to open reservation edit panel with reservation
                GetFreeReservationObject().PlaceUpdateObject(p.minPos, p.size, r, (res) => Debug.Log($"Edit reservation for {res.CustomerName} with {r.RoomIDs.Count} rooms"));
            }
        }
    }

    private CalendarDayColumn GetDateColumn(IReservation r, List<CalendarDayColumn> columns, out bool isStart)
    {
        if(columns.Exists(d => d.ObjectDate == r.Period.Start))
        {
            isStart = true;
            return columns.Find(c => c.ObjectDate == r.Period.Start.Date);
        }
        else
        {
            isStart = true;
            return columns.Find(c => c.ObjectDate == r.Period.End.Date);
        }
    }

    //TODO: position is taken from the first rect only (probably instantiated too soon)
    private PointSize CalculatePositionSpan(bool isStart, int daySpan, RectTransform dayRect)
    {
        dayRect.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.yellow;

        PointSize output = new PointSize();

        if(!isStart)
        {
            output.size.x = daySpan * dayRect.rect.width;
            output.size.y = dayColumnObjectPrefabRect.rect.height;

            output.minPos = dayRect.TransformPoint(dayRect.rect.min);
        }
        else
        {
            output.size.x = daySpan * dayRect.rect.width;
            output.size.y = dayColumnObjectPrefabRect.rect.height;

            Vector2 maxPos = dayRect.TransformPoint(dayRect.rect.max);
            output.minPos.x = maxPos.x - output.size.x;
            output.minPos.y = maxPos.y - output.size.y;
        }
        return output;
    }

    struct PointSize
    {
        public Vector2 minPos;
        public Vector2 size;
    }

    private void ManagePool(List<IReservation> reservations)
    {
        if(pool.Count != reservations.Count)
        {
            //Create New Objects as needed
            while(pool.Count < reservations.Count)
            {
                CreateReservationObject();
            }

            //Disable unused objects
            for (int i = pool.Count - 1; i > reservations.Count; i--)
            {
                pool[i].gameObject.SetActive(false);
            }
        }
    }

    private ReservationObject GetFreeReservationObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if(!pool[i].gameObject.activeSelf)
            {
                return pool[i];
            }
        }

        CreateReservationObject();
        return pool[pool.Count -1];
    }

    private void DisableAllReservationObjects()
    {
        foreach (ReservationObject r in pool)
        {
            r.Disable();
        }
    }

    private void CreateReservationObject()
    {
        pool.Add(Instantiate(reservationObjectPrefab, reservationObjectParent).GetComponent<ReservationObject>());
        pool[pool.Count -1].Disable();
    }
}
