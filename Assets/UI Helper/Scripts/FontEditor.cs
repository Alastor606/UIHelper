
#if UNITY_EDITOR
namespace UIHelper
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public class FontEditor : EditorWindow
    {

        private TMP_FontAsset _asset = null;
        private Color _color = default;
        GUIStyle labelStyle;

        private bool _bold, _italic, _underfine, _striketrouth, _lower, _upper, _smallcaps;
        private int _size;

        [MenuItem("UI Helper/Edit Text")]
        public static void ShowWindow()
        {
            var window = GetWindow<FontEditor>("Font Changer");
            window.minSize = new Vector2(400, 475);
        }

        void OnGUI()
        {
            if (labelStyle == null)
            {
                labelStyle = new(GUI.skin.label);
                labelStyle.fontSize = 16;
                labelStyle.normal.textColor = Color.white;
            }
            EditorGUILayout.LabelField("Select the font you want to change to", labelStyle);
            EditorGUILayout.Space(10);
            _asset = EditorGUILayout.ObjectField("TMP Font", _asset, typeof(TMP_FontAsset), false) as TMP_FontAsset;
            _color = EditorGUILayout.ColorField("Text color", _color);
            _size = EditorGUILayout.IntField("Font size", _size);
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            _bold = GUILayout.Toggle(_bold, "B", "Button");
            _italic = GUILayout.Toggle(_italic, "I", "Button");
            _underfine = GUILayout.Toggle(_underfine, "U", "Button");
            _striketrouth = GUILayout.Toggle(_striketrouth, "S", "Button");
            _lower = GUILayout.Toggle(_lower, "ab", "Button");
            _upper = GUILayout.Toggle(_upper, "AB", "Button");
            _smallcaps = GUILayout.Toggle(_smallcaps, "SC", "Button");
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Set to all")) Settings(FindObjectsOfType<TMP_Text>());

            if (GUILayout.Button("Set to selected"))
            {
                List<TMP_Text> texts = new();
                foreach (var item in Selection.gameObjects)
                {
                    if (item.TryGetComponent(out TMP_Text text)) texts.Add(text);
                    if(item.transform.childCount > 0)
                    {
                        var items = item.GetComponentsInChildren<TMP_Text>();
                        foreach (var i in items) texts.Add(i);
                    }
                }
                Settings(texts.ToArray());
            }
        }

        private void Settings(params TMP_Text[] texts)
        {
            foreach (var item in texts)
            {
                if (_asset != null) item.font = _asset;
                if (_color != default) item.color = _color;
                item.fontStyle = FontStyles.Bold;
                
            }
        }
    }
}
#endif

