/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using InfinityCode.ProjectContextActions.UnityTypes;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ProjectContextActions.Actions
{
    [InitializeOnLoad]
    public static class CreateFolder
    {
        private static string[] _defaultNames = {
            "Animations",
            "Audio",
            "Editor",
            "Materials",
            "Models",
            "Plugins",
            "Prefabs",
            "Resources",
            "Scenes",
            "Scripts",
            "Settings",
            "Shaders",
            "StreamingAssets",
            "Textures",
            "UI",
        };

        static CreateFolder()
        {
            ItemDrawer.Register("CREATE_FOLDER", DrawButton);
        }

        private static void DrawButton(ProjectItem item)
        {
            if (!item.isFolder) return;
            if (!item.hovered) return;
            if (!item.path.StartsWith("Assets")) return;

            Rect r = item.rect;
            r.xMin = r.xMax - 18;
            r.height = 16;

            item.rect.xMax -= 18;

            GUIContent content = TempContent.Get(Icons.AddFolder, "Create Subfolder\n(Right click to select default names)");
            ButtonEvent be = GUILayoutUtils.Button(r, content, GUIStyle.none);

            if (be == ButtonEvent.Click) ProcessClick(item);
        }

        private static void OnCreateFolder(Object asset, string folderName)
        {
            Selection.activeObject = asset;
            ProjectWindowUtilsRef.CreateFolderWithTemplates(folderName, null);
        }

        private static void ProcessClick(ProjectItem item)
        {
            Event e = Event.current;
            Object asset = item.asset;
            if (asset == null) return;
            
            if (e.button == 0)
            {
                OnCreateFolder(asset, "New Folder");
            }
            else if (e.button == 1)
            {
                GenericMenu menu = new GenericMenu();

                if (item.path.EndsWith("/Scripts"))
                {
                    menu.AddItem(new GUIContent("Editor"), false, () => OnCreateFolder(asset, "Editor"));
                    menu.AddSeparator("");
                }

                for (int i = 0; i < _defaultNames.Length; i++)
                {
                    string folderName = _defaultNames[i];
                    menu.AddItem(new GUIContent(folderName), false, () => OnCreateFolder(asset, folderName));
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("New Folder"), false, () => OnCreateFolder(asset, "New Folder"));

                menu.ShowAsContext();
            }
        }
    }
}