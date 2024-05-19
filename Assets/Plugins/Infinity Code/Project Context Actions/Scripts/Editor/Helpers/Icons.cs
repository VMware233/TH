/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEngine;

namespace InfinityCode.ProjectContextActions
{
    public static class Icons
    {
        private static Texture _addFolder;

        public static Texture AddFolder
        {
            get
            {
                if (_addFolder == null) _addFolder = Utils.LoadIcon("Add-Folder");
                return _addFolder;
            }
        }
    }
}