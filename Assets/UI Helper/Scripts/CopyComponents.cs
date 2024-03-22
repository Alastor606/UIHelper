namespace UIHelper
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class CopyComponents : EditorWindow
    {
        private GameObject _currentObject;
        private static bool _withPosition = false;
        private GUIStyle _labelStyle = new (), _microLabel = new();

        [MenuItem("UI Helper/Copy components")]
        public static void ShowWindow()
        {
            var window = GetWindow<CopyComponents>("Copy Components");
            window.minSize = new Vector2(400, 475);
            
        }

        private void OnGUI()
        {
            _labelStyle.normal.textColor= Color.white;
            _microLabel.normal.textColor = Color.white;
            _labelStyle.fontSize = 20;
            _labelStyle.alignment = TextAnchor.MiddleCenter;
            _currentObject = EditorGUILayout.ObjectField("Select the object", _currentObject, typeof(GameObject), true) as GameObject;

            if (_currentObject != null) GUILayout.Label("Current object name : " + _currentObject.name, _microLabel);

            if (GUILayout.Button("Copy components from selected")) Copy(_currentObject, Selection.activeGameObject);
            if (GUILayout.Button("Copy values from selected")) CopyValues(_currentObject, Selection.activeGameObject);
            if (GUILayout.Button("Create full copy from selected (ctrl + q)")) FullCopy();
            GUILayout.Space(10);
            _withPosition = RadioButton.Draw("Copy position", _withPosition);
        }

        [MenuItem("GameObject/UI Helper/Functions/CopyObject %q", false, -1)]
        private static void FullCopy()
        {
            var selectedObj = Selection.activeGameObject;
            var obj = new GameObject("C" + selectedObj.name);
            if(selectedObj.transform.parent != null) obj.transform.SetParent(selectedObj.transform.parent);
            Copy(obj, selectedObj);
            CopyValues(obj, selectedObj);
            if (selectedObj.transform.childCount > 0)
            {
                foreach (Transform item in selectedObj.transform) CopyChild(item.gameObject, obj.transform);
            }
        }

        private static void CopyChild(GameObject copyFrom, Transform parent)
        {
            var obj = new GameObject("C" + copyFrom.name);
            obj.transform.SetParent(parent);
            Copy(obj, copyFrom);
            CopyValues(obj, copyFrom);
            if (copyFrom.transform.childCount > 0)
            {
                foreach (Transform item in copyFrom.transform) CopyChild(item.gameObject, obj.transform);
            }
        }


        [MenuItem("GameObject/UI Helper/Functions/CopyObject %q", true)]
        static bool ValidateMyContextFunction() =>
            Selection.activeGameObject != null;

        private static void CopyValues(GameObject currentObject, GameObject copyFrom)
        {
            if (currentObject == null) return;
            Component[] components = copyFrom.GetComponents<Component>();
            if (components == null) return;

            if (currentObject) currentObject.transform.position = components[0].transform.position;

            foreach (Component comp in components)
            {
                foreach(Component c in currentObject.GetComponentsInChildren<Component>())
                {
                    if (comp.GetType() != comp.GetType()) continue;
                    if (c.GetType() == typeof(Transform) && !_withPosition) continue;
                    ComponentUtility.CopyComponent(comp);
                    ComponentUtility.PasteComponentValues(c);
                }
            }
        }

        private static void Copy(GameObject currentObject, GameObject copyFrom)
        {
            if (currentObject == null) return;
            Component[] components = copyFrom.GetComponents<Component>();
            if (components == null) return;
            if (_withPosition) currentObject.transform.position = components[0].transform.position;


            foreach (Component comp in components)
            {
                if (ComponentExistsInChildren(comp, currentObject)) continue;

                ComponentUtility.CopyComponent(comp);
                ComponentUtility.PasteComponentAsNew(currentObject);
            }
        }

        private static bool ComponentExistsInChildren(Component comp, GameObject target)
        {
            Component[] targetComponents = target.GetComponentsInChildren<Component>();

            foreach (var targetComponent in targetComponents)
            {
                if (targetComponent.GetType() == comp.GetType())  return true;
            }

            return false;
        }

    }
}