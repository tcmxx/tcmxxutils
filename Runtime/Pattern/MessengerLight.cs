using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public class MessengerLight<TKey>
    {
        private Dictionary<TKey, Delegate> eventTable = new Dictionary<TKey, Delegate>();

        protected virtual Dictionary<TKey, Type> EventParameterTalbe { get; set; } = null;

        public void Cleanup()
        {
            eventTable.Clear();
        }

        public void PrintEventTable()
        {
            Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");

            foreach (KeyValuePair<TKey, Delegate> pair in eventTable)
            {
                Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
            }

            Debug.Log("\n");
        }

        public void OnListenerAdding(TKey eventType, Delegate listenerBeingAdded)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }

            if (EventParameterTalbe != null)
            {
                //use the event parameter table to verify the datatype if there is
                if (listenerBeingAdded.GetType() != EventParameterTalbe[eventType])
                {
                    throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}.Listeners should have type {1} and listener being added has type {2}", eventType, EventParameterTalbe[eventType], listenerBeingAdded.GetType().Name));
                }
            }
            else
            {
                //use existing listeneres to verify the datatype
                Delegate d = eventTable[eventType];
                if (d != null && d.GetType() != listenerBeingAdded.GetType())
                {
                    throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
                }
            }
        }

        public void OnListenerRemoving(TKey eventType, Delegate listenerBeingRemoved)
        {
            if (eventTable.ContainsKey(eventType))
            {
                Delegate d = eventTable[eventType];

                if (d == null)
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
            }
        }

        public void OnListenerRemoved(TKey eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        static public BroadcastException CreateBroadcastSignatureException(TKey eventType)
        {
            return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
        }

        public class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        public class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }

        public void AddListener<T>(TKey eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
        }

        public void AddListener(TKey eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            eventTable[eventType] = (Action)eventTable[eventType] + handler;
        }

        public void RemoveListener<T>(TKey eventType, Action<T> handler)
        {
            OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        public void RemoveListener(TKey eventType, Action handler)
        {
            OnListenerRemoving(eventType, handler);
            eventTable[eventType] = (Action)eventTable[eventType] - handler;
            OnListenerRemoved(eventType);
        }

        public void Broadcast<T>(TKey eventType, T arg1)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<T> Action = d as Action<T>;

                if (Action != null)
                {
                    Action(arg1);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        public void Broadcast(TKey eventType)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action Action = d as Action;

                if (Action != null)
                {
                    Action();
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

    }
}