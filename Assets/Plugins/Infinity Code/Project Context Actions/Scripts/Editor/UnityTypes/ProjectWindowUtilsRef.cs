/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor;

namespace InfinityCode.ProjectContextActions.UnityTypes
{
    public static class ProjectWindowUtilsRef
    {
        private static MethodInfo _createFolderWithTemplatesMethod;

        private static MethodInfo CreateFolderWithTemplatesMethod
        {
            get
            {
                if (_createFolderWithTemplatesMethod == null) _createFolderWithTemplatesMethod = Type.GetMethod("CreateFolderWithTemplates", ReflectionHelper.StaticLookup, null, new[] { typeof(string), typeof(string[]) }, null);
                return _createFolderWithTemplatesMethod;
            }
        }

        public static Type Type
        {
            get
            {
                return typeof(ProjectWindowUtil);
            }
        }

        public static void CreateFolderWithTemplates(string defaultName, params string[] templates)
        {
            CreateFolderWithTemplatesMethod.Invoke(null, new object[] { defaultName, templates });
        }
    }
}