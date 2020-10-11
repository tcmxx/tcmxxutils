using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    [System.Serializable]
    public struct SimpleSerializableValue
    {
        public enum ValueType
        {
            Integer,
            Float,
            Boolean
        }

        public ValueType type;
        public float value;

        public float GetFloatValue()
        {
            return value;
        }

        public int GetIntValue()
        {
            return Mathf.RoundToInt(value);
        }

        public bool GetBooleanValue()
        {
            return value > 0;
        }

        public void SetValue(float floatValue)
        {
            value = floatValue;
        }

        public void SetValue(int intValue)
        {
            value = intValue;
        }

        public void SetValue(bool boolValue)
        {
            value = boolValue ? 1 : 0;
        }
        
    }
}