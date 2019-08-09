using System;
using System.Collections.Generic;
using UnityEngine;

public class DoOnMainThread : MonoBehaviour
{
    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    public virtual void Update()
    {
        // dispatch stuff on main thread
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }
    }
}
