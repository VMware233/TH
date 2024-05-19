/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEditor;
using UnityEngine;

namespace InfinityCode.ProjectContextActions
{
    public static class EditorIconContents
    {
        private static GUIContent _csScript;
        private static GUIContent _material;
        private static GUIContent _playButtonOn;
        private static GUIContent _preAudioPlayOn;

        public static GUIContent CsScript
        {
            get
            {
                if (_csScript == null)
                {
                    _csScript = EditorGUIUtility.IconContent("cs Script Icon");
                }
                return _csScript;
            }
        }

        public static GUIContent Material
        {
            get
            {
                if (_material == null)
                {
                    _material = EditorGUIUtility.IconContent("Material Icon");
                }
                return _material;
            }
        }

        public static GUIContent PlayButtonOn
        {
            get
            {
                if (_playButtonOn == null)
                {
                    _playButtonOn = EditorGUIUtility.IconContent("d_PlayButton On");
                }
                return _playButtonOn;
            }
        }

        public static GUIContent PreAudioPlayOn
        {
            get
            {
                if (_preAudioPlayOn == null)
                {
                    _preAudioPlayOn = EditorGUIUtility.IconContent("preAudioPlayOn");
                }
                return _preAudioPlayOn;
            }
        }
    }
}