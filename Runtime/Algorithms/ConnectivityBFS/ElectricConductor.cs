using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElectricConductor : MonoBehaviour, IConnectivityNode
{
    [SerializeField]
    protected bool isSource = false;

    public UnityEvent onOn;
    public UnityEvent onOff;

    public AudioClip onClip;
    public AudioClip offClip;

    public ConductorGroup Group { get; set; }

    protected List<IConnectivityNode> neigbors = new List<IConnectivityNode>();

    protected bool isOnPrev = false;

    public bool IsOn { get {
            return isSource || Group.IsOn;
        } }

    private void Start()
    {
        Group = new ConductorGroup(new List<ElectricConductor>(){ this});
        isOnPrev = IsOn;
    }
    private void Update()
    {
        Group.ResolveIfDirty();
        bool newIsOn = IsOn;
        if(newIsOn != isOnPrev)
        {
            if (newIsOn)
                OnOn();
            else
                OnOff();
            isOnPrev = newIsOn;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //RegisterIfContainSource(collision.gameObject);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //UnregisterIfContainSource(collision.gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        RegisterIfContainSource(collision.gameObject);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        UnregisterIfContainSource(collision.gameObject);
    }
    
    protected void RegisterIfContainSource(GameObject go)
    {
        ElectricConductor conductor = go.GetComponent<ElectricConductor>();
        if (conductor != null && !neigbors.Contains(conductor))
        {
            neigbors.Add(conductor);
            Group.SetDirty();
        }
    }
    protected void UnregisterIfContainSource(GameObject go)
    {
        ElectricConductor conductor = go.GetComponent<ElectricConductor>();
        if (conductor != null && neigbors.Contains(conductor))
        {
            neigbors.Remove(conductor);
            Group.SetDirty();
        }
    }

    protected void OnOn()
    {
        //Debug.Log("IsOnChanged: On");
        if(onClip != null)
            AudioSource.PlayClipAtPoint(onClip, transform.position);
        onOn.Invoke();
    }

    protected void OnOff()
    {
        //Debug.Log("IsOnChanged: Off");
        if(offClip != null)
            AudioSource.PlayClipAtPoint(offClip, transform.position);
        onOff.Invoke();   
    }

    public List<IConnectivityNode> GetNeighbors()
    {
        return neigbors;
    }

    public bool IsAvailable()
    {
        return this != null && gameObject != null;
    }

    public uint GetAndPropertyBits()
    {
        if (isSource)
            return 0;
        else
            return 1;
    }
}
