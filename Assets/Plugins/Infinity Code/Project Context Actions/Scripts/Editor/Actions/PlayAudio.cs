/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.ProjectContextActions.UnityTypes;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ProjectContextActions.Actions
{
    [InitializeOnLoad]
    public static class PlayAudio
    {
        private static AudioClip _clip;

        static PlayAudio()
        {
            ItemDrawer.Register("PLAY_AUDIO", DrawButton, 10);
        }

        private static void DrawButton(ProjectItem item)
        {
            Object asset = item.asset;
            if (asset == null) return;
            if (!(asset is AudioClip)) return;
            
            bool isPlaying = _clip == asset;
            
            if (isPlaying)
            {
                if (!AudioUtilsRef.IsClipPlaying(_clip))
                {
                    isPlaying = false;
                    _clip = null;
                }
            }
            
            if (!item.hovered && !isPlaying) return;
            
            Rect r = item.rect;
            r.xMin = r.xMax - 18;
            r.height = 16;

            item.rect.xMax -= 18;

            Texture icon;
            string tooltip;
            if (isPlaying)
            {
                icon = EditorIconContents.PreAudioPlayOn.image;
                tooltip = "Stop Audio";
            }
            else
            {
                icon = EditorIconContents.PlayButtonOn.image;
                tooltip = "Play Audio";
            }

            GUIContent content = TempContent.Get(icon, tooltip);
            ButtonEvent be = GUILayoutUtils.Button(r, content, GUIStyle.none);
            if (be == ButtonEvent.Click) ProcessClick(item, isPlaying);
        }

        private static void ProcessClick(ProjectItem item, bool isPlaying)
        {
            Event e = Event.current;
            if (e.button == 0)
            {
                if (isPlaying)
                {
                    AudioUtilsRef.StopAllClips();
                    _clip = null;
                }
                else
                {
                    AudioUtilsRef.StopAllClips();
                    _clip = item.asset as AudioClip;
                    AudioUtilsRef.PlayClip(_clip);
                }
            }
        }
    }
}