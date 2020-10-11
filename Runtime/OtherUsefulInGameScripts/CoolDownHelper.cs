using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public class CoolDownHelper
    {
        public float CoolDownTime { get; set; }
        public float CountUpTimer { get; protected set; }
        public float CoolDownPercentage
        {
            get
            {
                if (CoolDownTime <= 0) return 1;
                else return Mathf.Clamp01(CountUpTimer / CoolDownTime);
            }
        }

        public event Action onCoolDowned;

        public CoolDownHelper(float baseCoolDownTime)
        {
            CoolDownTime = baseCoolDownTime;
            CountUpTimer = 0;
        }

        public void Reset()
        {
            CountUpTimer = 0;
        }

        public void Update(float deltaTime)
        {
            if (CoolDownPercentage < 1)
            {
                CountUpTimer += deltaTime;
                if (CoolDownPercentage >= 1)
                {
                    onCoolDowned?.Invoke();
                }
            }
        }

        
    }
}
