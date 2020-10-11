using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TCUtils
{
    public static class ReflectionUtilities
    {
        public static List<FieldInfo> GetConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }

        public static List<T> GetAllPublicConstantValues<T>(Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }

        public static List<Type> GetAllSubTypes(Type baseType, bool includeStruct = false, bool includeAbstractclass = false, bool includeGenericInstance = false)
        {
            List<Type> subclassTypes;
            if (baseType.IsGenericTypeDefinition)
            {
                subclassTypes = Assembly
                    .GetAssembly(baseType)
                    .GetTypes()
                    .Where(t => HasGenericBase(t, baseType) && (includeAbstractclass || !t.IsAbstract) && (includeStruct || t.IsClass)
                    && (includeGenericInstance || !t.ContainsGenericParameters)).ToList();
            }
            else
            {
                subclassTypes = Assembly
                    .GetAssembly(baseType)
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(baseType) && (includeAbstractclass || !t.IsAbstract) && (includeStruct || t.IsClass)
                    && (includeGenericInstance || !t.ContainsGenericParameters)).ToList();
            }
            return subclassTypes;
        }

        private static bool HasGenericBase(Type myType, Type genericTypeDef)
        {
            Debug.Assert(genericTypeDef.IsGenericTypeDefinition);
            while (myType != typeof(object))
            {
                if (myType.IsGenericType && myType.GetGenericTypeDefinition() == genericTypeDef)
                {
                    return true;
                }
                myType = myType.BaseType;
            }
            return false;
        }
    }
}