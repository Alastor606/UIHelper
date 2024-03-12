#if UNITY_EDITOR
namespace UIHelper
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;
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
            GUI.DrawTexture(new Rect(10, 150, 40, 40), GetTexture());
            
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
            if (_horizontal) gradientTexture = CreateGradientTexture();
            else gradientTexture = CreateVerticalGradientTexture();
            _lastTexture = gradientTexture;
            return gradientTexture;
        }
        

        void SetTexture()
        {
            foreach(var item in Selection.gameObjects)
            {
                if(item.TryGetComponent(out Image img)) img.sprite = Sprite.Create(GetTexture(), new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
            }
        }

        private Texture2D CreateGradientTexture()
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

        private Texture2D CreateVerticalGradientTexture()
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

        public void GetTexture(Texture2D txt) =>_lastTexture = txt;        

        private void OnGUI()
        {
            GUILayout.Label("Enter the filename");
            _name = GUILayout.TextField(_name);

            if (GUILayout.Button("Save"))
            {
                byte[] bytes = _lastTexture.EncodeToPNG();
                var filename = "Assets/HelperPrefabs/" + _name + ".png";
                if (!Directory.Exists("Assets/HelperPrefabs"))Directory.CreateDirectory("Assets/HelperPrefabs");

                if (!File.Exists(filename))
                { 
                    File.WriteAllBytes(filename, bytes);
                    _message = "Saving..."; 
                }
                
            }
            GUILayout.Label(_message);
        }

    }
}


#endif