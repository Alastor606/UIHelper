
#if UNITY_EDITOR

namespace UIHelper
{
    using UnityEditor;
    using UnityEditor.SceneManagement;
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
                    PrefabUtility.InstantiatePrefab(prefab, item.transform);
                    break;
                }
            }
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            EditorSceneManager.MarkSceneDirty(prefabStage == null
                ? UnityEngine.SceneManagement.SceneManager.GetActiveScene()
                : prefabStage.scene);
        }

        [MenuItem("Custom UI/Button")]
        public static void SpawnButton()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<CustomButton>("Assets/UI Helper/Prefabs/CustomButton.prefab");
            if (Selection.gameObjects.Length < 1)
            {
                PrefabUtility.InstantiatePrefab(prefab);
                return;
            }
            Spawn(prefab);
        }

        [MenuItem("Custom UI/RadioButton")]
        public static void SpawnRadioButton()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<RadioButton>("Assets/UI Helper/Prefabs/RadioButton.prefab");
            if (Selection.gameObjects.Length < 1)
            {
                PrefabUtility.InstantiatePrefab(prefab);
                return;
            }
            Spawn(prefab);
        }

        [MenuItem("Custom UI/Radio Group")]
        public static void SpawRadioGroup()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<RadioGroup>("Assets/UI Helper/Prefabs/Radio Group.prefab");
            if (Selection.gameObjects.Length < 1)
            { 
                PrefabUtility.InstantiatePrefab(prefab);
                return;
            }
            Spawn(prefab);
        }
    }
#endif
}

