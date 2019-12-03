using RoboRyanTron.SearchableEnum.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Slaggy.Utility
{
    // Inspired by
    // https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SearchableEnum/Code/Editor/SearchableEnumDrawer.cs

    public static class TypeDrawerUtillity
    {
        public static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }

        public static void GetTypes(Type baseType, out string[] aqnTypes, out string[] displayTypes, bool includeGenericArgumentTypes = false)
        {
            List<Type> types = new List<Type>();

            if (baseType != null)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    types.AddRange(assembly.GetTypes());

                types = types.FindAll(baseType.IsAssignableFrom);
            }
            else
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    types.AddRange(assembly.GetTypes());
            }

            if (!includeGenericArgumentTypes) types = types.FindAll(type => !type.IsGenericType);


            aqnTypes = types.Select(type => type.AssemblyQualifiedName).ToArray();
            displayTypes = types.Select(type => type.Name).ToArray();
        }

        public static Type GetTypeFromAQN(string assemblyQualifiedName)
        {
            return Type.GetType(assemblyQualifiedName);
        }

        public static string GetNiceTypeName(Type type)
        {
            return type.Name;
        }

        public static string GetNiceTypeName(string assemblyQualifiedName)
        {
            return GetNiceTypeName(GetTypeFromAQN(assemblyQualifiedName));
        }
    }

    [CustomPropertyDrawer(typeof(ConstrainedSerializableTypeBase), true)]
    public class SerializableGenericTypeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Cache of the hash to use to resolve the ID for the drawer.
        /// </summary>
        private int _idHash;

        private string[] _types = null;
        private string[] _displayTypes = null;
        private int _typeIndex = -1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // By manually creating the control ID, we can keep the ID for the
            // label and button the same. This lets them be selected together
            // with the keyboard in the inspector, much like a normal popup.
            if (_idHash == 0) _idHash = "SerializableGenericTypeBase".GetHashCode();
            int id = GUIUtility.GetControlID(_idHash, FocusType.Keyboard, position);

            DrawType(position, fieldInfo.FieldType, property, label, id);
        }

        private void DrawType(Rect position, Type type, SerializedProperty property, GUIContent label, int id)
        {
            SerializedProperty typeNameProperty = property.FindPropertyRelative("assemblyQualifiedName");

            if (_types == null || _types.Length == 0)
            {
                if (type.IsArray) type = type.GetElementType();
                TypeDrawerUtillity.GetTypes(type.BaseType.GetGenericArguments()[0],
                    out _types, out _displayTypes);

                _typeIndex = -1;
            }

            Type serializedType = TypeDrawerUtillity.GetTypeFromAQN(typeNameProperty.stringValue);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText = serializedType == null
                ? new GUIContent()
                : new GUIContent(TypeDrawerUtillity.GetNiceTypeName(serializedType));

            if (TypeDrawerUtillity.DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
                    _typeIndex = i;
                    typeNameProperty.stringValue = _types[i];
                    property.serializedObject.ApplyModifiedProperties();
                };

                SearchablePopup.Show(position, _displayTypes, _typeIndex, onSelect);
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Cache of the hash to use to resolve the ID for the drawer.
        /// </summary>
        private int _idHash;

        private string[] _types = null;
        private string[] _displayTypes = null;
        private int _typeIndex = -1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // By manually creating the control ID, we can keep the ID for the
            // label and button the same. This lets them be selected together
            // with the keyboard in the inspector, much like a normal popup.
            if (_idHash == 0) _idHash = "SerializableType".GetHashCode();
            int id = GUIUtility.GetControlID(_idHash, FocusType.Keyboard, position);
            DrawType(position, fieldInfo.FieldType, property, label, id);
        }

        private void DrawType(Rect position, Type type, SerializedProperty property, GUIContent label, int id)
        {
            SerializedProperty typeNameProperty = property.FindPropertyRelative("assemblyQualifiedName");

            if (_types == null || _types.Length == 0)
            {
                TypeDrawerUtillity.GetTypes(null, out _types, out _displayTypes);
                _typeIndex = -1;
            }

            Type serializedType = TypeDrawerUtillity.GetTypeFromAQN(typeNameProperty.stringValue);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText = serializedType == null
                ? new GUIContent()
                : new GUIContent(TypeDrawerUtillity.GetNiceTypeName(serializedType));

            if (TypeDrawerUtillity.DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
                    _typeIndex = i;
                    typeNameProperty.stringValue = _types[i];
                    property.serializedObject.ApplyModifiedProperties();
                };

                SearchablePopup.Show(position, _displayTypes, _typeIndex, onSelect);
            }

            EditorGUI.EndProperty();
        }
    }
}