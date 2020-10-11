using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    
    public abstract class ModifiableValue<T>
    {
        public IReadOnlyList<ValueModifier<T>> Modifiers { get { return modifiers; } }
        protected List<ValueModifier<T>> modifiers = new List<ValueModifier<T>>();

        public T BaseValue
        {
            get
            {
                return baseValue;
            }
            set
            {
                baseValue = value;
                ApplyModifiers();
            }

        }
        [SerializeField]
        protected T baseValue;

        public T FinalValue { get; protected set; }

        public ModifiableValue(T baseValue){
            BaseValue = baseValue;
        }

        public void ApplyModifiers()
        {
            FinalValue = BaseValue;
            foreach (var m in Modifiers)
            {
                FinalValue = m.ApplyModify(BaseValue, FinalValue);
            }
        }

        public void AddModifier(ValueModifier<T> modifier)
        {
            modifiers.Add(modifier);
            ApplyModifiers();
        }

        //remove the modifier and return whether this modifier is found or not.
        public bool RemoveModifier(ValueModifier<T> modifier)
        {
            if (modifiers.Remove(modifier))
            {
                ApplyModifiers();
                return true;
            }
            return false;
        }

        public void RemoveModifierAt(int index)
        {
            modifiers.RemoveAt(index);
            ApplyModifiers();
        }

        public void InsertModifier(int index, ValueModifier<T> modifier)
        {
            modifiers.Insert(index, modifier);
            ApplyModifiers();
        }
    }

    public abstract class ValueModifier<T>
    {
        public string DisplayName { get; protected set; }
    
        public ValueModifier(string displayName)
        {
            DisplayName = displayName;
        }

        public abstract T ApplyModify(T baseValue, T beforeModifyValue);
    }

    public class FuncValueModifier<T> : ValueModifier<T>
    {
        Func<T, T, T> ModifyFunc { get; set; }

        public FuncValueModifier(string displayName, Func<T, T, T> modifyFunc) : base(displayName)
        {
            ModifyFunc = modifyFunc;
        }

        public override T ApplyModify(T baseValue, T beforeModifyValue)
        {
            return ModifyFunc(baseValue, beforeModifyValue);
        }
    }

    public class FuncValueModifier<TModifyArg, T> : ValueModifier<T>
    {
        public TModifyArg Arg { get; set; }
        Func<TModifyArg, T, T, T> ModifyFunc  { get; set; }

        public FuncValueModifier(string displayName, TModifyArg arg, Func<TModifyArg, T, T, T> modifyFunc): base(displayName)
        {
            ModifyFunc = modifyFunc;
            Arg = arg;
        }

        public override T ApplyModify(T baseValue, T beforeModifyValue)
        {
            return ModifyFunc(Arg, baseValue, beforeModifyValue);
        }
    }

}