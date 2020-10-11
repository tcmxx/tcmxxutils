using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{

    [Serializable]
    public class ModifiableFloat : ModifiableValue<float>
    {
        public ModifiableFloat(float baseValue) : base(baseValue)
        {

        }

        #region modifier builders
        public static Func<float, float, float, float> addFunc = (arg,baseValue, beforeModify)=> arg + beforeModify;
        public static Func<float, float, float, float> multiplyFunc = (arg, baseValue, beforeModify) => arg * beforeModify;
        public static Func<float, float, float, float> addMultipliedFunc = (arg, baseValue, beforeModify) => beforeModify + arg * baseValue;
        public static Func<float, float, float, float> setFunc = (arg, baseValue, beforeModify) => arg;

        public static ValueModifier<float> Add(string displayName, float toAdd)
        {
            return new FuncValueModifier<float, float>(displayName, toAdd, addFunc);
        }

        public static ValueModifier<float> Multiply(string displayName, float toMultiplay)
        {
            return new FuncValueModifier<float, float>(displayName, toMultiplay, multiplyFunc);
        }

        public static ValueModifier<float> AddBaseMultiplied(string displayName, float basetoMultiplyAdd)
        {
            return new FuncValueModifier<float, float>(displayName, basetoMultiplyAdd, addMultipliedFunc);
        }

        public static ValueModifier<float> Set(string displayName, float toSet)
        {
            return new FuncValueModifier<float, float>(displayName, toSet, setFunc);
        }
        #endregion

    }
}