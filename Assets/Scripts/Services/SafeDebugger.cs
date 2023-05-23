using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeDebugger : MonoBehaviour
{
    public static event System.Action<string> OnLog;

    public static void Log(string value)
    {
        UnityMainThreadDispatcher instance = UnityMainThreadDispatcher.Instance();
        if (!instance)
            return;

        instance.Enqueue(() =>
        {
            Debug.Log(value);

            OnLog?.Invoke(value);
        });
    }
}