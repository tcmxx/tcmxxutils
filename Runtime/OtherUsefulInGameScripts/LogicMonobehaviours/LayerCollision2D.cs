using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace TCUtils
{
    [Serializable]
    public class CollisionEvent2D : UnityEvent<Collision2D> { }

    public class LayerCollision2D : MonoBehaviour
    {

        public LayerMask triggerLayer;
        [NaughtyAttributes.Tag]
        public string requiredTag;
        public List<CollisionEventEntrance> events;


        public bool oneWay = false; //if true, only object enter/exit from local positive y will be triggered

        [SerializableAttribute]
        public struct CollisionEventEntrance
        {
            public CollisionEventType eventType;
            public CollisionEvent2D unityEvent;
        }

        public enum CollisionEventType
        {
            Enter,
            Stay,
            Exit
        }

        public void AddEvent(CollisionEventEntrance eventToAdd)
        {

        }

        void OnCollisionEnter2D(Collision2D col)
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
                TriggerEvents(CollisionEventType.Enter, col);
            }
        }

        void OnCollisionStay2D(Collision2D col)
        {
            if (oneWay)
            {
                if (transform.InverseTransformPoint(col.transform.position).y < 0)
                {
                    return;
                }
            }
            if (CheckCondition(col))
                TriggerEvents(CollisionEventType.Stay, col);
        }

        void OnCollisionExit2D(Collision2D col)
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
                TriggerEvents(CollisionEventType.Exit, col);
            }
        }


        private void TriggerEvents(CollisionEventType type, Collision2D col)
        {
            foreach (var f in events)
            {
                if (f.eventType == type)
                {
                    f.unityEvent.Invoke(col);
                }
            }
        }


        private bool CheckCondition(Collision2D col)
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
