using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    //singleton pool will be destroyed on load and release all resources
    public class ObjectPoolSingleton<P, T> : SingletonBaseBehaviour<ObjectPoolSingleton<P, T>> where P: ObjectPool<T>, new() where T : class
    {
        public override bool DestroyOnload
        {
            get { return false; }
        }

        private P pool;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            pool = new P();
        }

        public T Get()
        {
            return pool.Get();
        }

        public void Release(T obj)
        {
            pool.Release(obj);
        }
    }
}