﻿using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
namespace VMFramework.GameEvents
{
    public partial class FloatInputGameEventConfig
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            positiveActionGroups ??= new();
            negativeActionGroups ??= new();
        }

        [Button("快速添加AD左右输入动作组", ButtonSizes.Medium), TabGroup(TAB_GROUP_NAME, INPUT_MAPPING_CATEGORY)]
        private void AddADActionGroupGUI()
        {
            isFromAxis = false;

            positiveActionGroups.Clear();
            positiveActionGroups.Add(new(KeyCode.D, KeyBoardTriggerType.KeyStay));
            positiveActionGroups.Add(new(KeyCode.RightArrow, KeyBoardTriggerType.KeyStay));

            negativeActionGroups.Clear();
            negativeActionGroups.Add(new(KeyCode.A, KeyBoardTriggerType.KeyStay));
            negativeActionGroups.Add(new(KeyCode.LeftArrow, KeyBoardTriggerType.KeyStay));
        }
    }
}
#endif