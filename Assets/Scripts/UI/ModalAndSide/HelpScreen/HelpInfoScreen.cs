using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpInfoScreen : MonoBehaviour
{
    [SerializeField]
    private Text infoTitle = null;
    [SerializeField]
    private TextMeshProUGUI infoText = null;
    [SerializeField]
    private RectTransform contentTransform = null;

    public void Initialize(string title)
    {
        switch (title)
        {
            case "Pagina principală":
                infoTitle.text = LocalizedText.Instance.HelpTextMainPage[0]; 
                infoText.text = LocalizedText.Instance.HelpTextMainPage[1]; 
                break;
            case "Proprietăți":
                infoTitle.text = LocalizedText.Instance.HelpTextPropertyPage[0];
                infoText.text = LocalizedText.Instance.HelpTextPropertyPage[1];
                break;
            case "Camere":
                infoTitle.text = LocalizedText.Instance.HelpTextRoomPage[0];
                infoText.text = LocalizedText.Instance.HelpTextRoomPage[1];
                break;
            case "Clienți":
                infoTitle.text = LocalizedText.Instance.HelpTextClientPage[0];
                infoText.text = LocalizedText.Instance.HelpTextClientPage[1];
                break;
            case "Rezervări":
                infoTitle.text = LocalizedText.Instance.HelpTextReservationPage[0];
                infoText.text = LocalizedText.Instance.HelpTextReservationPage[1];
                break;
            case "Filtrare calendar":
                infoTitle.text =LocalizedText.Instance.HelpTextFilterPage[0];
                infoText.text = LocalizedText.Instance.HelpTextFilterPage[1];
                break;
            default:
                break;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
        Canvas.ForceUpdateCanvases();
    }
}
