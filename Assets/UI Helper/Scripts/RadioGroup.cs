namespace UIHelper
{
    using System.Collections.Generic;
    using UnityEngine;
    public class RadioGroup : MonoBehaviour
    {
        [Tooltip("Container is Optional field, if it not null all child witch have a component RadioButton adds to List")]
        [SerializeField] private Transform _container;
        [SerializeField] private List<RadioButton> _buttons;

        private void OnValidate()
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
            if (value == false) return;
            foreach (var radio in _buttons)
            {
                if (radio != button) radio.Off();
            }
        }
    }
}


