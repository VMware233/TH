/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace InfinityCode.ProjectContextActions
{
    public static class ReflectionHelper
    {
        public const BindingFlags InstanceLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags StaticLookup = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private static Dictionary<Assembly, Type[]> _cache;
        private static Assembly _editorAssembly;

        public static Assembly EditorAssembly
        {
            get
            {
                if (_editorAssembly == null) _editorAssembly = Assembly.Load("UnityEditor");
                return _editorAssembly;
            }
        }

        public static Type GetEditorType(string name, string @namespace = "UnityEditor")
        {
            return EditorAssembly.GetType(@namespace + "." + name);
        }
    }
}