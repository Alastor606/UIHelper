
#if UNITY_EDITOR
namespace UIHelper
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class CanvasEditor : EditorWindow
    {
        private float _width, _height;
        private float _math;
        GUIStyle labelStyle;


        [MenuItem("UI Helper/Edit Canvas")]
        public static void ShowWindow()
        {
            var window = GetWindow<CanvasEditor>("Font Changer");
            window.minSize = new Vector2(400, 475);
            
        }

        private void OnGUI()
        {
            if (labelStyle == null)
            {
                labelStyle = new(GUI.skin.label);
                labelStyle.fontSize = 16;
                labelStyle.normal.textColor = Color.white;
            }
            EditorGUILayout.LabelField("Set Reference resolution", labelStyle);
            
            _width = EditorGUILayout.FloatField("Width", _width);
            _height = EditorGUILayout.FloatField("Height", _height);
            _math = EditorGUILayout.FloatField("Match",_math);
            _width = Mathf.Clamp(_width, 0, 2000);
            _height = Mathf.Clamp(_height, 0, 2000);
            _math = Mathf.Clamp(_math, 0, 1);
            if (GUILayout.Button("Set custom Scale for all"))
            {
                foreach (var item in Resources.FindObjectsOfTypeAll<CanvasScaler>())
                {
                    SetSettings(item);
                    item.referenceResolution = new Vector2(_width, _height);
                    item.matchWidthOrHeight = _math;
                }
            }

            if (GUILayout.Button("Set custom Scale for selected"))
            {
                foreach (var item in Selection.gameObjects)
                {
                    if (!item.TryGetComponent(out CanvasScaler scale)) continue;
                    SetSettings(scale);
                    scale.referenceResolution = new Vector2(_width, _height);
                    scale.matchWidthOrHeight = _math;
                }
            }
            EditorGUILayout.Space(25);
            EditorGUILayout.LabelField("Automatic scale sets to all canvases on scene", labelStyle);
            if (GUILayout.Button("Auto Mobile Scale"))
            {
                foreach(var item in Resources.FindObjectsOfTypeAll<CanvasScaler>())
                {
                    SetSettings(item);
                    item.referenceResolution = new Vector2(1920, 1080);
                    item.matchWidthOrHeight = 1;
                }
            }

            if(GUILayout.Button("Auto PC Scale"))
            {
                foreach (var item in Resources.FindObjectsOfTypeAll<CanvasScaler>())
                {
                    SetSettings(item);
                    item.referenceResolution = new Vector2(1920, 1080);
                    item.matchWidthOrHeight = 0.5f;
                }
            }
        }

        private void SetSettings(CanvasScaler item)
        {
            item.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            item.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        }
    }
}

#endif