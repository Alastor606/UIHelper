namespace UIHelper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector, SerializeField]private float _scale, _timeToScale, _changeColorTime;
        [SerializeField, HideInInspector] private Color _startColor, _endColor;
        [HideInInspector]public bool isScaleable, isColorable;
        [SerializeField, Tooltip("Objects to set Active, or change enabled of canvas")] private List<GameObject> _off, _on;
        [SerializeField, Space(15)] private UnityEvent _onClick;
        private Image _image;
        private bool _stopScale = false, _stopUnscale = false, _stopChangeColor = false, _stopResetColor = false;
        private Vector2 _mainScale;

        private void Awake()
        {
            _image ??= GetComponent<Image>();
            _mainScale = transform.localScale;
            _startColor = _image.color;
        }
           
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
         
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isScaleable)
            {
                _stopScale = false;
                _stopUnscale = true;
                StopCoroutine(UnScale());
                StartCoroutine(Scale());
            }
            if(isColorable)
            {
                _stopResetColor = true;
                _stopChangeColor = false;

                StartCoroutine(ChangeColor());
                StopCoroutine(ResetColor());
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isScaleable)
            {
                _stopScale = true;
                _stopUnscale = false;
                StopCoroutine(Scale());
                StartCoroutine(UnScale());
            }
            if (isColorable)
            {
                _stopResetColor = false;
                _stopChangeColor = true;

                StartCoroutine(ResetColor());
                StopCoroutine(ChangeColor());
            }
        }

        private IEnumerator ChangeColor()
        {
            var currentTime = 0.0f;
            while (currentTime <= _changeColorTime)
            {
                _image.color = Color.Lerp(_startColor, _endColor, currentTime / _changeColorTime);
                currentTime += Time.deltaTime;
                if (_stopChangeColor) yield break;
                yield return null;
            }
            _image.color = _endColor;
        }

        private IEnumerator ResetColor()
        {
            var currentTime = 0.0f;
            while (currentTime <= _changeColorTime)
            {
                _image.color = Color.Lerp(_endColor, _startColor, currentTime / _changeColorTime);
                currentTime += Time.deltaTime;
                if (_stopResetColor) yield break;
                yield return null;
            }
            _image.color = _startColor;
        }

        private IEnumerator Scale()
        {
            float currentTime = 0.0f;

            while (currentTime <= _timeToScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(_scale, _scale), currentTime / _timeToScale);
                currentTime += Time.deltaTime;
                if (_stopScale) yield break;
                yield return null;
            }

            transform.localScale = new Vector3(_scale, _scale);
        }

        private IEnumerator UnScale()
        {
            float currentTime = 0.0f;
            while (currentTime <= _timeToScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _mainScale, currentTime / _timeToScale);
                currentTime += Time.deltaTime;
                if (_stopUnscale) yield break;
                yield return null;
            }

            transform.localScale = _mainScale;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomButton)), Serializable]
    public class EditorButton : Editor
    {
        [SerializeField]private bool _showDropdown = false, _syncTime;
        [SerializeField]private float _syncedTime;
        GUIStyle _style = new();

        SerializedProperty scaleProperty, timeToScaleProperty, timeToChangeColor, endColor;

        void OnEnable()
        {
            scaleProperty = serializedObject.FindProperty("_scale");
            timeToScaleProperty = serializedObject.FindProperty("_timeToScale");
            timeToChangeColor = serializedObject.FindProperty("_changeColorTime");
            endColor = serializedObject.FindProperty("_endColor");
            _style.fontSize = 12;
            _style.fontStyle = FontStyle.Bold;
            _style.normal.textColor = Color.white;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var tg = (CustomButton)target;
            base.OnInspectorGUI();
            _showDropdown = EditorGUILayout.Foldout(_showDropdown, "Additional Settings", true);
            if (_showDropdown)
            {
                if (tg.isScaleable && tg.isColorable)
                {
                    _syncTime = EditorGUILayout.Toggle("Sync time", _syncTime);
                    if(!_syncTime)EditorGUILayout.Space(20);
                }
                if (_syncTime)
                {
                    _syncedTime = EditorGUILayout.FloatField("Synced time", _syncedTime);
                    timeToChangeColor.floatValue = _syncedTime;
                    timeToScaleProperty.floatValue = _syncedTime;
                    EditorGUILayout.Space(10);
                }
                EditorGUILayout.LabelField("Scale settings", _style);
                tg.isScaleable = EditorGUILayout.Toggle("Scale on poiner enter ",tg.isScaleable);
                if (tg.isScaleable)
                {
                    if (!_syncTime) EditorGUILayout.PropertyField(timeToScaleProperty);
                    EditorGUILayout.PropertyField(scaleProperty);
                }
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Color settings", _style);
                tg.isColorable = EditorGUILayout.Toggle("Change color on pointer enter", tg.isColorable);
                if (tg.isColorable)
                { 
                    if(!_syncTime)EditorGUILayout.PropertyField(timeToChangeColor);
                    EditorGUILayout.PropertyField(endColor);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}