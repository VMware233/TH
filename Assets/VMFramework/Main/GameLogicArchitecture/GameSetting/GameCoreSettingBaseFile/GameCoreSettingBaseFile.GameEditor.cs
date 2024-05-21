﻿#if UNITY_EDITOR
using System.Collections.Generic;
using VMFramework.Core.Editor;
using VMFramework.Editor;
using VMFramework.Editor.GameEditor;

namespace VMFramework.GameLogicArchitecture
{
    public partial class GameCoreSettingBaseFile : IGameEditorToolBarProvider, IGameEditorContextMenuProvider
    {
        IEnumerable<IGameEditorToolBarProvider.ToolbarButtonConfig> IGameEditorToolBarProvider.
            GetToolbarButtons()
        {
            yield return new(EditorNames.openScriptButtonName, this.OpenScriptOfObject);
            yield return new(EditorNames.saveButtonName, this.EnforceSave);
        }
    }
}
#endif