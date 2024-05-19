﻿#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using VMFramework.Configuration;
using VMFramework.Core;

namespace VMFramework.OdinExtensions
{
    public class ColorChooserConfigContextMenuDrawer<T> : OdinValueDrawer<T>, IDefinesGenericMenuItems
        where T : IChooserConfig<Color>
    {
        private const float COLOR_ALPHA_THRESHOLD = 0.3f;
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            var chooser = ValueEntry.SmartValue;

            if (chooser == null)
            {
                return;
            }

            if (chooser.GetAvailableValues().Any(color => color.a < COLOR_ALPHA_THRESHOLD))
            {
                genericMenu.AddItem(new GUIContent("设置Alpha为1"), false, SetAlphaTo1);
            }

            if (chooser.GetAvailableValues().Any(color => color.a < 1))
            {
                genericMenu.AddItem(new GUIContent("设置所有Alpha为1"), false, SetAllAlphaTo1);
            }

            void SetAlphaTo1()
            {
                chooser.SetAvailableValues(color =>
                {
                    if (color.a < COLOR_ALPHA_THRESHOLD)
                    {
                        return color.ReplaceAlpha(1);
                    }
                    
                    return color;
                });
            }

            void SetAllAlphaTo1()
            {
                chooser.SetAvailableValues(color => color.ReplaceAlpha(1));
            }
        }
    }
}
#endif