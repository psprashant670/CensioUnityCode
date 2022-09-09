using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public string InteractableID;
    public TriggerInteractable Trigger;
    public GameObject FX;
    public List<GameObject> Lights;
    public bool TriggerOnce = false;

    public bool isInteractable { get; set; }

    public abstract void Interact();

    public void LogValidClick()
    {
        Data.instance.ClickLog.Add(InteractableID);
        Data.instance.CurrentLevel.NoValidclicks++;
    }

    public void ShowFX(bool _state)
    {
        FX?.SetActive(_state);
    }

    public void ShowLights(bool _state)
    {
        foreach (GameObject _light in Lights)
        {
            _light.SetActive(_state);
        }
    }

    void OnEnable() 
    {
        Trigger.TriggerAction += Interact;
        // Debug.Log("Interactable");
    }

    void OnDisable() 
    {
        Trigger.TriggerAction -= Interact;
    }
}
