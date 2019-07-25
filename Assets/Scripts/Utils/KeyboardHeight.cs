using UnityEngine;

public class KeyboardHeight : MonoBehaviour
{
    private static float GetSizeRatio()
    {
        if (Application.isEditor)
            return 0.4f;

#if UNITY_ANDROID
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var view = unityClass.GetStatic<AndroidJavaObject>("currentActivity")
                .Get<AndroidJavaObject>("mUnityPlayer")
                .Call<AndroidJavaObject>("getView");

            var dialog = unityClass.GetStatic<AndroidJavaObject>("currentActivity")
                .Get<AndroidJavaObject>("mUnityPlayer")
                .Get<AndroidJavaObject>("b");

            var decorView = dialog.Call<AndroidJavaObject>("getWindow")
                .Call<AndroidJavaObject>("getDecorView");

            var height = decorView.Call<int>("getHeight");

            using (var rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                view.Call("getWindowVisibleDisplayFrame", rect);
                return (float)(Screen.height - rect.Call<int>("height") + height) / Screen.height;
            }
        }
#elif UNITY_IOS
        return (float) TouchScreenKeyboard.area.height / Screen.height;
#endif
    }
}
