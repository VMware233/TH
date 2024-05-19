﻿#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Localization;

namespace VMFramework.OdinExtensions
{
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class ColorContextMenuDrawer : OdinValueDrawer<Color>, IDefinesGenericMenuItems
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            genericMenu.AddSeparator("");

            genericMenu.AddDisabledItem(
                new GUIContent(ValueEntry.SmartValue.ToLocalizedString(ColorStringFormat.Name)));
        }
    }
}
#endif