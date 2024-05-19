/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ProjectContextActions
{
    public class AssetCache
    {
        private static Dictionary<string, Object> _assets = new Dictionary<string, Object>();
        
        public static T Get<T>(string path) where T : Object
        {
            Object asset;
            if (_assets.TryGetValue(path, out asset)) return (T) asset;
            
            asset = AssetDatabase.LoadAssetAtPath<T>(path);
            _assets[path] = asset;
            return (T) asset;
        }

    }
}