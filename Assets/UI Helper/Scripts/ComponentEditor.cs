#if UNITY_EDITOR
namespace UIHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ComponentEditor : EditorWindow
    {
        private List<Component> _components = new();
        private GameObject _currentObject;
        private int _selectedIndex;
        Vector2 scrollPosition = Vector2.zero;

        [MenuItem("UI Helper/Component Editor")]
        public static void Init()
        {
            Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            var window = GetWindow<ComponentEditor>("Component editor", new Type[] { inspectorType });
            window.minSize = new Vector2(400, 475);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("select an object with the required component");
            EditorGUI.BeginChangeCheck();
            _currentObject = EditorGUILayout.ObjectField(_currentObject, typeof(GameObject), true)as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                _components = new(_currentObject.GetComponents<Component>());
            }
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, ToStringArray(_components));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Settings");
            if(_currentObject != null)
            {
                if (_selectedIndex >= 0 && _selectedIndex < _components.Count)
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    Component selectedComponent = _components[_selectedIndex];
                    SerializedObject serializedObject = new(selectedComponent);
                    SerializedProperty prop = serializedObject.GetIterator();

                    bool enterChildren = true;
                    while (prop.NextVisible(enterChildren))
                    {
                        enterChildren = false;
                        EditorGUILayout.PropertyField(prop, true);
                    }

                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.EndScrollView();
                }
            }
            GUILayout.Space(10);
            if(GUILayout.Button("Apply to all objects")) SetToObjects(FindObjectsOfType<Component>());

            if(GUILayout.Button("Apply to selected objects"))
            {
                SetToObjects(GetComponentsAll(Selection.gameObjects));
            }
        }
         
        private void SetToObjects(Component[] allComponents)
        {
            Component selectedComponent = _currentObject.GetComponents<Component>()[_selectedIndex];
            foreach (Component component in allComponents)
            {
                if (component.GetType() == selectedComponent.GetType())
                {
                    Undo.RecordObject(component, "Apply Settings");
                    EditorUtility.CopySerialized(selectedComponent, component);
                }
            }
        }

        private string[] ToStringArray(List<Component> components)
        {
            var list = new List<string>();
            foreach (var item in components)
            {
                var str = item.ToString().Split('(').Last();
                list.Add(str.Remove(str.Length - 1));
            }
            return list.ToArray();
        }

        private Component[] GetComponentsAll(GameObject[] arr)
        {
            var list = new List<Component>();
            foreach(var item in arr)
            {
                if(item.TryGetComponent(out Component value))list.Add(value);
            }
            return list.ToArray();
        }
    }
}

#endif