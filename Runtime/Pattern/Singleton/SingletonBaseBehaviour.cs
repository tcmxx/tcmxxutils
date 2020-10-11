using UnityEngine;

namespace TCUtils
{

    ///<summary>
    ///base class for singleton 
    ///
    ///</summary>
    public abstract class SingletonBaseBehaviour<T> : MonoBehaviour
        where T : MonoBehaviour
    {

        public virtual bool AllowCreateOnDemand
        {
            get { return true; }
        }

        public virtual bool DestroyOnload
        {
            get { return false; }
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    SingletonBaseBehaviour<T> instanceInScene = null;

                    instanceInScene = CreateInstanceInScene();

                    //destroy it if not allowed to create on demand
                    if (!instanceInScene.AllowCreateOnDemand)
                    {
                        Debug.LogError($"{typeof(T).ToString()} is not allowed to create on demand! Destroy it!");
                        Destroy(instanceInScene.gameObject);
                        instance = null;
                        return null;
                    }


                    if (initialized == false)
                    {
                        instanceInScene.OnInitialize();
                        initialized = true;
                    }

                    instance = instanceInScene as T;
                }

                return instance;
            }
        }

        public static bool HasInstance { get => instance != null; }

        private static T instance = null;
        private static bool initialized = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;

                if (initialized == false)
                {
                    OnInitialize();
                    initialized = true;
                }
                if (!DestroyOnload)
                    DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        //called when intiializeed or the instance it created. Called before OnAwake()
        protected virtual void OnInitialize()
        {

        }

        //use this instead of OnAwake
        protected virtual void OnAwake()
        {

        }

        //use this instead of OnDestroyed
        protected virtual void OnDestroyMessage()
        {

        }


        private static SingletonBaseBehaviour<T> CreateInstanceInScene()
        {
            // Use reflection if we are in Editor to set name
            string gameObjectName =
#if UNITY_EDITOR
            typeof(T).FullName + "_Singleton";
#else
			"SingletonInstance";
#endif

            GameObject obj = new GameObject(gameObjectName);

            var inst = obj.AddComponent<T>() as SingletonBaseBehaviour<T>;
            return inst;
        }


        private void OnDestroy()
        {
            if (instance == (this as T))
            {
                NotifyInstanceDestroy();
            }

            OnDestroyMessage();
        }

        private void NotifyInstanceDestroy()
        {
            instance = null;
            initialized = false;
        }

    }

}