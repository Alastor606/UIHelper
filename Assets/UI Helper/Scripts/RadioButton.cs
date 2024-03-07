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
        public UnityEvent Checked, UnChecked;
        private Sprite _currentImage;
        
        private void Awake() =>
            _currentImage ??= _image.sprite;

        public void OnPointerClick(PointerEventData eventData) =>
            Press();

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
                _image.sprite = _activatedImage;
                Checked?.Invoke();
            }
            else
            {
                _image.sprite = _currentImage;
                UnChecked?.Invoke();
            }
        }
    }
}

