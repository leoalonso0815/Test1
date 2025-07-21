using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class EventTriggerListenerExtensions
{
    public static EventTriggerListener Listener(this MaskableGraphic target)
    {
        var listener = target.GetComponent<EventTriggerListener>();
        if (listener == null) listener = target.gameObject.AddComponent<EventTriggerListener>();
        return listener;
    }
    
    public static EventTriggerListener Listener(this Transform target)
    {
        var listener = target.GetComponent<EventTriggerListener>();
        if (listener == null) listener = target.gameObject.AddComponent<EventTriggerListener>();
        return listener;
    }

    public static void InvokeExecute(this Toggle toggle, bool isOn = true)
    {
        if (toggle.isOn == isOn)
        {
            toggle.isOn = !isOn;
            toggle.isOn = isOn;
        }
        else
        {
            toggle.isOn = isOn;
        }
    }

    public static void InvokeExecute(this ToggleGroup toggleGroup, Toggle toggle)
    {
        toggleGroup.NotifyToggleOn(toggle);
    }

    /// <summary>
    /// RemoveAll 并且 AddListener
    /// </summary>
    /// <param name="e"></param>
    /// <param name="call"></param>
    public static void AddNewListener(this Button.ButtonClickedEvent e, UnityAction call)
    {
        e.RemoveAllListeners();
        e.AddListener(call);
    }

    /// <summary>
    /// RemoveAll 并且 AddListener
    /// </summary>
    /// <param name="e"></param>
    /// <param name="call"></param>
    public static void Interactable(this Button btn, bool interactable, Action<bool> onInteractableChanged)
    {
        btn.interactable = interactable;
        if (null != onInteractableChanged)
        {
            onInteractableChanged(interactable);
        }
    }
}