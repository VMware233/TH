﻿#if UNITY_EDITOR
using VMFramework.GameLogicArchitecture;

namespace VMFramework.Editor.GameEditor
{
    internal sealed partial class GameEditorGeneralSetting : GeneralSetting
    {
        public int autoStackMenuTreeNodesMaxCount = 8;
    }
}

#endif