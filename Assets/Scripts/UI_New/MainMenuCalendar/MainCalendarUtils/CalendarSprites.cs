using UnityEngine;

public class CalendarSprites : MonoBehaviour
{
    public Sprite currentDaySprite;
    public Sprite selectedDaySprite;

    public static CalendarSprites Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<CalendarSprites>();
            }
            return instance;
        }
    }

    private static CalendarSprites instance;

}
