using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NumberTrigger : MonoBehaviour
{
    public int currentTriggerNum = 0;
    public List<TriggerEntry> triggers = new List<TriggerEntry>();

    [System.Serializable]
    public class TriggerEntry
    {
        public int requiredTriggerNum = 1;
        public UnityEvent onTriggered;
        public UnityEvent onUntriggered;
    }
    protected virtual void OnTriggered(int newNumber)
    {

    }
    protected virtual void OnUntriggered(int oldNumber)
    {

    }

    public void AddNumber(int number)
    {
        if (number == 0)
            return;

        var oldNumber = currentTriggerNum;
        currentTriggerNum += number;

        foreach (var t in triggers)
        {
            if (oldNumber == t.requiredTriggerNum)
            {
                t.onUntriggered.Invoke();
            }
            else if (currentTriggerNum == t.requiredTriggerNum)
            {
                t.onTriggered.Invoke();
            }
        }

        OnUntriggered(oldNumber);
        OnTriggered(currentTriggerNum);
    }
}
