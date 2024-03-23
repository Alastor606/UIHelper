#if UNITY_EDITOR
namespace UIHelper
{
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEngine;
    using System.IO;

    public class TextureSaver : EditorWindow
    {
        private string _name;
        private Texture2D _lastTexture;
        private Image _selectedObject;

        public void GetTexture(Texture2D txt) => _lastTexture = txt;

        private void OnGUI()
        {
            GUILayout.Label("Enter the filename");
            _name = GUILayout.TextField(_name);
            _selectedObject = EditorGUILayout.ObjectField("Select object texure", _selectedObject, typeof(Image), true) as Image;

            if (GUILayout.Button("Save lastest gradient"))
            {
                Save(_name, _lastTexture.EncodeToPNG());
            }
            if (GUILayout.Button("Save selected image sprite"))
            {
                Texture2D texture = _selectedObject.sprite.texture;
                Save(_name, texture.EncodeToPNG());
            }
        }

        public static void Save(string name, byte[] bytes)
        {
            var filename = "Assets/HelperPrefabs/" + name + ".png";
            if (!Directory.Exists("Assets/HelperPrefabs")) Directory.CreateDirectory("Assets/HelperPrefabs");

            if (!File.Exists(filename))
            {
                File.WriteAllBytes(filename, bytes);
            }
        }

    }
}
#endif