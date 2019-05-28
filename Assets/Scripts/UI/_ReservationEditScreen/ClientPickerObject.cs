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

    ///<summary>
    ///Sets the target client for this picker object and adds selectAction to the onClick event of the object.
    ///</summary>
    internal void Initialize(IClient _client, UnityAction<IClient> selectAction)
    {
        client = _client;
        nameText.text = client.Name;
        numberText.text = client.Number;

        button.onClick.AddListener(() => selectAction(client));
    }
}
