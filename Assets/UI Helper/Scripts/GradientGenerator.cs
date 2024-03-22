#if UNITY_EDITOR
namespace UIHelper
{
    using System;
    using System.IO;
    using TMPro;
    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class GradientGenerator : EditorWindow
    {

        public Color startColor = Color.red;
        public Color endColor = Color.blue;
        public int width = 200;
        public int height = 50;
        private float duration = 1.0f;
        private float t = 0.0f;
        private static GUIStyle _style, _labelStyle;

        private Texture2D _lastTexture;
        private bool _horizontal = true, _vertical;

        [MenuItem("UI Helper/Gradient")]
        public static void ShowWindow()
        {
            Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            var window = GetWindow<GradientGenerator>("Gradient generator",new Type[] { inspectorType });
            window.minSize = new Vector2(400, 475);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Generate Gradient", new GUIStyle(EditorStyles.boldLabel));
            startColor = EditorGUILayout.ColorField(startColor);
            endColor = EditorGUILayout.ColorField(endColor);
            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height); 
            GUILayout.BeginHorizontal();
            _horizontal = RadioButton.Draw("Horizontal", _horizontal, () => _vertical = false);
            _vertical = RadioButton.Draw("Vertical", _vertical, () => _horizontal = false);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            width = Mathf.Clamp(width,0, 1000);
            height = Mathf.Clamp(height, 0, 1000);
            if (GUILayout.Button("Set gradient to selected"))  SetTexture();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Swap Colors")) SwapColors();
            if (GUILayout.Button("Save Texture")) Save();
            GUILayout.EndHorizontal();

            _style = new(GUI.skin.button);
            _style.normal.textColor = Color.Lerp(Color.white, Color.cyan, t);
            t = Mathf.PingPong(Time.time / duration, 1.0f);

            if (GUILayout.Button("Random colors", _style))
            {
                startColor = UnityEngine.Random.ColorHSV();
                endColor = UnityEngine.Random.ColorHSV();
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Spawn image")) SpawnImage();
            if (GUILayout.Button("Spawn TMP")) SpawnTMP();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("Copy colors from selected image");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy horizontal gradient")) CopyGradientHorizontal();
            if (GUILayout.Button("Copy vertical gradient")) CopyGradientVertical();
            GUILayout.EndHorizontal();

            GUI.DrawTexture(new Rect(10, 300, 60, 60), GetTexture());
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
            SetTextGradient(_horizontal, txt, startColor, endColor);
            var rectTransform = Selection.activeObject.GetComponent<RectTransform>();
            if (rectTransform == null) return;
            tmp.transform.SetParent(rectTransform.transform, false);
            var rect = tmp.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 150);
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
            rect.sizeDelta = new Vector2(400, 400);

        }

        private void CopyGradientHorizontal()
        {
            var selectedObj = Selection.activeGameObject;
            if(selectedObj.TryGetComponent(out Image img))
            {
                _horizontal = true;
                _vertical = false;
                var texture = img.sprite.texture;
                startColor = texture.GetPixel(0, 0);
                endColor = texture.GetPixel(texture.width - 1, 0);
            }
        }

        private void CopyGradientVertical()
        {
            var selectedObj = Selection.activeGameObject;
            if (selectedObj.TryGetComponent(out Image img))
            {
                _vertical = true;
                _horizontal = false;
                var texture = img.sprite.texture;
                startColor = texture.GetPixel(0, texture.height - 1);
                endColor = texture.GetPixel(0, 0);
            }
        }

        private void Save()
        {
            var window = GetWindow<TextureSaver>("Gradient generator");
            window.minSize = new Vector2(200, 200);
            window.GetTexture(_lastTexture);
        }

        private void SwapColors() =>
            (endColor, startColor) = (startColor, endColor);

        private Texture2D GetTexture()
        {
            Texture2D gradientTexture;
            if (_horizontal) gradientTexture = CreateGradientTexture(width, height, startColor, endColor);
            else gradientTexture = CreateVerticalGradientTexture(width, height, startColor, endColor);
            _lastTexture = gradientTexture;
            return gradientTexture;
        }

        private void SetTexture()
        {
            foreach(var item in Selection.gameObjects)
            {
                if(item.TryGetComponent(out Image img)) img.sprite = Sprite.Create(GetTexture(), new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                if (item.TryGetComponent(out TMP_Text text)) SetTextGradient(_horizontal, text, startColor, endColor);
            }
        }

        public static void SetTextGradient(bool horizontal, TMP_Text text, Color startColor, Color endColor)
        {
            text.enableVertexGradient = true;
            text.colorGradient = horizontal == true ? new VertexGradient(startColor, endColor, startColor, endColor) : new VertexGradient(startColor, startColor, endColor, endColor);
        }

        public static Texture2D CreateGradientTexture(int width, int height, Color startColor, Color endColor)
        {
            Texture2D texture = new(width, height); 
            for (int x = 0; x < width; x++)
            {
                Color color = Color.Lerp(startColor, endColor, (float)x / width);
                for (int y = 0; y < height; y++) texture.SetPixel(x, y, color);
            }

            texture.Apply();
            return texture;
        }

        public static Texture2D CreateVerticalGradientTexture(int width, int height, Color startColor, Color endColor)
        {
            Texture2D texture = new (width, height);
            for (int y = 0; y < height; y++)
            {
                Color color = Color.Lerp(startColor, endColor, (float)y / height);
                for (int x = 0; x < width; x++)  texture.SetPixel(x, y, color);
            }

            texture.Apply();
            return texture;
        }

    }

    public class TextureSaver : EditorWindow
    {
        private string _name, _message;
        private Texture2D _lastTexture;
        private Image _selectedObject;

        public void GetTexture(Texture2D txt) =>_lastTexture = txt;        

        private void OnGUI()
        {
            GUILayout.Label("Enter the filename");
            _name = GUILayout.TextField(_name);
            GUILayout.Label("If object doesnt selected you save last created gradient");
            _selectedObject = EditorGUILayout.ObjectField("Select object texure",_selectedObject, typeof(Image), true) as Image;

            if (GUILayout.Button("Save lastest gradient"))
            {
                Save(_lastTexture.EncodeToPNG());
            }
            if(GUILayout.Button("Save selected image sprite"))
            {
                Texture2D texture = _selectedObject.sprite.texture;
                Save(texture.EncodeToPNG());
            }
            GUILayout.Label(_message);
        }

        private void Save(byte[] bytes)
        {
            var filename = "Assets/HelperPrefabs/" + _name + ".png";
            if (!Directory.Exists("Assets/HelperPrefabs")) Directory.CreateDirectory("Assets/HelperPrefabs");

            if (!File.Exists(filename))
            {
                File.WriteAllBytes(filename, bytes);
                _message = "Saving...";
            }
        }

    }
}
#endif