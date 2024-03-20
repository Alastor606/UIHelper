namespace UIHelper
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class CopyComponents : EditorWindow
    {
        private GameObject _currentObject;
        private bool _withPosition = false;
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

            if (GUILayout.Button("Copy components from selected")) Copy();
            if (GUILayout.Button("Copy values from selected")) CopyValues();
            if (GUILayout.Button("Create full copy from selected")) FullCopy();
            GUILayout.Space(10);
            _withPosition = RadioButton.Draw("Copy position", _withPosition);
        }

        [MenuItem("GameObject/UI Helper/Functions/Copy Object %q", false, -1)]
        private static void FullCopy()
        {
            var selectedObjet = Selection.activeGameObject;
            var mainObj = CopyObj(selectedObjet.transform);
            if (selectedObjet.transform.parent != null) mainObj.transform.SetParent(selectedObjet.transform.parent);
            if (selectedObjet.transform.childCount < 1) return;

            CopyChild(selectedObjet.transform, mainObj.transform);
            mainObj.transform.position = selectedObjet.transform.position;

            Selection.activeGameObject = mainObj;
        }

        private static void CopyChild(Transform parent, Transform copyTo)
        {
            if(parent == null || parent.childCount <= 0) return;
            foreach (Transform item in parent.transform)
            {
                var obj = CopyObj(item);
                obj.transform.SetParent(copyTo.transform);
                CopyChild(item, obj.transform);
            }
        }

        [MenuItem("GameObject/UI Helper/Functions/CopyObject %q", true)]
        static bool ValidateMyContextFunction() =>
            Selection.activeGameObject != null;
        
        private static GameObject CopyObj(Transform objec)
        {
            List<Type> cp = new();
            foreach (Component item in objec.GetComponents<Component>()) 
                if (item.GetType() != typeof(Transform)) cp.Add(item.GetType());

            var obj = new GameObject("copyof" + objec.name, cp.ToArray());

            Component[] components = objec.GetComponents<Component>();
            if (components == null) return obj;

            foreach (Component comp in components)
            {
                foreach (Component c in obj.GetComponentsInChildren<Component>())
                {
                    if (comp.GetType() != comp.GetType()) continue;
                    ComponentUtility.CopyComponent(comp);
                    ComponentUtility.PasteComponentValues(c);
                } 
            }

            if(objec.TryGetComponent(out RectTransform rect)) CopySize(obj.GetComponent<RectTransform>(), rect); 
            else
            {
                Vector3 size = objec.GetComponent<Renderer>().bounds.size;
                var b = obj.GetComponent<Renderer>().bounds;
                b.size = size;
                obj.transform.localScale = objec.transform.localScale;
            }
            obj.transform.position = objec.transform.position;
            
            return obj;
        }

        private void CopyValues()
        {
            if (_currentObject == null) return;
            Component[] components = Selection.activeGameObject.GetComponents<Component>();
            if (components == null) return;

            if (_withPosition) _currentObject.transform.position = components[0].transform.position;

            foreach (Component comp in components)
            {
                foreach(Component c in _currentObject.GetComponentsInChildren<Component>())
                {
                    if (comp.GetType() != comp.GetType()) continue;
                    if (c.GetType() == typeof(Transform) && !_withPosition) continue;
                    ComponentUtility.CopyComponent(comp);
                    ComponentUtility.PasteComponentValues(c);
                }
            }
        }

        private void Copy()
        {
            if (_currentObject == null) return;
            Component[] components = Selection.activeGameObject.GetComponents<Component>();
            if (components == null) return;
            if (_withPosition) _currentObject.transform.position = components[0].transform.position;


            foreach (Component comp in components)
            {
                if (ComponentExistsInChildren(comp, _currentObject)) continue;

                ComponentUtility.CopyComponent(comp);
                ComponentUtility.PasteComponentAsNew(_currentObject);
            }
        }

        private bool ComponentExistsInChildren(Component comp, GameObject target)
        {
            Component[] targetComponents = target.GetComponentsInChildren<Component>();

            foreach (var targetComponent in targetComponents)
            {
                if (targetComponent.GetType() == comp.GetType())  return true;
            }

            return false;
        }

        public static void CopySize(RectTransform rect, RectTransform obj)
        {
            rect.ApplyRectTransformData(obj.CopyRectTransformData());
        }
    }
}