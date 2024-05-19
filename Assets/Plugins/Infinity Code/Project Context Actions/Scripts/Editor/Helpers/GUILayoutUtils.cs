/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEngine;

namespace InfinityCode.ProjectContextActions
{
    public static class GUILayoutUtils
    {
        public static Rect LastRect;
        
        private static int _buttonHash = "Button".GetHashCode();
        private static int _hoveredButtonID;

        public static ButtonEvent Button(GUIContent content)
        {
            return Button(content, GUI.skin.button);
        }

        public static ButtonEvent Button(Texture texture, GUIStyle style, params GUILayoutOption[] options)
        {
            return Button(TempContent.Get(texture), style, options);
        }

        public static ButtonEvent Button(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(content, style, options);
            LastRect = rect;
            return Button(rect, content, style);
        }

        public static ButtonEvent Button(Rect rect, GUIContent content, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(_buttonHash, FocusType.Passive, rect);

            Event e = Event.current;
            bool isHover = rect.Contains(e.mousePosition);
            bool hasMouseControl = GUIUtility.hotControl == id;

            if (e.type == EventType.Repaint)
            {
                style.Draw(rect, content, id, false, isHover);
                if (isHover) return ButtonEvent.Hover;
            }
            else if (e.type == EventType.MouseDrag)
            {
                if (hasMouseControl)
                {
                    GUIUtility.hotControl = 0;
                    return ButtonEvent.Drag;
                }
            }
            else if (e.type == EventType.MouseMove)
            {
                if (isHover)
                {
                    _hoveredButtonID = id;
                    return ButtonEvent.Hover;
                }
            }
            else if (e.type == EventType.MouseDown)
            {
                if (isHover)
                {
                    Debug.unityLogger.logEnabled = false;
                    try
                    {
                        GUIUtility.hotControl = id;
                    }
                    catch
                    {
                    }

                    Debug.unityLogger.logEnabled = true;
                    e.Use();
                    return ButtonEvent.Press;
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                if (hasMouseControl)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();

                    if (isHover)
                    {
                        GUI.changed = true;
                        return ButtonEvent.Click;
                    }
                }

                return ButtonEvent.Release;
            }

            return ButtonEvent.None;
        }
    }
}