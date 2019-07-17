using UnityEngine;
using UnityEngine.UI;

public class ToggleSettings : MonoBehaviour
{
   [SerializeField]
    private ThemeManager theme;
    [SerializeField]
    private Image imageRound;
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private GameObject checkmarck;
    public readonly static Color CheckColor = new Color(0.1921569f, 0.6588235f, 0.2039216f);
    public readonly static Color textDark = new Color(0.9607843f, 0.9607843f, 0.9607843f);
    public void ActivateToggle()
    {
        theme.SelectTheme();
        if (toggle.isOn)
        {
            checkmarck.SetActive(true);
            imageRound.color =CheckColor;
        }
        else
        {
            checkmarck.SetActive(false);
            imageRound.color = textDark;
        }
    }
}
