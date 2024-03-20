
#if UNITY_EDITOR

namespace UIHelper
{
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    internal static class UISpawner
    {

        internal static void Spawn(string path, Transform parent = null)
        {
            
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(parent != null)
            {
                PrefabUtility.InstantiatePrefab(prefab, parent);
                EditorUtility.SetDirty(parent);
                return;
            }

            if (Selection.gameObjects.Length < 1)
            {
                PrefabUtility.InstantiatePrefab(prefab);
                return;
            }
             
            foreach (var item in Selection.gameObjects)
            {
                if (!item.TryGetComponent(out Canvas _) && item.GetComponentInParent<Canvas>() == null) continue;

                if (prefab != null)
                {
                    PrefabUtility.InstantiatePrefab(prefab, item.transform);
                    EditorUtility.SetDirty(item);
                    break;
                }
            }
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            EditorSceneManager.MarkSceneDirty(prefabStage == null
                ? UnityEngine.SceneManagement.SceneManager.GetActiveScene()
                : prefabStage.scene);
        }



        [MenuItem("GameObject/UI Helper/Button", false, 1)] 
        private static void SpawnButton() =>
            Spawn("Assets/UI Helper/Prefabs/CustomButton.prefab");

        [MenuItem("GameObject/UI Helper/RadioButton", false, 2)]
        private static void SpawnRadioButton() =>
            Spawn("Assets/UI Helper/Prefabs/RadioButton.prefab");

        [MenuItem("GameObject/UI Helper/Radio Group")]
        private static void SpawRadioGroup() =>
            Spawn("Assets/UI Helper/Prefabs/Radio Group.prefab");

        [MenuItem("GameObject/UI Helper/Scroll View")]
        private static void SpawnView() =>
            Spawn("Assets/UI Helper/Prefabs/CustomView.prefab");
    }
}
#endif


