

namespace UIHelper
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System.Collections.Generic;
    using UnityEngine;

    public class RadioGroup : MonoBehaviour
    {
        [Tooltip("Container is Optional field, if it not null all child witch have a component RadioButton adds to List")]
        [SerializeField] private Transform _container;
        [SerializeField] private List<RadioButton> _buttons;

        private void OnValidate() =>
            Ping();
        public void ClearList() => 
            _buttons.Clear();

        public void Add(RadioButton radio)
        {
            _buttons.Add(radio);
            radio.OnValueChanged += CheckButtonsActive;
        }
            
        public void Ramove(RadioButton radio)
        {
            _buttons.Remove(radio);
            radio.OnValueChanged -= CheckButtonsActive;
        }

        public void Ping()
        {
            if (_container == null) return;
            foreach (Transform child in _container)
            {
                if (child.TryGetComponent(out RadioButton button) && !_buttons.Contains(button)) _buttons.Add(button);
            }
        }

        private void Awake()
        {
            foreach (var button in _buttons) button.OnValueChanged += CheckButtonsActive;
        }

        private void CheckButtonsActive(RadioButton button, bool value)
        {
            if (value == false)
            {
                button.Press();
                return;
            }
            foreach (var radio in _buttons)
            {
                if (radio != button) radio.Off();
            }
           
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(RadioGroup))]
    public class RadioEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            var tg = (RadioGroup)target;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                UISpawner.Spawn("Assets/UI Helper/Prefabs/RadioButton.prefab", tg.transform);
                tg.Ping();
                serializedObject.Update();
            }
            if (GUILayout.Button("Clear"))
            {
                for (int i = tg.transform.childCount - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(tg.transform.GetChild(i).gameObject);
                }
                tg.ClearList();
                serializedObject.Update();
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
        }
    }

#endif
}