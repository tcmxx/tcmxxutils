using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TCUtils
{



    //The idea is from https://github.com/Siccity/SerializableCallback
    //however, this does not work with List<>. You need a container to hold the SerializableClassConstructor<TBase> for that
    [Serializable]
    public class SerializableClassConstructor<TBase> : SerializableClassConstructor where TBase: class
    {
        [SerializeField]
        protected string baseTypeString;

        private TBase objectCache = null;

        public SerializableClassConstructor():base()
        {
            UpdateBaseTypeStringIfNull();
        }

        public void UpdateBaseTypeStringIfNull()
        {
            if(string.IsNullOrEmpty(baseTypeString))
                baseTypeString = GetClassSerializedName(typeof(TBase));
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            UpdateBaseTypeStringIfNull();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            UpdateBaseTypeStringIfNull();
        }

        public TBase CreateInstance()
        {
            if (type == null)
                type = GetTypeFromName(typeName);
            if (constructor == null)
                constructor = type.GetConstructor(ArgTypes);
            if (constructor == null)
            {
                Debug.LogError("No matching constructor found for serialized class constructor of class " + typeName);
            }
             
            return constructor.Invoke(Arguments) as TBase;
        }

        public TBase GetOrCreateInstance()
        {
            if (objectCache != null)
            {
                return objectCache;
            }
            else
            {
                objectCache = CreateInstance();
                return objectCache;
            }
        }
    }


    [Serializable]
    public class SerializableClassConstructor : ISerializationCallbackReceiver
    {
        /// <summary> Target method name </summary>
        public object[] Arguments { get { return arguments != null ? arguments : arguments = serializedArguments.Select(x => x.GetValue()).ToArray(); } }
        protected object[] arguments;

        public Type[] ArgTypes { get { return argTypes != null ? argTypes : argTypes = serializedArguments.Select(x => SerializableArgument.RealType(x.argType)).ToArray(); } }
        protected Type[] argTypes;

        [SerializeField]
        protected SerializableArgument[] serializedArguments;
        [SerializeField]
        protected string typeName;
        protected Type type = null;

        protected ConstructorInfo constructor = null;

        public void SetConstructorArgs(Type classType,params SerializableArgument[] arguments)
        {
            typeName = GetClassSerializedName(classType);
            type = classType;
            this.serializedArguments = arguments;

            ClearCache();
        }
        public Type GetConstructorType()
        {
            if (type == null)
                type = GetTypeFromName(typeName);
            return type;
        }
        public static Type GetTypeFromName(string assemblyQualifiedClassName)
        {
            return !string.IsNullOrEmpty(assemblyQualifiedClassName)
                ? Type.GetType(assemblyQualifiedClassName)
                : null;
        }
        public static string GetClassSerializedName(Type type)
        {
            var result = type != null
                ? type.FullName + ", " + type.Assembly.GetName().Name
                : "";
            return result;
        }

        public virtual void ClearCache()
        {
            type = null;
            argTypes = null;
            arguments = null;
            constructor = null;
        }

        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
            type = GetTypeFromName(typeName);
        }
        
#if UNITY_EDITOR
        public static Type GetConstructorType(SerializedProperty thisProperty)
        {
            return GetTypeFromName(thisProperty.FindPropertyRelative("typeName").stringValue);
        }
        public static ConstructorInfo GetConstructor(SerializedProperty thisProperty)
        {
            var type = GetConstructorType(thisProperty);
            var argTypes = GetArgumentTypes(thisProperty);
            return type.GetConstructor(argTypes);
        }

        public static Type[] GetArgumentTypes(SerializedProperty thisProperty)
        {
            SerializedProperty argProps = thisProperty.FindPropertyRelative("serializedArguments");
            Type[] argTypes = new Type[argProps.arraySize];
            for (int i = 0; i < argTypes.Length; i++)
            {
                SerializedProperty argProp = argProps.GetArrayElementAtIndex(i);
                argTypes[i] = SerializableArgument.RealType(SerializableArgument.GetPropertyType(argProp));
            }
            return argTypes;
        }
#endif

    }


    [System.Serializable]
    public struct SerializableArgument
    {
        public enum ArgType { Unsupported, Bool, Int, Float, String, Object, Transform }
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;
        public UnityEngine.Object objectValue;
        public Transform transformValue;
        public ArgType argType;

        public object GetValue()
        {
            return GetValue(argType);
        }

        public object GetValue(ArgType type)
        {
            switch (type)
            {
                case ArgType.Bool:
                    return boolValue;
                case ArgType.Int:
                    return intValue;
                case ArgType.Float:
                    return floatValue;
                case ArgType.String:
                    return stringValue;
                case ArgType.Object:
                    return objectValue;
                case ArgType.Transform:
                    return transformValue;
                default:
                    return null;
            }
        }

        public static Type RealType(ArgType type)
        {
            switch (type)
            {
                case ArgType.Bool:
                    return typeof(bool);
                case ArgType.Int:
                    return typeof(int);
                case ArgType.Float:
                    return typeof(float);
                case ArgType.String:
                    return typeof(string);
                case ArgType.Transform:
                    return typeof(Transform);
                case ArgType.Object:
                    return typeof(UnityEngine.Object);
                default:
                    return null;
            }
        }

        public static ArgType FromRealType(Type type)
        {
            if (type == typeof(bool)) return ArgType.Bool;
            else if (type == typeof(int)) return ArgType.Int;
            else if (type == typeof(float)) return ArgType.Float;
            else if (type == typeof(String)) return ArgType.String;
            else if (type == typeof(Transform)) return ArgType.Transform;
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return ArgType.Object;
            else return ArgType.Unsupported;
        }

        public static bool IsSupported(Type type)
        {
            return FromRealType(type) != ArgType.Unsupported;
        }

#if UNITY_EDITOR
        
        public static ArgType GetPropertyType(SerializedProperty thisProperty)
        {
            return (ArgType)thisProperty.FindPropertyRelative("argType").enumValueIndex;
        }

        public static SerializedProperty GetMatchProperty(SerializedProperty thisProperty)
        {
            switch (GetPropertyType(thisProperty))
            {
                case SerializableArgument.ArgType.Bool:
                    return thisProperty.FindPropertyRelative("boolValue");
                case SerializableArgument.ArgType.Int:
                    return thisProperty.FindPropertyRelative("intValue");
                case SerializableArgument.ArgType.Float:
                    return thisProperty.FindPropertyRelative("floatValue");
                case SerializableArgument.ArgType.String:
                    return thisProperty.FindPropertyRelative("stringValue");
                case SerializableArgument.ArgType.Transform:
                    return thisProperty.FindPropertyRelative("transformValue");
                case SerializableArgument.ArgType.Object:
                    return thisProperty.FindPropertyRelative("objectValue");
                default:
                    return null;
            }
        }
#endif
    }
}