namespace UIHelper
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class RadioButton : MonoBehaviour, IPointerClickHandler
    {
        public Action<RadioButton, bool> OnValueChanged;
        [field: SerializeField] public bool IsOn { get; private set; }
        [SerializeField, Space(2)] private Image _image;
        [SerializeField] private Sprite _activatedImage;
        [SerializeField] private Color _checkedColor;
        public UnityEvent Checked, UnChecked;
        private Sprite _currentImage;
        private Color _currentColor;

        public void OnPointerClick(PointerEventData eventData) =>
           Press();

        private void Awake()
        {
            _currentColor = _image.color;
            _currentImage ??= _image.sprite;
        }
            
        public void Press()
        {
            IsOn = !IsOn;
            Render();
            OnValueChanged?.Invoke(this, IsOn);
        }

        public void Off()
        {
            IsOn = false;
            Render();
        }


        private void Render()
        {
            if (IsOn)
            {
                if(_activatedImage != null)_image.sprite = _activatedImage;
                if (_checkedColor != default) _image.color = _checkedColor;
                Checked?.Invoke();
            }
            else
            {
                if(_currentImage != null)_image.sprite = _currentImage;
                if (_currentColor != default) _image.color = _currentColor;
                UnChecked?.Invoke();
            }
        }

#if UNITY_EDITOR
        public static bool Draw(string label, bool value, Action valueChanged = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);

            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = value ? Color.green : Color.white;

            if (GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20)))
            {
                value = !value;
                valueChanged?.Invoke();
            }

            GUI.backgroundColor = originalColor;
            GUILayout.EndHorizontal();

            return value;
        }
#endif
    }
}

