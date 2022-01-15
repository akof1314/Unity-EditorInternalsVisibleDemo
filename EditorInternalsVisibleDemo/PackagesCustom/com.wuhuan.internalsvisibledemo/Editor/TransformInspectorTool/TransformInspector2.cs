using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(Transform))]
    [CanEditMultipleObjects]
    internal class TransformInspector2 : Editor
    {
        SerializedProperty m_Position;
        SerializedProperty m_Scale;
        SerializedProperty m_Rotation;
        TransformRotationGUI m_RotationGUI;
        private static Vector3 s_ClipBoardVector3;

        class Contents
        {
            public GUIContent positionContent = EditorGUIUtility.TrTextContent("P", "The local position of this GameObject relative to the parent.");
            public GUIContent scaleContent = EditorGUIUtility.TrTextContent("S", "The local scaling of this GameObject relative to the parent.");
            public GUIContent rotationContent = EditorGUIUtility.TrTextContent("R", "The local rotation of this GameObject relative to the parent.");
            public string floatingPointWarning = LocalizationDatabase.GetLocalizedString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");
            public GUIContent copyContent = EditorGUIUtility.TrTextContent("C", "Copy");
            public GUIContent pasteContent = EditorGUIUtility.TrTextContent("V", "Paste");
        }
        static Contents s_Contents;

        public void OnEnable()
        {
            m_Position = serializedObject.FindProperty("m_LocalPosition");
            m_Scale = serializedObject.FindProperty("m_LocalScale");
            m_Rotation = serializedObject.FindProperty("m_LocalRotation");

            if (m_RotationGUI == null)
                m_RotationGUI = new TransformRotationGUI();
            m_RotationGUI.OnEnable(m_Rotation, GUIContent.none);
        }

        public override void OnInspectorGUI()
        {
            if (s_Contents == null)
                s_Contents = new Contents();

            if (!EditorGUIUtility.wideMode)
            {
                EditorGUIUtility.wideMode = true;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
            }

            serializedObject.Update();

            Inspector3D();
            // Warning if global position is too large for floating point errors.
            // SanitizeBounds function doesn't even support values beyond 100000
            Transform t = target as Transform;
            Vector3 pos = t.position;
            if (Mathf.Abs(pos.x) > 100000 || Mathf.Abs(pos.y) > 100000 || Mathf.Abs(pos.z) > 100000)
                EditorGUILayout.HelpBox(s_Contents.floatingPointWarning, MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }

        private void Inspector3D()
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth - 20;
            GUILayout.BeginHorizontal();
            bool reset = GUILayout.Button(s_Contents.positionContent, GUILayout.Width(20f));
            EditorGUILayout.PropertyField(m_Position, GUIContent.none);
            if (reset)
            {
                m_Position.vector3Value = Vector3.zero;
            }
            ShowCopyPaste(m_Position, false);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            reset = GUILayout.Button(s_Contents.rotationContent, GUILayout.Width(20f));
            m_RotationGUI.RotationField();
            if (reset)
            {
                m_Rotation.quaternionValue = Quaternion.identity;
            }
            ShowCopyPaste(m_Rotation, true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            reset = GUILayout.Button(s_Contents.scaleContent, GUILayout.Width(20f));
            EditorGUILayout.PropertyField(m_Scale, GUIContent.none);
            if (reset)
            {
                m_Scale.vector3Value = Vector3.one;
            }
            ShowCopyPaste(m_Scale, false);
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = labelWidth;
        }

        private void ShowCopyPaste(SerializedProperty serializedProperty, bool isRot)
        {
            bool reset = GUILayout.Button(s_Contents.copyContent, GUILayout.Width(21f));
            if (reset)
            {
                if (isRot)
                {
                    s_ClipBoardVector3 = serializedProperty.quaternionValue.eulerAngles;
                }
                else
                {
                    s_ClipBoardVector3 = serializedProperty.vector3Value;
                }
            }
            reset = GUILayout.Button(s_Contents.pasteContent, GUILayout.Width(20f));
            if (reset)
            {
                if (isRot)
                {
                    serializedProperty.quaternionValue = Quaternion.Euler(s_ClipBoardVector3);
                }
                else
                {
                    serializedProperty.vector3Value = s_ClipBoardVector3;
                }
            }
        }

        [EditorHeaderItem(typeof(Object), -1031)]
        public static bool DrawHeaderCopyButton(Rect rectangle, Object[] targets)
        {
            var target = targets[0];
            if (!(target is Transform) || targets.Length > 1)
            {
                return false;
            }
            
            if (s_Contents == null)
                s_Contents = new Contents();
            if (EditorGUI.Button(rectangle, s_Contents.copyContent))
            {
                ComponentUtility.CopyComponent(target as Component);
            }
            return true;
        }

        [EditorHeaderItem(typeof(Object), -1032)]
        public static bool DrawHeaderPasteButton(Rect rectangle, Object[] targets)
        {
            var target = targets[0];
            if (!(target is Transform))
            {
                return false;
            }
            
            if (s_Contents == null)
                s_Contents = new Contents();
            if (EditorGUI.Button(rectangle, s_Contents.pasteContent))
            {
                foreach (var o in targets)
                {
                    if (o is Transform)
                    {
                        ComponentUtility.PasteComponentValues(o as Component);
                    }
                }
            }
            return true;
        }
    }
}
