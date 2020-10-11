using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneOkGame.NCP
{
    public class AndCondition : ConditionBase
    {
        public ConditionBase[] andConditions;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            foreach(var c in andConditions)
            {
                c.onSatisfied.AddListener(OnConditionChanged);
                c.onUnsatisfied.AddListener(OnConditionChanged);
            }
        }

        private void OnConditionChanged()
        {
            bool satisfied = true;
            foreach(var c in andConditions)
            {
                if (c.Status != ConditionStatus.Satisfied)
                    satisfied = false;
            }

            UpdateSatisfaction(satisfied);
        }
    }
}