
#if UNITY_EDITOR
namespace UIHelper
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class CanvasEditor : EditorWindow
    {
        private string[] _resolutions = new string[] { "Default","1920x1080", "1280x720", "1366x768" };
        private int _selectedIndex, _previousIndex;
        private float _width, _height;
        private float _math;
        GUIStyle labelStyle;

        [MenuItem("UI Helper/Edit Canvas")]
        public static void ShowWindow()
        {
            var window = GetWindow<CanvasEditor>("Canvas editor");
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
            
            _width = EditorGUILayout.Slider("Width", _width,0,2000);
            _height = EditorGUILayout.Slider("Height", _height,0,2000);
            _math = EditorGUILayout.Slider("Match",_math, 0, 1);
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
            GUILayout.Label("Select resolution");
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _resolutions);
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

            ApplyResolution();
        }

        private void ApplyResolution()
        {
            if (_selectedIndex == _previousIndex) return;
            switch (_selectedIndex)
            {
                case 1:
                    _width = 1920;
                    _height = 1080;
                    break;
                 case 2:
                    _width = 1280;
                    _height = 720;
                    break;
                case 3:
                    _width = 1366;
                    _height = 768;
                    break;
            }
            _previousIndex = _selectedIndex;
        }

        private void SetSettings(CanvasScaler item)
        {
            item.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            item.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            item.GetComponent<Canvas>().vertexColorAlwaysGammaSpace = true;
            item.GetComponent<Canvas>().pixelPerfect = true;
        }
    }
}

#endif