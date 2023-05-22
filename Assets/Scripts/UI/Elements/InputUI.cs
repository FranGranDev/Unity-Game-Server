using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Services;


[RequireComponent(typeof(TMP_InputField))]
public class InputUI : MonoBehaviour, Initializable
{
    private TMP_InputField inputField;


    public event System.Action<string> OnFilled;


    public void Initialize()
    {
        inputField = GetComponent<TMP_InputField>();

        inputField.onEndEdit.AddListener(CallOnFilled);
    }

    private void CallOnFilled(string text)
    {
        OnFilled?.Invoke(text);
    }
    public void Fill(string text)
    {
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }


        inputField.text = text;
    }
}
