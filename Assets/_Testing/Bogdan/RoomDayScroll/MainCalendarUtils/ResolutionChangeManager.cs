using UnityEngine;
using UnityEngine.Events;

public class ResolutionChangeManager : MonoBehaviour
{

    private Vector2 res;
    private static UnityEvent OnResolutionChange = new UnityEvent();

    private void Start()
    {
        UpdateResolutionCache();
    }

    private void Update()
    {
        if(res.x != Screen.width || res.y != Screen.height)
        {
            UpdateResolutionCache();
            OnResolutionChange?.Invoke();
        }
    }

    private void OnDestroy()
    {
        OnResolutionChange.RemoveAllListeners();
    }


    public static void AddListener(UnityAction resolutionChangeAction)
    {
        OnResolutionChange.AddListener(resolutionChangeAction);
    }


    private void UpdateResolutionCache()
    {
        res.x = Screen.width;
        res.y = Screen.height;
    }
}
