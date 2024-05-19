using System;
using VMFramework.UI;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.UI
{
    [GamePrefabTypeAutoRegister(ID)]
    public class MainHUDUIPanelPreset : UIToolkitPanelPreset
    {
        public const string ID = "main_hud_ui";

        #region Categories

        protected const string IN_GAME_MAIN_MENU_CATEGORY = "游戏内主界面";

        protected const string SPELL_SLOT_CATEGORY =
            TAB_GROUP_NAME + "/" + IN_GAME_MAIN_MENU_CATEGORY + "/法术槽";

        #endregion

        public override Type controllerType => typeof(MainHUDUIPanelController);

        [LabelText("金币数量Label名称"), TabGroup(TAB_GROUP_NAME, IN_GAME_MAIN_MENU_CATEGORY)]
        [VisualElementName(typeof(Label))]
        [IsNotNullOrEmpty]
        public string iconCountLabelName;

        [BoxGroup(SPELL_SLOT_CATEGORY)]
        [VisualElementName(typeof(SlotVisualElement))]
        public string spellOneSlotName;

        [BoxGroup(SPELL_SLOT_CATEGORY)]
        [VisualElementName(typeof(SlotVisualElement))]
        public string spellTwoSlotName;

        [BoxGroup(SPELL_SLOT_CATEGORY)]
        [VisualElementName(typeof(SlotVisualElement))]
        public string spellThreeSlotName;

        [BoxGroup(SPELL_SLOT_CATEGORY)]
        [VisualElementName(typeof(SlotVisualElement))]
        public string spellFourSlotName;
    }
}
