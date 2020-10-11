using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TCUtils
{
    public class SwitchTrigger : MonoBehaviour
    {
        public UnityEvent onTurnOn;
        public UnityEvent onTurnOff;

        public bool startOn = false;
        public bool IsActive { get { return isActive; } set { isActive = value; } }
        [SerializeField]
        protected bool isActive = true;
        public bool invokeRepeat = false;
        public bool IsOn { get { return isOn; } private set { isOn = value; } }
        [ReadOnly]
        [SerializeField]
        protected bool isOn;


        private void Start()
        {
            IsOn = startOn;
        }

        public void Switch()
        {
            if (!IsActive)
                return;
            IsOn = !IsOn;
            (IsOn ? onTurnOn : onTurnOff).Invoke();
        }

        public void Switch(bool on)
        {
            if (!IsActive)
                return;
            if (IsOn != on || invokeRepeat)
                (on ? onTurnOn : onTurnOff).Invoke();
            IsOn = on;

        }
    }
}