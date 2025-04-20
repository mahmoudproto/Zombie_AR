using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VisualLogger : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI loggerTextElement;

    static VisualLogger Instance;

    private void Start()
    {
        Instance = this;
    }

    public static void Log(string text)
    {
        Debug.Log(text);
        Instance.loggerTextElement.text += text;
    }
}
