using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class TriggerEvent : UnityEvent<Collider> { }

public class LayerTrigger : MonoBehaviour {


    
	public LayerMask triggerLayer;

	public TriggerEventEntrance[] events;

    
    public bool oneWay = false; //if true, only object enter/exit from local positive y will be triggered

	[SerializableAttribute]
	public struct TriggerEventEntrance
    {
		public TriggerEventType eventType;
		public TriggerEvent unityEvent;
	}

	public enum TriggerEventType{
		Enter,
		Stay,
		Exit
	}

	void OnTriggerEnter(Collider col){

        if (oneWay)
        {
            if(transform.InverseTransformPoint(col.transform.position).y < 0)
            {
                return;
            }
        }

        if ((1 << col.gameObject.layer & triggerLayer.value) != 0) {
			TriggerEvents (TriggerEventType.Enter, col);
		}
	}

	void OnTriggerStay(Collider col){
        if (oneWay)
        {
            if (transform.InverseTransformPoint(col.transform.position).y < 0)
            {
                return;
            }
        }
        if ((1<<col.gameObject.layer & triggerLayer.value) != 0)
			TriggerEvents (TriggerEventType.Stay, col);
	}

	void OnTriggerExit(Collider col){
        if (oneWay)
        {
            if (transform.InverseTransformPoint(col.transform.position).y < 0)
            {
                return;
            }
        }
        if ((1 << col.gameObject.layer & triggerLayer.value) != 0) {
			TriggerEvents (TriggerEventType.Exit, col);
		}
	}


	private void TriggerEvents(TriggerEventType type, Collider col){
		foreach (var f in events) {
			if (f.eventType == type) {
				f.unityEvent.Invoke (col);
			}
		}
	}

   


    private void OnDrawGizmosSelected()
    {
        if (oneWay)
        {
            Vector3 localY = transform.TransformDirection(Vector3.up);
            Vector3 localX1 = transform.TransformDirection(Vector3.up + Vector3.right);
            Vector3 localX2 = transform.TransformDirection(Vector3.up - Vector3.right);
            Gizmos.DrawLine(transform.position, transform.position + localY);
            Gizmos.DrawLine(transform.position, transform.position + localX1/2);
            Gizmos.DrawLine(transform.position, transform.position + localX2/2);
        }
    }

}
