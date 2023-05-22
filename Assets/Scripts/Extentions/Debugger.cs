using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugger : MonoBehaviour
{
    public static event System.Action<string> OnLog;

    public static void Log(string value)
    {
        Debug.Log(value);

        OnLog?.Invoke(value);
    }
}