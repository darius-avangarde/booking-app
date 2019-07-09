using UnityEngine;
using UnityEngine.UI;

public class RotateUpdate : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvasToRebuild;

    private void Start()
    {
        DeviceChange.OnOrientationChange += MyOrientationChangeCode;
        DeviceChange.OnResolutionChange += MyResolutionChangeCode;
    }

    void MyOrientationChangeCode(DeviceOrientation orientation)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvasToRebuild);
        Debug.Log("canvas rebuilt < orientation");
    }

    void MyResolutionChangeCode(Vector2 resolution)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvasToRebuild);
        Debug.Log("canvas rebuilt < resolution");
    }


    public void RebuildCanvas()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvasToRebuild);
        Debug.Log("canvas rebuilt < manual");
    }
}
