using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveEventsManager {

    public static SaveEventsManager Instance {
        get {
            if (instance == null) {
                instance = new SaveEventsManager();
            }

            return instance;
        }
    }

    private static SaveEventsManager instance = null;

    protected List<string> occuredEvents; //value is the list of occured event sin that scene. NOte that the events are ordered based on their occurring time

    public SaveEventsManager() {
        occuredEvents = new List<string>();
    }

    public void LoadDataFromSave(List<string> items) {
        if (items == null)
            occuredEvents = new List<string>();
        else
            occuredEvents = new List<string>(items);
    }

    public List<string> GetSaveData() {
        var result = new List<string>(occuredEvents);
        return result;
    }

    /// <summary>
    /// Checks the event status.
    /// </summary>
    /// <returns><c>true</c>, if event was finished, <c>false</c> otherwise.</returns>
    /// <param name="eventID">Event I.</param>
    public bool CheckEventOccured(string eventID) {
        return occuredEvents.Contains(eventID);
    }

    public void SetEventOccured(string eventID) {
        if (CheckEventOccured(eventID))
            return;
        occuredEvents.Add(eventID);

    }

    public void InitializeEventsInScene(string sceneName) {
        var allEvents = new Dictionary<string, SaveEventObject>();

        var events = UnityEngine.Object.FindObjectsOfType<SaveEventObject>();
        foreach (var e in events) {
            string id = e.EventID;
            if (allEvents.ContainsKey(id))
                Debug.LogWarning("even ID: " + id + "already exist!");
            allEvents[id] = e;
        }

        foreach (var t in occuredEvents) {
            if (allEvents.TryGetValue(t, out var eventObj)) {
                eventObj.LoadOccurred();
            }
        }
    }

}