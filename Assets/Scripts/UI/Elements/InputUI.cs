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

        inputField.onSubmit.AddListener(CallOnFilled);
    }

    private void CallOnFilled(string text)
    {
        OnFilled?.Invoke(text);
    }
    public void Fill(string text)
    {
        inputField.text = text;
    }
}
