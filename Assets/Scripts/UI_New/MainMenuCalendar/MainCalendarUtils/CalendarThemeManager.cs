using UnityEngine;

public class CalendarThemeManager : MonoBehaviour
{
    public Color CalendarHeadCurrentColor;
    public Color CalendarHeadWeekendColor;
    public Color CalendarHeadNormalColor;
    public Color CalendarCurrentColor;
    public Color CalendarWeekendColor;
    public Color CalendarNormalColor;
    public Color currentReservationColor;
    public Color pastReservationColor;

    public Sprite currentDaySprite;
    public Sprite selectedDaySprite;


    public static CalendarThemeManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<CalendarThemeManager>();
            }
            return instance;
        }
    }

    private static CalendarThemeManager instance;

}
