#if UNITY_EDITOR
namespace UIHelper
{
    using System;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class GradientView : EditorWindow
    {

        public Color startColor = Color.red, endColor = Color.blue;
        public int width = 200, height = 50;
        private float _duration = 1.0f, _time = 0.0f, _angle;
        private static GUIStyle _style, _labelStyle;
        private bool _showSpawn, _showCopy;

        private int _selectedIndex, _yOffset = 250;
        private string[] _gradientTypes = new string[]{"Linear","Radial"}; 
        private Texture2D _lastTexture, _toDraw;

        [MenuItem("UI Helper/Gradient")]
        public static void ShowWindow()
        {
            Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            var window = GetWindow<GradientView>("Gradient generator",new Type[] { inspectorType });
            window.minSize = new Vector2(400, 475);
        } 

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Generate Gradient", new GUIStyle(EditorStyles.boldLabel));
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            startColor = EditorGUILayout.ColorField(startColor);
            endColor = EditorGUILayout.ColorField(endColor);
            GUILayout.EndHorizontal();
            width = EditorGUILayout.IntSlider("Width", width, 0, 1000);
            height = EditorGUILayout.IntSlider("Height", height,0,1000);
            if(_selectedIndex == 0)_angle = EditorGUILayout.Slider("Angle", _angle, 0, 360);
            GUILayout.BeginHorizontal();
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _gradientTypes);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) _toDraw = GetTexture();
            GUILayout.Space(5);
            if (GUILayout.Button("Set gradient to selected"))  SetTexture();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Swap Colors"))
            {
                _toDraw = GetTexture();
                SwapColors();
            }
            if (GUILayout.Button("Save Texture")) Save();
            GUILayout.EndHorizontal();

            _style = new(GUI.skin.button);
            _style.normal.textColor = Color.Lerp(Color.white, Color.cyan, _time);
            _time = Mathf.PingPong(Time.time / _duration, 1.0f);

            if (GUILayout.Button("Random colors", _style))
            {
                startColor = UnityEngine.Random.ColorHSV();
                endColor = UnityEngine.Random.ColorHSV();
                _toDraw = GetTexture();
            }
            GUILayout.Space(5);
            
            _showSpawn = EditorGUILayout.Foldout(_showSpawn, "Spawn");
            if (_showSpawn)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Image")) SpawnImage();
                if (GUILayout.Button("TMP")) SpawnTMP();
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            _showCopy = EditorGUILayout.Foldout(_showCopy, "Copy gradient");
            if (_showCopy)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Horizontal gradient"))
                {
                    CopyGradient(true);
                    _toDraw = GetTexture();
                }
                if (GUILayout.Button("Vertical gradient"))
                {
                    CopyGradient(false);
                    _toDraw = GetTexture();
                }
                GUILayout.EndHorizontal();
            }

            if (_showCopy)
            {
                if (_showSpawn) GUI.DrawTexture(new Rect(10, _yOffset + 60, 60, 60), _toDraw);
                else GUI.DrawTexture(new Rect(10, _yOffset + 30, 60, 60), _toDraw);
            }
            else GUI.DrawTexture(new Rect(10, _yOffset + 30, 60, 60), _toDraw);

            GUILayout.Space(10);
            _labelStyle = new()
            {
                fontSize = 16
            };
            _labelStyle.normal.textColor = Color.white;
            GUILayout.Label("Preview :", _labelStyle);
            Repaint();
        }

        private void SpawnTMP()
        {
            var tmp = new GameObject("Gradient TMP", typeof(TextMeshProUGUI));
            var txt = tmp.GetComponent<TMP_Text>();
            GradientCreator.Text(_selectedIndex == 0, txt, startColor, endColor);
            var rectTransform = Selection.activeObject.GetComponent<RectTransform>();
            if (rectTransform == null) return;
            tmp.transform.SetParent(rectTransform.transform, false);
            var rect = tmp.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);
            txt.text = "Gradient text";
            txt.enableAutoSizing = true;
        }

        private void SpawnImage()
        {
            var img = new GameObject("Gradiented image", typeof(Image));
            img.GetComponent<Image>().sprite = Sprite.Create(GetTexture(), new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
            var rectTransform = Selection.activeObject.GetComponent<RectTransform>();
            if (rectTransform == null) return;
            img.transform.SetParent(rectTransform.transform, false);
            var rect = img.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);
        }

        private void CopyGradient(bool horizontal)
        {
            var selectedObj = Selection.activeGameObject;
            if (selectedObj.TryGetComponent(out Image img))
            {
                var texture = img.sprite.texture;
                if (horizontal)
                {
                    startColor = texture.GetPixel(0, 0);
                    endColor = texture.GetPixel(texture.width - 1, 0);
                }
                else
                {
                    startColor = texture.GetPixel(0, texture.height - 1);
                    endColor = texture.GetPixel(0, 0);
                }
            }
        }

        private void Save()
        {
            var window = GetWindow<TextureSaver>("Texture saver");
            window.minSize = new Vector2(200, 200);
            window.GetTexture(_lastTexture);
        }

        private void SwapColors() =>
            (endColor, startColor) = (startColor, endColor);

        private Texture2D GetTexture()
        {
            Texture2D gradientTexture;
            if (_selectedIndex == 0) gradientTexture = GradientCreator.Generate(width, height, startColor, endColor, _angle);
            //else if (_selectedIndex == 1) gradientTexture = GradientCreator.Conus(width, height, startColor, endColor) ;
            else gradientTexture = GradientCreator.Radial(width, height, startColor, endColor);
            _lastTexture = gradientTexture;
            return gradientTexture;
        }

        private void SetTexture()
        {
            foreach(var item in Selection.gameObjects)
            {
                if(item.TryGetComponent(out Image img)) img.sprite = Sprite.Create(GetTexture(), new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                if (item.TryGetComponent(out TMP_Text text)) GradientCreator.Text(_selectedIndex == 0, text, startColor, endColor);
            }
        }
    }
}
#endif