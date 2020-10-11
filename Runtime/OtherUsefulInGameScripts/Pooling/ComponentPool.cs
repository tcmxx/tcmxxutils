
using UnityEngine;

namespace TCUtils
{
    public class ComponentPool<T> : ObjectPool<T> where T : Component
    {
        public T Prefab { get; set; } = null;

        protected override T CreateInstance()
        {
            if (Prefab == null)
            {
                var go = new GameObject("PoolObject");
                return go.AddComponent<T>();
            }
            else
            {
                return Object.Instantiate(Prefab);
            }
        }
        protected override void BeforeRelease(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        protected override void BeforeGet(T obj)
        {
            obj.gameObject.SetActive(true);
        }

        protected override void OnClear(T obj)
        {
            if (obj == null) return;

            var go = obj.gameObject;
            if (go == null) return;
            Object.Destroy(go);
        }
    }
}