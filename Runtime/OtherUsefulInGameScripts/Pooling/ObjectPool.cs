using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    //adapted from UniRx.
    public abstract class ObjectPool<T>: IDisposable where T: class
    {
        bool isDisposed = false;
        protected Queue<T> queue = new Queue<T>();
        /// <summary>
        /// Limit of instace count.
        /// </summary>
        protected int MaxPoolCount
        {
            get
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// Current pooled object count.
        /// </summary>
        public int Count
        {
            get
            {
                if (queue == null) return 0;
                return queue.Count;
            }
        }

        protected abstract T CreateInstance();
        protected virtual void BeforeRelease(T obj) { }
        protected virtual void BeforeGet(T obj) { }
        protected virtual void OnClear(T obj) { }

        public T Get()
        {
            if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (queue == null) queue = new Queue<T>();

            var instance = (queue.Count > 0)
                ? queue.Dequeue()
                : CreateInstance();

            BeforeGet(instance);
            return instance;
        }

        public void Release(T obj)
        {
            if (isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (obj == null) throw new ArgumentNullException("obj");

            if (queue == null) queue = new Queue<T>();

            if ((queue.Count + 1) == MaxPoolCount)
            {
                throw new InvalidOperationException("Reached Max PoolSize");
            }

            BeforeRelease(obj);
            queue.Enqueue(obj);
        }



        /// <summary>
        /// Clear pool.
        /// </summary>
        public void Clear(bool callOnBeforeRent = false)
        {
            if (queue == null) return;
            while (queue.Count != 0)
            {
                var instance = queue.Dequeue();
                if (callOnBeforeRent)
                {
                    BeforeGet(instance);
                }
                OnClear(instance);
            }
        }

        /// <summary>
        /// Trim pool instances. 
        /// </summary>
        /// <param name="instanceCountRatio">0.0f = clear all ~ 1.0f = live all.</param>
        /// <param name="minSize">Min pool count.</param>
        /// <param name="callOnBeforeRent">If true, call BeforeGet before OnClear.</param>
        public void Shrink(float instanceCountRatio, int minSize, bool callBeforeGet = false)
        {
            if (queue == null) return;

            if (instanceCountRatio <= 0) instanceCountRatio = 0;
            if (instanceCountRatio >= 1.0f) instanceCountRatio = 1.0f;

            var size = (int)(queue.Count * instanceCountRatio);
            size = Math.Max(minSize, size);

            while (queue.Count > size)
            {
                var instance = queue.Dequeue();
                if (callBeforeGet)
                {
                    BeforeGet(instance);
                }
                OnClear(instance);
            }
        }


        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Clear(false);
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}