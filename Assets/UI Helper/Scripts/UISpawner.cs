
#if UNITY_EDITOR

namespace UIHelper
{
    using UnityEditor;
    using UnityEngine;

    public class UISpawner
    {

        private static void Spawn(Object prefab)
        {
            foreach (var item in Selection.gameObjects)
            {
                if (!item.TryGetComponent(out Canvas _) && item.GetComponentInParent<Canvas>() == null) continue;

                if (prefab != null)
                {
                    Object.Instantiate(prefab, item.transform);
                    break;
                }

            }
        }

        [MenuItem("UI Helper/Custom Button")]
        public static void SpawnButton()
        {
            var prefab = Resources.FindObjectsOfTypeAll(typeof(CustomButton))[0];
            if (Selection.gameObjects.Length < 1)
            {
                Object.Instantiate(prefab);
                return;
            }
            Spawn(prefab);
        }

        [MenuItem("UI Helper/RadioButton")]
        public static void SpawnRadioButton()
        {
            var prefab = Resources.FindObjectsOfTypeAll(typeof(RadioButton))[0];
            if (Selection.gameObjects.Length < 1)
            {
                Object.Instantiate(prefab);
                return;
            }
            Spawn(prefab);
        }

        [MenuItem("UI Helper/Radio Group")]
        public static void SpawRadioGroup()
        {
            var prefab = Resources.FindObjectsOfTypeAll(typeof(RadioGroup))[0];
            if (Selection.gameObjects.Length < 1)
            {
                Object.Instantiate(prefab);
                return;
            }
            Spawn(prefab);
        }
    }
#endif
}

