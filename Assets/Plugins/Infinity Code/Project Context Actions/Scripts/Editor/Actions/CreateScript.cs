/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ProjectContextActions.Actions
{
    [InitializeOnLoad]
    public static class CreateScript
    {
        static CreateScript()
        {
            ItemDrawer.Register("CREATE_SCRIPT", DrawButton, 10);
        }

        private static void DrawButton(ProjectItem item)
        {
            if (!item.isFolder) return;
            if (!item.hovered) return;
            if (!item.path.StartsWith("Assets")) return;
            if (!item.path.Contains("Scripts")) return;

            Rect r = item.rect;
            r.xMin = r.xMax - 18;
            r.height = 16;

            item.rect.xMax -= 18;

            ButtonEvent be = GUILayoutUtils.Button(r, TempContent.Get(EditorIconContents.CsScript.image, "Create Script\n(Right click to select a template)"), GUIStyle.none);
            if (be == ButtonEvent.Click) ProcessClick(item);
        }

        private static void OnCreateScript(object userdata)
        {
            object[] data = (object[])userdata;
            Object asset = data[0] as Object;
            string name = (string)data[1];
            string path = null;

            string[] files = Directory.GetFiles(Utils.AssetFolder + "LocalResources/ScriptTemplates/", "*.txt", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file).StartsWith(name + "-"))
                {
                    path = file;
                    break;
                }
            }
            
            if (path == null) return;

            Selection.activeObject = asset;
            string defaultName = Path.GetFileNameWithoutExtension(path).Substring(name.Length + 1);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, defaultName);
        }

        private static void ProcessClick(ProjectItem item)
        {
            Event e = Event.current;
            if (e.button == 0)
            {
                OnCreateScript(new object[] { item.asset, "C# Script" });
            }
            else if (e.button == 1)
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("C# Script"), false, OnCreateScript, new object[] { item.asset, "C# Script" });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("C# Class"), false, OnCreateScript, new object[] { item.asset, "C# Class" });
                menu.AddItem(new GUIContent("C# Interface"), false, OnCreateScript, new object[] { item.asset, "C# Interface" });
                menu.AddItem(new GUIContent("C# Abstract Class"), false, OnCreateScript, new object[] { item.asset, "C# Abstract Class" });
                menu.AddItem(new GUIContent("C# Struct"), false, OnCreateScript, new object[] { item.asset, "C# Struct" });
                menu.AddItem(new GUIContent("C# Enum"), false, OnCreateScript, new object[] { item.asset, "C# Enum" });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("C# Custom Editor Script"), false, OnCreateScript, new object[] { item.asset, "C# Custom Editor" });
                menu.AddItem(new GUIContent("C# Editor Window Script"), false, OnCreateScript, new object[] { item.asset, "C# Editor Window" });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("C# Test Script"), false, OnCreateScript, new object[] { item.asset, "C# Test Script" });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Assembly Definition"), false, OnCreateScript, new object[] { item.asset, "Assembly Definition" });
                menu.AddItem(new GUIContent("Assembly Definition Reference"), false, OnCreateScript, new object[] { item.asset, "Assembly Definition Reference" });

                menu.ShowAsContext();
            }
        }
    }
}