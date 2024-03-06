
#if UNITY_EDITOR
namespace UIHelper
{
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public class FontEditor : EditorWindow
    {
        private TMP_FontAsset _asset = null;
        private Color _color = default;

        [MenuItem("Font Changer/Change TMP_Text")]
        public static void ShowWindow()
        {
            var window = GetWindow<FontEditor>("Font Changer");
            window.minSize = new Vector2(400, 475);
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Select the font you want to change to");
            _asset = EditorGUILayout.ObjectField("TMP Font", _asset, typeof(TMP_FontAsset), false) as TMP_FontAsset;
            _color = EditorGUILayout.ColorField("Text color", _color);
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Set font to all TMP"))
            {
                var results = Resources.FindObjectsOfTypeAll<TMP_Text>();
                foreach (var item in results)
                {
                    if (_asset != null) item.font = _asset;
                    if (_color != default) item.color = _color;
                }
            }
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Set font to selected TMP"))
            {
                
                foreach (var item in Selection.gameObjects)
                {
                    if (!item.TryGetComponent(out TMP_Text text)) continue;
                    if (_asset != null) text.font = _asset;
                    if (_color != default) text.color = _color;
                }
            }
        }
    }
#endif
}

