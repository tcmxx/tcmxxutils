using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace TCUtils
{
    [CustomPropertyDrawer(typeof(SerializableClassConstructor), true)]
    public class SerializableClassContructorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            
            GUI.Box(position, "");
            //show the dropdown list for selecting constructors
            SerializedProperty typeNameProp = property.FindPropertyRelative("typeName");

            string dropDownMenuName = typeNameProp.stringValue;
            if (typeNameProp.stringValue.LastIndexOf(',') >= 0)
                dropDownMenuName = dropDownMenuName.Substring(0, typeNameProp.stringValue.LastIndexOf(','));

            dropDownMenuName = dropDownMenuName.Substring(dropDownMenuName.LastIndexOf('.') + 1);
            bool selected = !string.IsNullOrEmpty(dropDownMenuName);
            if (!selected)
            {
                dropDownMenuName = "Select a constructor";
            }
            position.height = EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.standardVerticalSpacing;
            Rect pos = EditorGUI.PrefixLabel(position, label);
            var tempPos = position;
            tempPos.x += pos.x;
            // Method select button
            if (EditorGUI.DropdownButton(pos, new GUIContent(dropDownMenuName), FocusType.Keyboard))
            {
                ShowConstructorSelector(property);
            }

            if (selected)
            {
                //show the constructor argments
                var parameterTypes = SerializableClassConstructor.GetArgumentTypes(property);
                Type classType = SerializableClassConstructor.GetConstructorType(property);
                var constructor = classType.GetConstructor(parameterTypes);
                if (constructor != null)
                {
                    var parameters = constructor.GetParameters();
                    SerializedProperty argProps = property.FindPropertyRelative("serializedArguments");

                    EditorGUI.indentLevel++;
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        SerializedProperty argProp = argProps.GetArrayElementAtIndex(i);
                        var drawPos = position;
                        //drawPos.height -= EditorGUIUtility.standardVerticalSpacing;
                        ShowSerializedArg(drawPos, argProp, parameters[i].Name);

                    }
                    EditorGUI.indentLevel--;
                }
            }

            // Set indent back to what it was
            EditorGUI.EndProperty();
        }

        private class MenuItem
        {
            public GenericMenu.MenuFunction action;
            public string path;
            public GUIContent label;

            public MenuItem(string path, string name, GenericMenu.MenuFunction action)
            {
                this.action = action;
                this.label = new GUIContent(path + '/' + name);
                this.path = path;
            }
        }

        protected void ShowConstructorSelector(SerializedProperty property)
        {
            // base type constraint
            Type baseType = SerializableClassConstructor.GetTypeFromName(
                property.FindPropertyRelative("baseTypeString").stringValue);

            var subclassTypes = ReflectionUtilities.GetAllSubTypes(baseType);

            List<MenuItem> menuItems = new List<MenuItem>();

            for (int i = 0; i < subclassTypes.Count; i++)
            {
                Type t = subclassTypes[i];

                ConstructorInfo[] constructors = t.GetConstructors();

                foreach (var constructor in constructors)
                {
                    Type[] parms = constructor.GetParameters().Select(x => x.ParameterType).ToArray();

                    // Skip methods with unsupported args
                    if (parms.Any(x => !SerializableArgument.IsSupported(x))) continue;

                    string constructorPrettyName = GetConstructorShowString(constructor);
                    menuItems.Add(new MenuItem(t.Name, constructorPrettyName, () => SetConstructor(property, constructor)));
                }
            }

            // Construct and display context menu
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < menuItems.Count; i++)
            {
                menu.AddItem(menuItems[i].label, false, menuItems[i].action);
            }
            if (menu.GetItemCount() == 0) menu.AddDisabledItem(new GUIContent("No supported constructor."));
            menu.ShowAsContext();
        }

        string GetConstructorShowString(string constructorName, Type[] parmTypes)
        {
            string parmnames = GetTypesString(parmTypes);
            return constructorName + "(" + parmnames + ")";
        }

        string GetConstructorShowString(ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null) throw new ArgumentNullException("constructorInfo");
            ParameterInfo[] parms = constructorInfo.GetParameters();
            string parmnames = GetTypesString(parms.Select(x => x.ParameterType).ToArray());

            return constructorInfo.DeclaringType.Name + "(" + parmnames + ")";
        }

        string GetTypesString(Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            return string.Join(", ", types.Select(x => GetTypeName(x)).ToArray());
        }

        private void SetConstructor(SerializedProperty property, ConstructorInfo constructorInfo)
        {
            SerializedProperty typeNameProp = property.FindPropertyRelative("typeName");
            typeNameProp.stringValue = SerializableClassConstructor.GetClassSerializedName(constructorInfo.DeclaringType);

            SerializedProperty argProp = property.FindPropertyRelative("serializedArguments");
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            argProp.arraySize = parameters.Length;
            for (int i = 0; i < parameters.Length; i++)
            {
                argProp.GetArrayElementAtIndex(i).FindPropertyRelative("argType").enumValueIndex = (int)SerializableArgument.FromRealType(parameters[i].ParameterType);
            }
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

        }

        protected void ShowSerializedArg(Rect position, SerializedProperty argProperty, string argName)
        {
            var valueProp = SerializableArgument.GetMatchProperty(argProperty);
            EditorGUI.PropertyField(position, valueProp, new GUIContent(argName));
        }


        private static string GetTypeName(Type t)
        {
            if (t == typeof(int)) return "int";
            else if (t == typeof(float)) return "float";
            else if (t == typeof(string)) return "string";
            else if (t == typeof(bool)) return "bool";
            else if (t == typeof(Transform)) return "transform";
            else if (t == typeof(void)) return "void";
            else if (t == typeof(UnityEngine.Object)) return "Object";
            else return t.Name;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeNameProp = property.FindPropertyRelative("typeName");
            string dropDownMenuName = typeNameProp.stringValue;
            bool selected = !string.IsNullOrEmpty(dropDownMenuName);
            if (!selected)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var parameterTypes = SerializableClassConstructor.GetArgumentTypes(property);
            int count = 1;
            Type classType = SerializableClassConstructor.GetConstructorType(property);
            if (classType != null)
            {
                var constructor = classType.GetConstructor(parameterTypes);
                if (constructor != null)
                {
                    var parameters = constructor.GetParameters();
                    count += (parameterTypes == null ? 0 : parameterTypes.Length);
                }
            }
            return EditorGUIUtility.standardVerticalSpacing * (count + 1) + EditorGUIUtility.singleLineHeight * count;
        }


    }
}