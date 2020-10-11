using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{

    public class DictionaryPool<TKey,TValue> : ObjectPool<Dictionary<TKey, TValue>>
    {
        protected override Dictionary<TKey, TValue> CreateInstance()
        {
            return new Dictionary<TKey, TValue>();
        }

        protected override void BeforeRelease(Dictionary<TKey, TValue> obj)
        {
            obj.Clear();
        }
    }

    public static class StaticDictionaryPool<TKey, TValue>
    {
        private static DictionaryPool<TKey, TValue> pool = new DictionaryPool<TKey, TValue>();

        public static Dictionary<TKey, TValue> Get()
        {
            return pool.Get();
        }

        public static void Release(Dictionary<TKey, TValue> obj)
        {
            pool.Release(obj);
        }
    }
}
