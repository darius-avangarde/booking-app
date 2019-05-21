using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ClientPickerObject : MonoBehaviour
{
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text numberText;
    [SerializeField]
    private Button button;

    private IClient client;


    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void Initialize(IClient _client, UnityAction<IClient> onSelect)
    {
        client = _client;
        nameText.text = client.Name;
        numberText.text = client.Number;

        button.onClick.AddListener(() => onSelect(client));
    }
}
