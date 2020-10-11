using UnityEngine;

namespace TCUtils
{

    ///<summary>
    ///base class for singleton that get created from Resource prefab. The prefab name should be the same as type T.
    ///
    ///</summary>
    public class SingletonFromResource<T> : MonoBehaviour
        where T : MonoBehaviour
    {

        public virtual bool AllowCreateOnDemand
        {
            get { return true; }
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    var instanceInScene = FindObjectOfType<SingletonFromResource<T>>();


                    // If no instance has been found in scene, create one.
                    if (instanceInScene == null)
                    {
                        instanceInScene = CreateInstanceInScene();

                        //destroy it if not allowed to create on demand
                        if (!instanceInScene.AllowCreateOnDemand)
                        {
                            Destroy(instanceInScene.gameObject);
                            instance = null;
                            return null;
                        }
                    }

                    if(initialized == false)
                    {
                        instanceInScene.OnInitialize();
                        initialized = true;
                    }

                    instance = instanceInScene as T;
                }

                return instance;
            }
        }


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


        private static SingletonFromResource<T> CreateInstanceInScene()
        {
            var inst = Instantiate(Resources.Load<GameObject>(typeof(T).Name)).GetComponent<T>();
            return inst as SingletonFromResource<T>;
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