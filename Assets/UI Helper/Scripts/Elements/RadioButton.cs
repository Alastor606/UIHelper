namespace UIHelper
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class RadioButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Action<RadioButton, bool> OnValueChanged;
        [field: SerializeField] public bool IsOn { get; private set; }
        [SerializeField, Space(2)] private Color _pressedColor = new (179,179,179);
        [SerializeField] private Image _buttonImage;
        [Header("Optional"), Tooltip("Settings apply to toggle image")]
        [SerializeField] private Image _toggleImage;
        [SerializeField] private Color _checkedColor;
        [SerializeField] private Sprite _activatedImage;
        [SerializeField, Tooltip("Objects off when checked is false, and on when checked true")] private List<GameObject> _objectsToSwitch;
        [Space(10)] public UnityEvent Checked, UnChecked, OnClick;
        private Sprite _currentImage;
        private Color _currentColor;
        public TMP_Text TMP {  get; private set; }
        public string text { get { return TMP.text; } set { TMP.text = value; } }

        public void SetCheckedColor(Color value) => _checkedColor = value;
        public void SetCheckedImage(Sprite value) => _activatedImage = value;
        public void AddSwitchObject(GameObject value) => _objectsToSwitch.Add(value);

        public void OnPointerClick(PointerEventData eventData) =>
           Press();

        private void Awake()
        {
            TMP ??= GetComponentInChildren<TMP_Text>();
            _currentColor = _toggleImage.color;
            _currentImage ??= _toggleImage.sprite;
        }
            
        public void Press()
        {
            IsOn = !IsOn;
            Render();
            OnValueChanged?.Invoke(this, IsOn);
            OnClick?.Invoke();
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
                if(_activatedImage != null) _toggleImage.sprite = _activatedImage;
                if (_checkedColor != default) _toggleImage.color = _checkedColor;
                if (_objectsToSwitch.Count > 0) foreach (var item in _objectsToSwitch) item.SetActive(true);
                Checked?.Invoke();
            }
            else
            {
                if(_currentImage != null)_toggleImage.sprite = _currentImage;
                if (_currentColor != default) _toggleImage.color = _currentColor;
                if (_objectsToSwitch.Count > 0) foreach (var item in _objectsToSwitch) item.SetActive(false);
                UnChecked?.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData) =>
            _buttonImage.color = _pressedColor;
        
        public void OnPointerUp(PointerEventData eventData) =>
            _buttonImage.color = Color.white;
        

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

