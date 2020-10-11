using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

namespace TCUtils
{

    public class QueuePool<T> : ObjectPool<Queue<T>>
    {
        protected override Queue<T> CreateInstance()
        {
            return new Queue<T>();
        }

        protected override void BeforeRelease(Queue<T> obj)
        {
            obj.Clear();
        }
    }

    public static class StaticQueuePool<T>
    {
        private static QueuePool<T> pool = new QueuePool<T>();

        public static Queue<T> Get()
        {
            return pool.Get();
        }

        public static void Release(Queue<T> obj)
        {
            pool.Release(obj);
        }
    }

    public class SimplePriorityQueuePool<T> : ObjectPool<SimplePriorityQueue<T>>
    {
        protected override SimplePriorityQueue<T> CreateInstance()
        {
            return new SimplePriorityQueue<T>();
        }

        protected override void BeforeRelease(SimplePriorityQueue<T> obj)
        {
            obj.Clear();
        }
    }

    public static class StaticSimplePriorityQueuePool<T>
    {
        private static SimplePriorityQueuePool<T> pool = new SimplePriorityQueuePool<T>();

        public static SimplePriorityQueue<T> Get()
        {
            return pool.Get();
        }

        public static void Release(SimplePriorityQueue<T> obj)
        {
            pool.Release(obj);
        }
    }
}
