using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerInteractable : MonoBehaviour
{
    public Action TriggerAction;
    public UnityEvent ExternalAction;

    public void Trigger()
    {
        TriggerAction?.Invoke();
        ExternalAction?.Invoke();
    }
}
