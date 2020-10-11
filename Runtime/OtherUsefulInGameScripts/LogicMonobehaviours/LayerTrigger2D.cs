using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace TCUtils
{
    [Serializable]
    public class TriggerEvent2D : UnityEvent<Collider2D> { }

    public class LayerTrigger2D : MonoBehaviour
    {

        public LayerMask triggerLayer;
        [NaughtyAttributes.Tag]
        public string requiredTag;
        public List<TriggerEventEntrance> events;


        public bool oneWay = false; //if true, only object enter/exit from local positive y will be triggered

        [SerializableAttribute]
        public struct TriggerEventEntrance
        {
            public TriggerEventType eventType;
            public TriggerEvent2D unityEvent;
        }

        public enum TriggerEventType
        {
            Enter,
            Stay,
            Exit
        }

        public void AddEvent(TriggerEventEntrance eventToAdd)
        {

        }

        void OnTriggerEnter2D(Collider2D col)
        {

            if (oneWay)
            {
                if (transform.InverseTransformPoint(col.transform.position).y < 0)
                {
                    return;
                }
            }

            if (CheckCondition(col))
            {
                TriggerEvents(TriggerEventType.Enter, col);
            }
        }

        void OnTriggerStay2D(Collider2D col)
        {
            if (oneWay)
            {
                if (transform.InverseTransformPoint(col.transform.position).y < 0)
                {
                    return;
                }
            }
            if (CheckCondition(col))
                TriggerEvents(TriggerEventType.Stay, col);
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (oneWay)
            {
                if (transform.InverseTransformPoint(col.transform.position).y < 0)
                {
                    return;
                }
            }
            if (CheckCondition(col))
            {
                TriggerEvents(TriggerEventType.Exit, col);
            }
        }


        private void TriggerEvents(TriggerEventType type, Collider2D col)
        {
            foreach (var f in events)
            {
                if (f.eventType == type)
                {
                    f.unityEvent.Invoke(col);
                }
            }
        }


        private bool CheckCondition(Collider2D col)
        {
            return (1 << col.gameObject.layer & triggerLayer.value) != 0 && (string.IsNullOrEmpty(requiredTag) || col.gameObject.CompareTag(requiredTag));
        }

        private void OnDrawGizmosSelected()
        {
            if (oneWay)
            {
                Vector3 localY = transform.TransformDirection(Vector3.up);
                Vector3 localX1 = transform.TransformDirection(Vector3.up + Vector3.right);
                Vector3 localX2 = transform.TransformDirection(Vector3.up - Vector3.right);
                Gizmos.DrawLine(transform.position, transform.position + localY);
                Gizmos.DrawLine(transform.position, transform.position + localX1 / 2);
                Gizmos.DrawLine(transform.position, transform.position + localX2 / 2);
            }
        }

    }
}