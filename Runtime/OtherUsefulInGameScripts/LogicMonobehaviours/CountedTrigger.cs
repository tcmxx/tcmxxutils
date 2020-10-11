using UnityEngine;
using UnityEngine.Events;

public class CountedTrigger : MonoBehaviour {

	public int requiredTriggerNum = 1;
	private int currentTriggerNum = 0;

    public bool allowRepeatTrigger = false;
    public bool clampTriggeredNumber = false;
    public bool Triggered { get; protected set; }

    public UnityEvent onTriggered;
    public UnityEvent onUntriggered;


    protected virtual void OnTriggered()
    {

    }
    protected virtual void OnUntriggered()
    {

    }

    public void AddCount(){
		currentTriggerNum++;
        if (currentTriggerNum >= requiredTriggerNum)
        {
            if (clampTriggeredNumber)
                currentTriggerNum = Mathf.Clamp(currentTriggerNum, 0, requiredTriggerNum);
            if (allowRepeatTrigger)
            {
                Triggered = true;
                onTriggered.Invoke();
                OnTriggered();
            }else if(Triggered == false)
            {
                onTriggered.Invoke();
                OnTriggered();
            }

        }
	}

    public void RemoveCount(){
		currentTriggerNum--;
        if (currentTriggerNum < requiredTriggerNum)
        {
            if (clampTriggeredNumber)
                currentTriggerNum = Mathf.Clamp(currentTriggerNum, 0, requiredTriggerNum);
            if (allowRepeatTrigger)
            {
                Triggered = false;
                onUntriggered.Invoke();
                OnUntriggered();
            }
            else if (Triggered == true)
            {
                onUntriggered.Invoke();
                OnUntriggered();
            }
        }
    }
    
}
