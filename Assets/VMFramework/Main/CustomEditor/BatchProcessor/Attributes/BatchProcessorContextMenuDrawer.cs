﻿#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace VMFramework.Editor
{
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public sealed class BatchProcessorContextMenuDrawer<T> : OdinValueDrawer<T>,
        IDefinesGenericMenuItems
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property,
            GenericMenu genericMenu)
        {
            var value = property.ValueEntry.WeakSmartValue;

            if (value == null)
            {
                return;
            }

            if (value is ICollection collection)
            {
                if (collection.Count == 0)
                {
                    return;
                }

                genericMenu.AddSeparator("");

                genericMenu.AddItem(new GUIContent("批处理"), false, () =>
                {
                    BatchProcessorWindow.OpenWindow(collection.Cast<object>());
                });

                genericMenu.AddItem(new GUIContent("添加到批处理"), false, () =>
                {
                    BatchProcessorWindow.AddToWindow(collection.Cast<object>());
                });
            }
            else
            {
                genericMenu.AddItem(new GUIContent("添加到批处理"), false, () =>
                {
                    BatchProcessorWindow.AddToWindow(new[] { value });
                });
            }
        }
    }
}

#endif