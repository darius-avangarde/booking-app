using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClientPickerObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clientText;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image underline;

    private IClient currentClient = null;
    private UnityAction<IClient> buttonCallback;

    private void Start()
    {
        ThemeManager.Instance.AddItems(clientText, backgroundImage, underline);
    }

    public void SetAndEnable(IClient client, UnityAction<IClient> callback)
    {
        currentClient = client;
        buttonCallback = callback;
        clientText.text = $"{currentClient.Name}\n<i>{currentClient.Number}";

        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void TriggerObject()
    {
        buttonCallback?.Invoke(currentClient);
    }
}
