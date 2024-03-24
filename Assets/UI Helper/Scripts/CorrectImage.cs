#if UNITY_EDITOR
namespace UIHelper
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class CorrectImage : EditorWindow
    {
        private Image _imageToRound;
        private float _cornerRadius;

        public static void Init()
        {
            Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            var window = GetWindow<CorrectImage>("Correct Image", new Type[] { inspectorType });
            window.minSize = new Vector2(400, 475);
        }

        private void OnGUI()
        {
            _imageToRound = EditorGUILayout.ObjectField(_imageToRound, typeof(Image), true) as Image;
            _cornerRadius = EditorGUILayout.Slider("Corner angle",_cornerRadius, 0, 360);
            if (GUILayout.Button("Round corners"))
            {
                RoundCorners();
            }
        }

        private void RoundCorners()
        {
            Sprite oldSprite = _imageToRound.sprite;
            Texture2D texture = new Texture2D((int)oldSprite.rect.width, (int)oldSprite.rect.height);
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, oldSprite.rect.width, oldSprite.rect.height), new Vector2(0.5f, 0.5f));

            Color[] pixels = oldSprite.texture.GetPixels((int)oldSprite.rect.x, (int)oldSprite.rect.y, (int)oldSprite.rect.width, (int)oldSprite.rect.height);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(pixels);
            texture.Apply();
            _imageToRound.sprite = newSprite;

            float xMul = texture.width / _cornerRadius;
            float yMul = texture.height / _cornerRadius;

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color pixel = texture.GetPixel(x, y);
                    float xDist = Mathf.Abs(x - texture.width);
                    float yDist = Mathf.Abs(y - texture.height);

                    if (xDist >= xMul && yDist >= yMul) pixel.a = 0;
                    
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
        }
    }
}

#endif