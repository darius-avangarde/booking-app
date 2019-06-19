using UnityEngine;

public class ToggleTestOpen : MonoBehaviour
{
    [SerializeField]
    private ToggleDialog toggleDialog;

    private ToggleDialogOptions opt;
    // Start is called before the first frame update
    void Start()
    {
        opt = new ToggleDialogOptions();
        opt.TitleMessage = "Test options";
        opt.SetOptions(
            new ToggleOption("Option 1", () => Debug.Log("calback for option 1")),
            new ToggleOption("Option 2", () => Debug.Log("calback for option 2"))//,
            // new ToggleOption("Option 3", () => Debug.Log("calback for option 3")),
            // new ToggleOption("Option 4", () => Debug.Log("calback for option 4")),
            // new ToggleOption("Option 5", () => Debug.Log("calback for option 5")),
            // new ToggleOption("Option 6", () => Debug.Log("calback for option 6")),
            // new ToggleOption("Option 2", () => Debug.Log("calback for option 2")),
            // new ToggleOption("Option 3", () => Debug.Log("calback for option 3")),
            // new ToggleOption("Option 4", () => Debug.Log("calback for option 4")),
            // new ToggleOption("Option 5", () => Debug.Log("calback for option 5")),
            // new ToggleOption("Option 6", () => Debug.Log("calback for option 6"))
        );
    }

    [ContextMenu("Show toggler")]
    public void Open()
    {
        toggleDialog.Show(opt);
    }
}
