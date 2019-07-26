using UnityEngine;
[CreateAssetMenu(fileName = "ThemeColors", menuName = "Theme Colors")]
public class ColorsData : ScriptableObject
{
    #region ColorsTheme
    public Color darkBackground = new Color(0, 0, 0);
    public Color lightBackground = new Color(0.8862745f, 0.8862745f, 0.8862745f);
    public Color separatorDark = new Color(0.5058824f, 0.5058824f, 0.5058824f);
    public Color separatorLight = new Color(0.282353f, 0.282353f, 0.282353f);
    public Color textDark = new Color(0.9607843f, 0.9607843f, 0.9607843f);
    public Color textLight = new Color(0.2235294f, 0.2235294f, 0.2235294f);
    public Color ItemDark = new Color(0.1254902f, 0.1254902f, 0.1254902f);
    public Color ItemLight = new Color(0.945098f, 0.945098f, 0.945098f);
    public Color CalendarHeadCurrentColor;
    public Color CalendarHeadWeekendColor;
    public Color CalendarHeadNormalColor;
    public Color CalendarCurrentColor;
    public Color CalendarWeekendColor;
    public Color CalendarNormalColor;
    public Color CurrentReservationColor;
    public Color PastReservationColor;
    #endregion
}
