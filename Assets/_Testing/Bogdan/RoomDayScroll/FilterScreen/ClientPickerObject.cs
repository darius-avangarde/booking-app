using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ClientPickerObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clientText;

    private IClient currentClient = null;
    private UnityAction<IClient> buttonCallback;

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
