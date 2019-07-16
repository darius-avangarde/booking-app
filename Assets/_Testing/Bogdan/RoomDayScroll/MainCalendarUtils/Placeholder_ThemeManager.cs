using UnityEngine;

public class Placeholder_ThemeManager : MonoBehaviour
{
    public Color CalendarHeadCurrentColor;
    public Color CalendarHeadWeekendColor;
    public Color CalendarHeadNormalColor;

    [Space]
    public Color CalendarCurrentColor;
    public Color CalendarWeekendColor;
    public Color CalendarNormalColor;

    [Space]
    public Color currentReservationColor;
    public Color pastReservationColor;

    public static Placeholder_ThemeManager Instance => instance;
    private static Placeholder_ThemeManager instance;


    private void Awake()
    {
        instance = this;
    }
}
