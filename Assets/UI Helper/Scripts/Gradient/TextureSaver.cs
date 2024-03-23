#if UNITY_EDITOR
namespace UIHelper
{
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEngine;
    using System.IO;
    using System.Threading.Tasks;

    public class TextureSaver : EditorWindow
    {
        private string _name;
        private Texture2D _lastTexture;
        private Image _selectedObject;
        private static string _message = "";

        public void GetTexture(Texture2D txt) => _lastTexture = txt;

        private void OnGUI()
        {
            GUILayout.Label("Enter the filename");
            _name = GUILayout.TextField(_name);
            _selectedObject = EditorGUILayout.ObjectField("Select object texure", _selectedObject, typeof(Image), true) as Image;

            if (GUILayout.Button("Save gradient")) Save(_name, _lastTexture.EncodeToPNG());
            
            if (GUILayout.Button("Save selected image sprite"))
            {
                Texture2D texture = _selectedObject.sprite.texture;
                Save(_name, texture.EncodeToPNG());
            }
            GUILayout.Label(_message);
        }

        public async static void Save(string name, byte[] bytes)
        {
            var filename = "Assets/HelperPrefabs/" + name + ".png";
            if (!Directory.Exists("Assets/HelperPrefabs")) Directory.CreateDirectory("Assets/HelperPrefabs");
            _message = "File creating";
            if (!File.Exists(filename))
            {
                var file = File.WriteAllBytesAsync(filename, bytes); 
                while (!file.IsCompleted)
                {
                    await Task.Yield();
                } 
                _message = $"File created in `{filename}`!";
                AssetDatabase.Refresh();
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(filename);
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            else _message = "File exists, please change the file name";
        }

    }
}
#endif