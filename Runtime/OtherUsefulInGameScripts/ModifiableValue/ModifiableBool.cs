using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    [Serializable]
    public class ModifiableBool : ModifiableValue<bool>
    {
        public ModifiableBool(bool baseValue) : base(baseValue)
        {

        }

        #region modifier builders
        public static Func<bool, bool, bool, bool> setFunc = (arg, baseValue, beforeModify)=> arg;
        public static Func<bool, bool, bool, bool> orFunc = (arg, baseValue, beforeModify) => arg || beforeModify;
        public static Func<bool, bool, bool, bool> andFunc = (arg, baseValue, beforeModify) => arg && beforeModify;
        public static Func<bool, bool, bool> negFunc = (baseValue, beforeModify) => !beforeModify;

        public static ValueModifier<bool> Set(string displayName, bool toSet)
        {
            return new FuncValueModifier<bool, bool>(displayName, toSet, setFunc);
        }

        public static ValueModifier<bool> Or(string displayName, bool toOr)
        {
            return new FuncValueModifier<bool, bool>(displayName, toOr, orFunc);
        }

        public static ValueModifier<bool> And(string displayName, bool toAnd)
        {
            return new FuncValueModifier<bool, bool>(displayName, toAnd, andFunc);
        }

        public static ValueModifier<bool> Negate(string displayName)
        {
            return new FuncValueModifier<bool>(displayName, negFunc);
        }
        #endregion

    }
}