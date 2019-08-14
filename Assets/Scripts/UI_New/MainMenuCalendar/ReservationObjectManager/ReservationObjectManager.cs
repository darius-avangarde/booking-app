﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ReservationObjectManager : MonoBehaviour
{
    [SerializeField]
    private ReservationsCalendarManager reservationsCalendarManager;

    [Space]
    [SerializeField]
    private GameObject reservationObjectPrefab;
    [SerializeField]
    private RectTransform reservationObjectParent;

    [Space]
    [SerializeField]
    private RectTransform dayColumnObjectPrefabRect;
    [SerializeField]
    private ScrollRect reservationsScrolrect;


    private List<ReservationObject> pool = new List<ReservationObject>();
    private List<IReservation> reservations = new List<IReservation>();
    private List<IReservation> placedReservations = new List<IReservation>();
    private UnityAction<IReservation> reservationButtonAction;
    public struct PointSize
    {
        public Vector2 minPos;
        public Vector2 size;
        public Vector2 pivot;
    }

    private void Start()
    {
        reservationsScrolrect.onValueChanged.AddListener(ClampTextOnScreen);
    }

    public void DisableUnseenReservations()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if(pool[i].ObjRectTransform.TransformPoint(pool[i].ObjRectTransform.rect.max).x < -Screen.width * 2 || pool[i].ObjRectTransform.TransformPoint(pool[i].ObjRectTransform.rect.min).x > Screen.width * 3)
            {
                placedReservations.Remove(pool[i].ObjReservation);
                pool[i].Disable();
            }

            else if(pool[i].ObjRectTransform.TransformPoint(pool[i].ObjRectTransform.rect.max).y < -Screen.height || pool[i].ObjRectTransform.TransformPoint(pool[i].ObjRectTransform.rect.min).y > Screen.height * 2)
            {
                placedReservations.Remove(pool[i].ObjReservation);
                pool[i].Disable();
            }
        }
    }

    public void SweepUpdateReservations(IProperty property, UnityAction<IReservation> tapAction)
    {
        reservationButtonAction = tapAction;
        DisableAllReservationObjects();

        if(property == null || ReservationDataManager.GetActivePropertyReservations(property.ID).Count() == 0)
            return;

        List<CalendarDayColumn> columns = reservationsCalendarManager.OrderedDayColumns;

        DateTime minDate = columns[0].ObjectDate.Date;
        DateTime maxDate = columns[columns.Count - 1].ObjectDate.Date;

        reservations = ReservationDataManager.GetActivePropertyReservations(property.ID).ToList();

        placedReservations = new List<IReservation>();

        foreach (IReservation r in reservations)
        {
            if((r.Period.Start.Date >= minDate && r.Period.Start.Date <= maxDate) || (r.Period.End.Date >= minDate && r.Period.End.Date <= maxDate))
            {
                DrawColumnsReservation(r, columns);
                placedReservations.Add(r);
            }
            else if(r.Period.Start.Date < minDate.Date && r.Period.End.Date > maxDate.Date)
            {
                DrawReservationOverlapped(r, columns);
                placedReservations.Add(r);
            }
        }

        InstantCenterTextOnScreen();
    }

    public void CreateReservationsForColumn(CalendarDayColumn dayColumn, IProperty property, bool isStart)
    {
        if(reservations.Exists(r => (isStart ? r.Period.Start.Date : r.Period.End.Date) == dayColumn.ObjectDate.Date))
        {
            foreach(IReservation res in reservations.FindAll(r => (isStart ? r.Period.Start.Date : r.Period.End.Date) == dayColumn.ObjectDate.Date))
            {
                if(!placedReservations.Exists(r => r.ID == res.ID))
                {
                    placedReservations.Add(res);
                    DrawReservation(res, dayColumn, isStart);
                }
            }
        }
    }

    public void CreateReservationsForRow(List<CalendarDayColumnObject> dayObjects)
    {
        DisableUnseenReservations();

        List<CalendarDayColumn> columns = reservationsCalendarManager.OrderedDayColumns;

        DateTime minDate = columns[0].ObjectDate.Date;
        DateTime maxDate = columns[columns.Count - 1].ObjectDate.Date;

        IRoom focalRoom = dayObjects[0].ObjectRoom;

        List<IReservation> roomReservations = reservations.Where(r => r.ContainsRoom(focalRoom.ID)).ToList();
        for (int i = 0; i < dayObjects.Count; i++)
        {
            if(roomReservations.Exists(r => r.Period.Start.Date == dayObjects[i].ObjDate.Date))
            {
                IReservation res = roomReservations.Find(r => r.Period.Start.Date == dayObjects[i].ObjDate.Date);
                if(!IsReservationPlacedForRoom(res, focalRoom))
                {
                    PointSize p = CalculatePositionSpan(true, (int)(res.Period.End.Date - res.Period.Start.Date).TotalDays, dayObjects[i].DayRectTransform);
                    ReservationObject g = GetFreeReservationObject();
                    g.PlaceUpdateObject(p, dayObjects[i], res, reservationButtonAction);
                    g.ObjTextTransform.localPosition = ScreenCenteredLocalPosition(g);
                    roomReservations.Remove(res);
                }
            }

            else if(roomReservations.Exists(r => r.Period.End.Date == dayObjects[i].ObjDate.Date))
            {
                IReservation res = roomReservations.Find(r => r.Period.End.Date == dayObjects[i].ObjDate.Date);
                if(!IsReservationPlacedForRoom(res, focalRoom))
                {
                    PointSize p = CalculatePositionSpan(false, (int)(res.Period.End.Date - res.Period.Start.Date).TotalDays, dayObjects[i].DayRectTransform);
                    ReservationObject g = GetFreeReservationObject();
                    g.PlaceUpdateObject(p, dayObjects[i], res, reservationButtonAction);
                    g.ObjTextTransform.localPosition = ScreenCenteredLocalPosition(g);
                    roomReservations.Remove(res);
                }
            }


            else if(roomReservations.Exists(r => r.Period.Start.Date < minDate && r.Period.End > maxDate))
            {
                CalendarDayColumnObject firstDayObject = dayObjects.Find(d => d.DayRectTransform.parent.GetSiblingIndex() == 0);
                IReservation res = roomReservations.Find(r => r.Period.Start.Date < minDate && r.Period.End > maxDate);
                if(!IsReservationPlacedForRoom(res, focalRoom))
                {
                    PointSize p = CalculatePositionSpan((int)(res.Period.End.Date - res.Period.Start.Date).TotalDays, (int)(columns[0].ObjectDate.Date - res.Period.Start.Date).TotalDays, dayObjects[i].DayRectTransform);
                    ReservationObject g = GetFreeReservationObject();
                    g.PlaceUpdateObject(p, firstDayObject, res, reservationButtonAction);
                    g.ObjTextTransform.localPosition = ScreenCenteredLocalPosition(g);
                    roomReservations.Remove(res);
                }
            }
        }
    }

    public void DisableAllReservationObjects()
    {
        foreach (ReservationObject r in pool)
        {
            r.Disable();
        }
    }

    private void DrawColumnsReservation(IReservation r, List<CalendarDayColumn> columns)
    {
        foreach (CalendarDayColumnObject cdco in GetDateColumn(r, columns, out bool isStart).ActiveDayColumnsObjects)
        {
            if(cdco.ObjectRoom != null && r.ContainsRoom(cdco.ObjectRoom.ID))
            {
                PointSize p = CalculatePositionSpan(isStart, (int)(r.Period.End.Date - r.Period.Start.Date).TotalDays, cdco.DayRectTransform);
                ReservationObject g = GetFreeReservationObject();
                g.PlaceUpdateObject(p, cdco, r, reservationButtonAction);
                g.ObjTextTransform.localPosition = ScreenCenteredLocalPosition(g);
            }
        }
    }

    private void DrawReservation(IReservation r, CalendarDayColumn column, bool isStart)
    {
        foreach (CalendarDayColumnObject cdco in column.ActiveDayColumnsObjects)
        {
            if(cdco.ObjectRoom != null && r.ContainsRoom(cdco.ObjectRoom.ID))
            {
                PointSize p = CalculatePositionSpan(isStart, (int)(r.Period.End.Date - r.Period.Start.Date).TotalDays, cdco.DayRectTransform);
                ReservationObject g = GetFreeReservationObject();
                g.PlaceUpdateObject(p, cdco, r, reservationButtonAction);
                g.ObjTextTransform.localPosition = ScreenCenteredLocalPosition(g);
            }
        }
    }

    private void DrawReservationOverlapped(IReservation r, List<CalendarDayColumn> columns)
    {
        foreach (CalendarDayColumnObject cdco in columns[0].ActiveDayColumnsObjects)
        {
            if(cdco.ObjectRoom != null && r.ContainsRoom(cdco.ObjectRoom.ID))
            {
                PointSize p = CalculatePositionSpan((int)(r.Period.End.Date - r.Period.Start.Date).TotalDays, (int)(columns[0].ObjectDate.Date - r.Period.Start.Date).TotalDays, cdco.DayRectTransform);
                GetFreeReservationObject().PlaceUpdateObject(p, cdco, r, reservationButtonAction);
            }
        }
    }

    private CalendarDayColumn GetDateColumn(IReservation r, List<CalendarDayColumn> columns, out bool isStart)
    {
        if(columns.Exists(d => d.ObjectDate.Date == r.Period.Start.Date))
        {
            isStart = true;
            return columns.Find(c => c.ObjectDate.Date == r.Period.Start.Date);
        }
        else
        {
            isStart = false;
            return columns.Find(c => c.ObjectDate.Date == r.Period.End.Date);
        }
    }

    private PointSize CalculatePositionSpan(bool isStart, int daySpan, RectTransform dayRect)
    {
        PointSize output = new PointSize();
        output.size.x = (daySpan) * dayRect.rect.width;
        output.size.y = dayColumnObjectPrefabRect.rect.height;

        output.minPos.x = dayRect.parent.localPosition.x;
        output.minPos.y = dayRect.localPosition.y;

        output.pivot = isStart ? Vector2.zero : Vector2.right;

        return output;
    }

    private PointSize CalculatePositionSpan(int daySpan, int differenceFromFirstDate, RectTransform firstRect)
    {
        PointSize output = new PointSize();
        output.size.x = (daySpan) * firstRect.rect.width;
        output.size.y = dayColumnObjectPrefabRect.rect.height;


        output.minPos.x = firstRect.parent.localPosition.x;
        output.minPos.y = firstRect.localPosition.y;

        output.minPos.x -= differenceFromFirstDate * firstRect.rect.width;

        output.pivot = Vector2.zero;

        return output;
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

    private void CreateReservationObject()
    {
        pool.Add(Instantiate(reservationObjectPrefab, reservationObjectParent).GetComponent<ReservationObject>());
        pool[pool.Count -1].Disable();
    }

    private bool IsReservationPlacedForRoom(IReservation reservation, IRoom room)
    {
        return pool.Exists(r => r.gameObject.activeSelf && r.ObjReservation.ID == reservation.ID && r.ObjRoomID == room.ID);
    }

    private void ClampTextOnScreen(Vector2 v)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if(pool[i].gameObject.activeSelf && pool[i].ObjRectTransform.rect.width > pool[i].ObjTextTransform.rect.width * 3)
            {
                pool[i].ObjTextTransform.localPosition = Vector3.Lerp(pool[i].ObjTextTransform.localPosition, ScreenCenteredLocalPosition(pool[i]), Time.deltaTime * 7.5f);
            }
        }
    }

    private void InstantCenterTextOnScreen()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].ObjTextTransform.localPosition = ScreenCenteredLocalPosition(pool[i]);
        }
    }

    private Vector3 ScreenCenteredLocalPosition(ReservationObject resOj)
    {
        Vector3 localLimits = new Vector3();

        if(resOj.gameObject.activeSelf && resOj.ObjRectTransform.rect.width > resOj.ObjTextTransform.rect.width * 3)
        {
            //X is left max local position, Y is right max local position and Z is target center position
            localLimits.x = - resOj.ObjRectTransform.rect.width/2 + resOj.ObjTextTransform.rect.width/2 + 20;
            localLimits.y =  resOj.ObjRectTransform.rect.width/2  - resOj.ObjTextTransform.rect.width/2 - 20;
            localLimits.z = resOj.ObjTextTransform.parent.InverseTransformPoint(reservationsScrolrect.viewport.TransformPoint(reservationsScrolrect.viewport.rect.center)).x;

            return new Vector3(Mathf.Clamp(localLimits.z, localLimits.x, localLimits.y), resOj.ObjTextTransform.localPosition.y, resOj.ObjTextTransform.localPosition.z);
        }
        else
        {
            return Vector3.zero;
        }
    }
}
