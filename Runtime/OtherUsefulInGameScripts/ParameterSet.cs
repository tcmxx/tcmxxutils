using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{

    //use those to prevent boxing when setting the parameter of value type
    internal abstract class BoxedType : ICloneable
    {
        public bool valid = false;

        public abstract object Clone();
    }

    internal class BoxedType<T> : BoxedType
    {
        public T value;

        public BoxedType(T value)
        {
            this.value = value;
            valid = true;
        }

        public void SetValue(T value)
        {
            this.value = value;
            valid = true;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class ParameterSet<TKey>
    {
        private Dictionary<TKey, object> dic = new Dictionary<TKey, object>();
        public MessengerLight<TKey> ValueChangeEvents { get; private set; } = new MessengerLight<TKey>();   //all events should be non-argument Action

        public virtual void Clear()
        {
            dic.Clear();
            ValueChangeEvents.Cleanup();
        }

        public virtual bool CheckParameterType<TValue>(TKey key, TValue value)
        {
            return true;
        }

        //add the parameter of type, with input value an object.
        //it will treat the value as a value type if the input type parameter is of a value type. This might be slow and creating garbage

        public void AddOrSetParameter(TKey key, object value, Type type)
        {
            if (!type.IsValueType)
            {
                AddOrSetParameter(key, value);
            }
            else
            {
                Type savedContainerType = typeof(BoxedType<>).MakeGenericType(type);
                if (!dic.ContainsKey(key))
                {
                    dic[key] = Activator.CreateInstance(savedContainerType, value);
                }
                else
                {
                    var obj = dic[key];
                    var actualType = obj.GetType();
                    if (savedContainerType.IsAssignableFrom(actualType))
                    {
                        //already is the type container
                        savedContainerType.GetMethod("SetValue").Invoke(obj, new[] { value });
                    }
                    else
                    {
                        dic[key] = Activator.CreateInstance(type, value);
                    }
                }

                ValueChangeEvents.Broadcast(key);
            }
        }

        public void AddOrSetParameter<TValue>(TKey key, TValue value)
        {
            if (!CheckParameterType(key, value))
                return;
            if (!typeof(TValue).IsValueType)
            {
                dic[key] = value;
            }
            else if(!dic.ContainsKey(key))
            {
                dic[key] = new BoxedType<TValue>(value);
            }
            else
            {
                var obj = dic[key] as BoxedType<TValue>;
                if(obj != null)
                {
                    obj.SetValue(value);
                }
                else
                {
                    dic[key] = new BoxedType<TValue>(value);
                }
            }

            ValueChangeEvents.Broadcast(key);
        }

        //set the parameters which existing in the other BattleParameterSet
        public void SetParameters(ParameterSet<TKey> other)
        {
            foreach(var p in other.dic)
            {
                var boxedType = p.Value as BoxedType;
                if (boxedType == null)
                {
                    dic[p.Key] = p.Value;
                    ValueChangeEvents.Broadcast(p.Key);
                }
                else
                {
                    dic[p.Key] = boxedType.Clone();
                    ValueChangeEvents.Broadcast(p.Key);
                }
            }
        }

        public void RemoveParameter(TKey key)
        {
            object parameterValue;

            if(dic.TryGetValue(key, out parameterValue))
            {
                var v = parameterValue as BoxedType;
                if (v != null)
                {
                    v.valid = false;
                }
                else
                {
                    dic.Remove(key);
                }
            }
        }

        public virtual bool TryGetParamter<TValue>(TKey key, out TValue output)
        {
            object originalValue;
            if(dic.TryGetValue(key, out originalValue))
            {
                if(originalValue is TValue)
                {
                    output = (TValue)originalValue;
                    return true;
                }

                var boxedValue = originalValue as BoxedType<TValue>;
                if(boxedValue != null && boxedValue.valid)
                {
                    output = boxedValue.value;
                    return true;
                }
            }
            output = default;
            return false;
        }

        public TValue GetParamter<TValue>(TKey key, TValue defaultValue = default(TValue))
        {
            TValue result;
            bool hasResult = TryGetParamter(key, out result);
            if (hasResult)
                return result;
            else
                return defaultValue;
        }
    }
}