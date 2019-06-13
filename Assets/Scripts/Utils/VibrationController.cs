using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationController
{
    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    public AndroidJavaObject sysService;


    public VibrationController()
    {
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        sysService = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    }

    public void vibrate()
    {
        sysService.Call("vibrate");
    }


    public void vibrate(long milliseconds)
    {
        sysService.Call("vibrate", milliseconds);
    }

    public void cancel()
    {
        sysService.Call("cancel");
    }

    public bool hasVibrator()
    {
        return sysService.Call<bool>("hasVibrator");
    }
}

