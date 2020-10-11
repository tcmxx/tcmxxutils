using System.Linq;
using UnityEngine;


namespace TCUtils
{
    /// <summary>
    /// Abstract class for making reload-proof singletons out of ScriptableObjects
    /// Returns the asset created on the editor. Make sure the asset name is the same as the type name
    /// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
    /// </summary>
    /// <typeparam name="T">Singleton type</typeparam>

    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance)
                        (_instance as SingletonScriptableObject<T>).OnInitialized();
                }

                if (!_instance)
                {
                    _instance = Resources.Load<T>(typeof(T).Name);
                    if (_instance)
                        (_instance as SingletonScriptableObject<T>).OnInitialized();
                }

                return _instance;
            }
        }
        
        protected virtual void OnInitialized()
        {

        }
    }
}