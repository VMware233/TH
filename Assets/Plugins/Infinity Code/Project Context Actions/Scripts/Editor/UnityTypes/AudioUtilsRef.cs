/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEngine;

namespace InfinityCode.ProjectContextActions.UnityTypes
{
    public static class AudioUtilsRef
    {
        private static MethodInfo _isClipPlayingMethod;
        private static MethodInfo _playClipMethod;
        private static MethodInfo _stopAllClipsMethod;
        private static MethodInfo _stopClipMethod;
        private static Type _type;

        private static MethodInfo IsClipPlayingMethod
        {
            get
            {
                if (_isClipPlayingMethod == null)
                {
#if UNITY_2020_2_OR_NEWER
                    _isClipPlayingMethod = Type.GetMethod("IsPreviewClipPlaying", ReflectionHelper.StaticLookup);
#else
                    _isClipPlayingMethod = type.GetMethod("IsClipPlaying", Reflection.StaticLookup, null, new[] { typeof(AudioClip) }, null);
#endif
                }
                return _isClipPlayingMethod;
            }
        }

        private static MethodInfo PlayClipMethod
        {
            get
            {
                if (_playClipMethod == null)
                {
#if UNITY_2020_2_OR_NEWER
                    _playClipMethod = Type.GetMethod("PlayPreviewClip", ReflectionHelper.StaticLookup, null, new[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
#else
                    _playClipMethod = type.GetMethod("PlayClip", Reflection.StaticLookup, null, new[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
#endif
                }
                return _playClipMethod;
            }
        }

        private static MethodInfo StopAllClipsMethod
        {
            get
            {
                if (_stopAllClipsMethod == null)
                {
#if UNITY_2020_2_OR_NEWER
                    _stopAllClipsMethod = Type.GetMethod("StopAllPreviewClips", ReflectionHelper.StaticLookup);
#else
                    _stopAllClipsMethod = type.GetMethod("StopAllClips", Reflection.StaticLookup);
#endif
                }
                return _stopAllClipsMethod;
            }
        }

        private static MethodInfo StopClipMethod
        {
            get
            {
                if (_stopClipMethod == null)
                {
#if UNITY_2020_2_OR_NEWER
                    _stopClipMethod = Type.GetMethod("PausePreviewClip", ReflectionHelper.StaticLookup);
#else
                    _stopClipMethod = type.GetMethod("StopClip", Reflection.StaticLookup, null, new[] { typeof(AudioClip) }, null);
#endif
                }
                return _stopClipMethod;
            }
        }

        public static Type Type
        {
            get
            {
                if (_type == null) _type = ReflectionHelper.GetEditorType("AudioUtil");
                return _type;
            }
        }

        public static bool IsClipPlaying(AudioClip clip)
        {
#if UNITY_2020_2_OR_NEWER
            return (bool)IsClipPlayingMethod.Invoke(null, Array.Empty<object>());
#else
            return (bool)isClipPlayingMethod.Invoke(null, new object[] {clip});
#endif
        }

        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            PlayClipMethod.Invoke(null, new object[] { clip, startSample, loop});
        }

        public static void StopAllClips()
        {
            StopAllClipsMethod.Invoke(null, null);
        }

        public static void StopClip(AudioClip clip)
        {
#if UNITY_2020_2_OR_NEWER
            StopClipMethod.Invoke(null, Array.Empty<object>());
#else 
            stopClipMethod.Invoke(null, new object[] {clip});
#endif
        }
    }
}