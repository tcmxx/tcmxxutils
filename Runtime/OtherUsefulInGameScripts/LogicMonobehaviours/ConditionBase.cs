using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OneOkGame.NCP
{
    public class ConditionBase : MonoBehaviour
    {
        public UnityEvent onSatisfied;
        public UnityEvent onUnsatisfied;

        public enum ConditionStatus
        {
            Unknown,
            Satisfied,
            Unsatisfied
        }

        public ConditionStatus Status { get; private set; } = ConditionBase.ConditionStatus.Unknown;

        public void UpdateSatisfaction(bool satisfied)
        {
            UpdateSatisfaction(satisfied, true);
        }

        //call this whenever your condition has been changed!
        protected void UpdateSatisfaction(bool satisfied, bool invokeEvent)
        {
            if(Status == ConditionStatus.Unknown)
            {
                Status = satisfied ? ConditionStatus.Satisfied : ConditionStatus.Unsatisfied;
            }
            else if(satisfied && Status == ConditionStatus.Unsatisfied)
            {
                Status = ConditionStatus.Satisfied;
            }
            else if(!satisfied && Status == ConditionStatus.Satisfied)
            {
                Status = ConditionStatus.Unsatisfied;
            }
            else
            {
                invokeEvent = false;
            }

            if (invokeEvent)
            {
                if (Status == ConditionStatus.Satisfied)
                    onSatisfied.Invoke();
                else
                    onUnsatisfied.Invoke();
            }
        }
    }
}