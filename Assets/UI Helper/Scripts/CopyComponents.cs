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
            Event evt = Event.current;

            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag gameObject to edit here!", _labelStyle);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged is GameObject) _currentObject = (GameObject)dragged;
                        }
                    }
                    Event.current.Use();
                    break;
            }

            if (_currentObject != null) GUILayout.Label("Current object name : " + _currentObject.name, _microLabel);

            if (GUILayout.Button("Copy components from selected")) Copy();
            if (GUILayout.Button("Copy values from selected")) CopyValues();
            if (GUILayout.Button("Create full copy from selected")) FullCopy();
            GUILayout.Space(10);
            _withPosition = RadioButton.Draw("Copy position", _withPosition);
        }

        [MenuItem("GameObject/UI Helper/Functions/CopyObject %q", false, -1)]
        private static void FullCopy()
        {
            var selectedObjet = Selection.activeGameObject;
            var mainObj = CopyObj(selectedObjet.transform);
            if (selectedObjet.transform.parent != null) mainObj.transform.parent = selectedObjet.transform.parent;
            if (selectedObjet.transform.childCount < 1) return;
            
            foreach(Transform item in selectedObjet.transform)
            {
                var obj = CopyObj(item);
                obj.transform.parent = mainObj.transform;
            }
            mainObj.transform.position = selectedObjet.transform.position;

            Selection.activeGameObject = mainObj;
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

        private void Copy(params GameObject[] objects)
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
            rect.sizeDelta = obj.sizeDelta;

            RectTransformUtility.FlipLayoutOnAxis(rect, 0, false, false);
            RectTransformUtility.FlipLayoutOnAxis(rect, 1, false, false);

            Vector3[] corners = new Vector3[4];
            obj.GetWorldCorners(corners);
            Vector2 size = new (Mathf.Abs(corners[2].x - corners[0].x), Mathf.Abs(corners[2].y - corners[0].y));
            Vector3[] targetCorners = new Vector3[4];
            rect.GetWorldCorners(targetCorners);
            Vector2 targetSize = new (Mathf.Abs(targetCorners[2].x - targetCorners[0].x), Mathf.Abs(targetCorners[2].y - targetCorners[0].y));
            Vector2 scale = new (size.x / targetSize.x, size.y / targetSize.y);

            rect.localScale = new Vector3(scale.x, scale.y, 1f);
        }
    }
}



