using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleDialog : MonoBehaviour, IClosable
{
    [SerializeField]
    private ModalFadeObject modalFade = null;
    [SerializeField]
    private Text titleText = null;
    [SerializeField]
    private Text cancelButtonText = null;
    [SerializeField]
    private Text confirmButtonText = null;
    [SerializeField]
    private Transform optionsParent;
    [SerializeField]
    private GameObject toggleOptionPrefab;
    [SerializeField]
    private ToggleGroup toggleGroup;

    public string defaultMessage = "Sunteți sigur?";
    public string defaultConfirmButtonText = "Confirmați";
    public string defaultCancelButtonText = "Anulați";
    private Action CancelCallback;
    private Action CurrentCallback;

    private GameObject[] spawnedObjects = new GameObject[0];
    private ToggleDialogOptions currentOptions;


    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Reset()
    {
        ClearSpawnedOptions();
        CurrentCallback = null;
        CancelCallback = null;
    }


    ///<summary>
    ///Opens the modal toggle dialog with the given toggle options
    ///</summary>
    public void Show(ToggleDialogOptions options)
    {
        ClearSpawnedOptions();
        spawnedObjects = new GameObject[options.ToggleOptions.Length];
        currentOptions = options;

        for (int i = 0; i < options.ToggleOptions.Length; i++)
        {
            spawnedObjects[i] = Instantiate(toggleOptionPrefab, optionsParent);
            spawnedObjects[i].transform.SetSiblingIndex(1 + i);
            ToggleDialogObj tog = spawnedObjects[i].GetComponent<ToggleDialogObj>();
            int optIndex = i;
            tog.SetObject(options.ToggleOptions[i].text, toggleGroup, (b) => SetOption(b, optIndex));

            if(i == 0)
            {
                tog.GetComponent<Toggle>().isOn = true;
            }
        }

        titleText.text = options.TitleMessage ?? defaultMessage;

        confirmButtonText.text = options.ConfirmText ?? defaultConfirmButtonText;
        cancelButtonText.text = options.CancelText ?? defaultCancelButtonText;

        CancelCallback = options.CancelCallback;
        modalFade.FadeIn();
        InputManager.CurrentlyOpenClosable = this;
    }

    public void Confirm()
    {
        CurrentCallback?.Invoke();
        modalFade.FadeOut();
        InputManager.CurrentlyOpenClosable = null;
        Reset();
    }

    public void Cancel()
    {
        CancelCallback?.Invoke();
        modalFade.FadeOut();
        InputManager.CurrentlyOpenClosable = null;
        Reset();
    }

    public void Close()
    {
        Cancel();
    }

    private void ClearSpawnedOptions()
    {
        foreach(GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects = new GameObject[0];
    }

    private void SetOption(bool toggle, int index)
    {
        if(toggle)
        {
            CurrentCallback = currentOptions.ToggleOptions[index].callback;
        }
    }
}

public class ToggleDialogOptions
{
    public string TitleMessage { get; set; }
    public string ConfirmText { get; set; }
    public string CancelText { get; set; }
    public ToggleOption[] ToggleOptions { get; set; }
    public Action CancelCallback { get; set; }


    ///<summary>
    ///Sets the toggle options for the dialog options object. Node: for a 1080p portrait resolution the recomended maximum number of options is <= 10
    ///</summary>
    public void SetOptions(params ToggleOption[] options)
    {
        ToggleOptions = options;
    }
}

public class ToggleOption
{
    public string text;
    public Action callback;

    ///<summary>
    ///Creates a new toggle option with the given option name and callback.
    ///</summary>
    public ToggleOption(string optionText, Action optionCallback)
    {
        text = optionText;
        callback = optionCallback;
    }
}
