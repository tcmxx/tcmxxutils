using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    [Serializable]
    public class ModifiableInt : ModifiableValue<int>
    {
        public ModifiableInt(int baseValue) : base(baseValue)
        {

        }

        #region modifier builders
        public static Func<int, int, int, int> addFunc = (arg,baseValue, beforeModify)=> arg + beforeModify;
        public static Func<float, int, int, int> multiplyFunc = (arg, baseValue, beforeModify) => (int)(arg * beforeModify);
        public static Func<float, int, int, int> addMultipliedFunc = (arg, baseValue, beforeModify) => (int)(beforeModify + arg * baseValue);
        public static Func<int, int, int, int> setFunc = (arg, baseValue, beforeModify) => arg;

        public static ValueModifier<int> Add(string displayName, int toAdd)
        {
            return new FuncValueModifier<int, int>(displayName, toAdd, addFunc);
        }

        public static ValueModifier<int> Multiply(string displayName, float toMultiplay)
        {
            return new FuncValueModifier<float, int>(displayName, toMultiplay, multiplyFunc);
        }

        public static ValueModifier<int> AddBaseMultiplied(string displayName, float basetoMultiplyAdd)
        {
            return new FuncValueModifier<float, int>(displayName, basetoMultiplyAdd, addMultipliedFunc);
        }

        public static ValueModifier<int> Set(string displayName, int toSet)
        {
            return new FuncValueModifier<int, int>(displayName, toSet, setFunc);
        }
        #endregion

    }
}