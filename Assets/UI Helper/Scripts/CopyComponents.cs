namespace UIHelper
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class CopyComponents : EditorWindow
    {
        private GameObject _copyFrom, _copyTo;
        private static bool _withPosition = false;
        private GUIStyle _labelStyle = new (), _microLabel = new();
        private string[] _states = new string[] { "Components", "Components values", "Full copy"};
        private int _selectedIndex;

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
            _labelStyle.fontSize = 16;
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("Copy components", _labelStyle);
            _copyFrom = EditorGUILayout.ObjectField("Copy from", _copyFrom, typeof(GameObject), true) as GameObject;
            _copyTo = EditorGUILayout.ObjectField("Copy to", _copyTo, typeof(GameObject), true) as GameObject;
            _withPosition = RadioButton.Draw("Copy position", _withPosition);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Select copy type : ");
            Rect popupRect = EditorGUILayout.GetControlRect(GUILayout.Width(Screen.width / 2));
            _selectedIndex = EditorGUI.Popup(popupRect,_selectedIndex,_states);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (GUI.Button(new Rect(new Vector2(Screen.width / 4f, Screen.height / 8), new Vector2(200, 25)),"Copy!"))
            {
                if(_selectedIndex == 0)Copy(_copyFrom, _copyTo == null ? Selection.activeGameObject : _copyTo);
                else if (_selectedIndex == 1) CopyValues(_copyFrom, _copyTo == null ? Selection.activeGameObject : _copyTo);
                else FullCopy(_copyFrom, _copyTo == null ? Selection.activeGameObject == null ? new GameObject("C" + _copyFrom.name) : Selection.activeGameObject : _copyTo);
            }
        }

        private static void FullCopy(GameObject copyFrom, GameObject copyTo)
        {
            if (copyFrom.Equals(copyTo))
            {
                FullCopy(copyFrom, new GameObject("C" + copyFrom.name));
                return;
            }
            if (copyFrom.transform.parent != null) copyTo.transform.SetParent(copyFrom.transform.parent);
            Copy(copyFrom, copyTo);
            CopyValues(copyFrom, copyTo);
            if (copyFrom.transform.childCount > 0)
            {
                foreach (Transform item in copyFrom.transform) CopyChild(item.gameObject, copyTo.transform);
            }
        }

        [MenuItem("GameObject/UI Helper/Functions/CopyObject %q", false, -1)]
        private static void FullCopy()
        {
            var selectedObj = Selection.activeGameObject;
            var obj = new GameObject("C" + selectedObj.name);
            FullCopy(selectedObj, obj);
        }

        private static void CopyChild(GameObject copyFrom, Transform parent)
        {
            var obj = new GameObject("C" + copyFrom.name);
            obj.transform.SetParent(parent);
            Copy(copyFrom, obj);
            CopyValues(copyFrom, obj);
            if (copyFrom.transform.childCount > 0)
            {
                foreach (Transform item in copyFrom.transform) CopyChild(item.gameObject, obj.transform);
            }
        }


        [MenuItem("GameObject/UI Helper/Functions/CopyObject %q", true)]
        static bool ValidateMyContextFunction() =>
            Selection.activeGameObject != null;

        private static void CopyValues(GameObject copyFrom, GameObject coyTo)
        {
            if (coyTo == null) return;
            Component[] components = copyFrom.GetComponents<Component>();
            if (components == null) return;

            if (coyTo) coyTo.transform.position = components[0].transform.position;

            foreach (Component comp in components)
            {
                foreach(Component c in coyTo.GetComponentsInChildren<Component>())
                {
                    if (comp.GetType() != comp.GetType()) continue;
                    if (c.GetType() == typeof(Transform) && !_withPosition) continue;
                    ComponentUtility.CopyComponent(comp);
                    ComponentUtility.PasteComponentValues(c);
                }
            }
        }

        private static void Copy(GameObject copyFrom,GameObject copyTo)
        {
            if (copyTo == null) return;
            Component[] components = copyFrom.GetComponents<Component>();
            if (components == null) return;
            if (_withPosition) copyTo.transform.position = components[0].transform.position;


            foreach (Component comp in components)
            {
                if (ComponentExistsInChildren(comp, copyTo)) continue;

                ComponentUtility.CopyComponent(comp);
                ComponentUtility.PasteComponentAsNew(copyTo);
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