using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviourEventsTrigger : MonoBehaviour
{
    public UnityEvent awake;
    public UnityEvent start;
    public UnityEvent onEnable;
    public UnityEvent onDisable;
    public UnityEvent onDestroy;

    public bool EnableTriggeres { get; set; } = true;

    private void Awake()
    {
        if(EnableTriggeres)
            awake.Invoke();
    }

    private void Start()
    {
        if (EnableTriggeres)
            start.Invoke();
    }

    private void OnEnable()
    {
        if (EnableTriggeres)
            onEnable.Invoke();
    }

    private void OnDisable()
    {
        if (EnableTriggeres)
            onDisable.Invoke();
    }

    private void OnDestroy()
    {
        onDestroy.Invoke();
    }

}
