namespace UIHelper
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class CustomButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private List<GameObject> _off, _on;
        [SerializeField, Space(15)] private UnityEvent _onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            foreach (var item in _off)
            {
                if (item.TryGetComponent(out Canvas c)) c.enabled = false;
                else item.gameObject.SetActive(false);
            }
            foreach (var item in _on)
            {
                if (item.TryGetComponent(out Canvas c)) c.enabled = true;
                else item.gameObject.SetActive(true);
            }
            _onClick?.Invoke();
        }
    }
}