#if UNITY_EDITOR
namespace UIHelper
{
    using System.IO;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.U2D;
    using UnityEngine.UI;

    public class GradientGenerator : EditorWindow
    {

        public Color startColor = Color.red;
        public Color endColor = Color.blue;
        public int width = 200;
        public int height = 50;
        private Texture2D _lastTexture;

        private bool _horizontal = true, _vertical;

        [MenuItem("UI Helper/Gradient")]
        public static void ShowWindow()
        {
            var window = GetWindow<GradientGenerator>("Gradient generator");
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
            width = Mathf.Clamp(width,0, 1000);
            height = Mathf.Clamp(height, 0, 1000);
            if (GUILayout.Button("Set gradient to selected"))  SetTexture();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Swap Colors")) SwapColors();
            if (GUILayout.Button("Save Texture")) Save();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("Copy colors from selected image");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy horizontal gradient")) CopyGradientHorizontal();
            if (GUILayout.Button("Copy vertical gradient")) CopyGradientVertical();
            GUILayout.EndHorizontal();

            GUI.DrawTexture(new Rect(10, 250, 60, 60), GetTexture());
            
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
        

        void SetTexture()
        {
            foreach(var item in Selection.gameObjects)
            {
                if(item.TryGetComponent(out Image img)) img.sprite = Sprite.Create(GetTexture(), new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                if(item.TryGetComponent(out TMP_Text text))
                {
                    text.enableVertexGradient = true;
                    text.colorGradient = _horizontal == true ? new VertexGradient(startColor, endColor, startColor, endColor) : new VertexGradient(startColor, startColor, endColor, endColor);
                }
            }
        }

        public static Texture2D CreateGradientTexture(int width, int height, Color startColor, Color endColor)
        {
            Texture2D texture = new(width, height); 

            for (int x = 0; x < width; x++)
            {
                Color color = Color.Lerp(startColor, endColor, (float)x / width);
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
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
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, color);
                }
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