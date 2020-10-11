using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessengerTrigger : MonoBehaviour
{
    [SerializeField]
    protected string message;
    [SerializeField]
    protected UnityEvent onMessageReceived;
    [SerializeField]
    protected bool validWhenEnabledOnly = false;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(message) && !validWhenEnabledOnly)
            Messenger.Messenger.AddListener(message, OnMessage);
    }
    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(message) && validWhenEnabledOnly)
            Messenger.Messenger.AddListener(message, OnMessage);
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(message) && validWhenEnabledOnly)
            Messenger.Messenger.RemoveListener(message, OnMessage);
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(message) && !validWhenEnabledOnly)
            Messenger.Messenger.RemoveListener(message, OnMessage);
    }

    private void OnMessage()
    {
        if(enabled)
            onMessageReceived.Invoke();
    }

    public void SendMessage()
    {
        Messenger.Messenger.Broadcast(message);
    }


}
