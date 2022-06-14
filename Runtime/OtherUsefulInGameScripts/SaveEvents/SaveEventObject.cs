using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueIDComponent))]
public class SaveEventObject : MonoBehaviour {

    public string EventID {
        get {
            if (!string.IsNullOrEmpty(overrideEventID))
                return overrideEventID;
            if (uniqueIDComponent == null) {
                uniqueIDComponent = GetComponent<UniqueIDComponent>();
            }

            return uniqueIDComponent.ID;
        }
    }

    [SerializeField]
    public string overrideEventID; //used if this event is a multiscene event

    public UnityEvent onLoadOccurred;

    private UniqueIDComponent uniqueIDComponent;

    public virtual void LoadOccurred() {
        onLoadOccurred.Invoke();
    }

    public void SetEventOccured() {
        SaveEventsManager.Instance.SetEventOccured(EventID);
    }
}