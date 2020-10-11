using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{

    public class ListPool<T> : ObjectPool<List<T>>
    {
        protected override List<T> CreateInstance()
        {
            return new List<T>();
        }

        protected override void BeforeRelease(List<T> obj)
        {
            obj.Clear();
        }
    }

    public static class StaticListPool<T>
    {
        private static ListPool<T> pool = new ListPool<T>();

        public static List<T> Get()
        {
            return pool.Get();
        }

        public static void Release(List<T> obj)
        {
            pool.Release(obj);
        }
    }
}
