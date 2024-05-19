/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ProjectContextActions
{
    public static class Utils
    {
        public const string Version = "3.13";
        
        private static string _assetFolder;
        private static string _iconsFolder;

        public static string AssetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_assetFolder))
                {
                    string[] paths = Directory.GetFiles(Application.dataPath, "Project Context Actions.asmdef", SearchOption.AllDirectories);
                    if (paths.Length != 0)
                    {
                        FileInfo info = new FileInfo(paths[0]);
                        _assetFolder = info.Directory.Parent.FullName.Substring(Application.dataPath.Length - 6) + "/";
                    }
                    else
                    {
                        _assetFolder = "Assets/Plugins/Infinity Code/Project Context Actions/";
                    }
                }

                return _assetFolder;
            }
        }

        public static string IconsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_iconsFolder)) _iconsFolder = AssetFolder + "Icons/";

                return _iconsFolder;
            }
        }
        
        public static Texture2D LoadIcon(string path, string ext = ".png")
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(IconsFolder + path + ext);
        }
    }
}