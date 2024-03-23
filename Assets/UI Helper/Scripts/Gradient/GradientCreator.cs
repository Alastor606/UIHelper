#if UNITY_EDITOR
namespace UIHelper
{
    using TMPro;
    using UnityEngine;
    public static class GradientCreator
    {

        public static Texture2D Generate(int width, int height, Color startColor, Color endColor, float angle)
        {
            Texture2D texture = new (width, height);
            var dir1 = GetDirection(angle);
            var dir2 = GetDirection(angle);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float t = Mathf.InverseLerp(0, width, y + Mathf.Cos(Mathf.Deg2Rad * angle) * width);
                    Vector2 dir = Vector2.Lerp(dir1, dir2, t);
                    float colorT = Vector2.Dot(new Vector2(x, y), dir) / Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2));
                    Color color = Color.Lerp(startColor, endColor, colorT);
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }

        private static Vector2 GetDirection(float angle)
        {
            float radAngle = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
        }

        public static Texture2D Horizontal(int width, int height, Color startColor, Color endColor)
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

        public static Texture2D Vertical(int width, int height, Color startColor, Color endColor)
        {
            Texture2D texture = new(width, height);
            for (int y = 0; y < height; y++)
            {
                Color color = Color.Lerp(startColor, endColor, (float)y / height);
                for (int x = 0; x < width; x++) texture.SetPixel(x, y, color);
            }

            texture.Apply();
            return texture;
        }

        public static Texture2D Radial(int width, int height, Color startColor, Color endColor)
        {
            var texture = new Texture2D(width, height);

            Vector2 center = new (width / 2f, height / 2f);
            float maxRadius = width / 2f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float distance = Vector2.Distance(center, new Vector2(x, y));
                    float normalizedDistance = distance / maxRadius;
                    Color color = Color.Lerp(startColor, endColor, normalizedDistance);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        public static void Text(bool horizontal, TMP_Text text, Color startColor, Color endColor)
        {
            text.enableVertexGradient = true;
            text.colorGradient = horizontal == true ? new VertexGradient(startColor, endColor, startColor, endColor) : new VertexGradient(startColor, startColor, endColor, endColor);
        }
    }
}
#endif