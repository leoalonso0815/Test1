using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonExtension
{
    public static void AddListener(this Button button, UnityAction call)
    {
        if (null != button)
        {
            button.onClick.AddListener(call);
        }
        else
        {
            Debug.LogError("button is null");
        }
    }
}