using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class UIDebugView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;

        private void Awake()
        {
            SafeDebugger.OnLog += Log;
        }

        private void Log(string obj)
        {
            textMesh.text += (obj + "\n");
        }
    }
}
