/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using Object = UnityEngine.Object;

namespace InfinityCode.ProjectContextActions.UnityTypes
{
    public static class ProjectBrowserRef
    {
        private static Type _type;
        private static FieldInfo _assetTreeStateField;
        private static FieldInfo _folderTreeStateField;
        private static MethodInfo _isTwoColumnsMethod;

        public static Type Type
        {
            get
            {
                if (_type == null) _type = ReflectionHelper.GetEditorType("ProjectBrowser");
                return _type;
            }
        }

        private static FieldInfo AssetTreeStateField
        {
            get
            {
                if (_assetTreeStateField == null) _assetTreeStateField = Type.GetField("m_AssetTreeState", ReflectionHelper.InstanceLookup);
                return _assetTreeStateField;
            }
        }

        private static FieldInfo FolderTreeStateField
        {
            get
            {
                if (_folderTreeStateField == null) _folderTreeStateField = Type.GetField("m_FolderTreeState", ReflectionHelper.InstanceLookup);
                return _folderTreeStateField;
            }
        }
        
        private static MethodInfo IsTwoColumnsMethod
        {
            get
            {
                if (_isTwoColumnsMethod == null) _isTwoColumnsMethod = Type.GetMethod("IsTwoColumns", ReflectionHelper.InstanceLookup);
                return _isTwoColumnsMethod;
            }
        }

        public static TreeViewState GetAssetTreeViewState(Object projectWindow)
        {
            return AssetTreeStateField.GetValue(projectWindow) as TreeViewState;
        }
        
        public static TreeViewState GetFolderTreeViewState(Object projectWindow)
        {
            return FolderTreeStateField.GetValue(projectWindow) as TreeViewState;
        }
        
        public static bool IsTwoColumns(Object projectWindow)
        {
            return (bool) IsTwoColumnsMethod.Invoke(projectWindow, null);
        }
    }
}