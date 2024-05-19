using VMFramework.UI;
using VMFramework.Core;
using Cysharp.Threading.Tasks;
using TH.Entities;
using TH.GameEvents;
using TH.Spells;
using UnityEngine.UIElements;
using VMFramework.GameEvents;

namespace TH.UI
{
    public class MainHUDUIPanelController : UIToolkitPanelController
    {
        protected MainHUDUIPanelPreset mainHUDUIPanelPreset { get; private set; }

        protected Label iconCountLabel { get; private set; }

        protected override void OnPreInit(UIPanelPreset preset)
        {
            base.OnPreInit(preset);

            mainHUDUIPanelPreset = preset as MainHUDUIPanelPreset;

            mainHUDUIPanelPreset.AssertIsNotNull(nameof(mainHUDUIPanelPreset));
        }

        protected override async void OnOpenInstantly(IUIPanelController source)
        {
            base.OnOpenInstantly(source);

            await UniTask.WaitUntil(() => PlayerManager.isThisPlayerInitialized);

            OnOpenSpellSlots();

            iconCountLabel = rootVisualElement.Q<Label>(mainHUDUIPanelPreset.iconCountLabelName);

            iconCountLabel.AssertIsNotNull(nameof(iconCountLabel));

            OnIconCountChanged(0, PlayerManager.GetThisPlayer().coinCount);

            PlayerManager.GetThisPlayer().coinCount.OnValueChanged += OnIconCountChanged;
        }

        private void OnIconCountChanged(int previous, int current)
        {
            iconCountLabel.text = $"x{current}";
        }

        #region Spell Slots

        protected SlotVisualElement spellOneSlot { get; private set; }
        protected SlotVisualElement spellTwoSlot { get; private set; }
        protected SlotVisualElement spellThreeSlot { get; private set; }
        protected SlotVisualElement spellFourSlot { get; private set; }

        private void OnOpenSpellSlots()
        {
            spellOneSlot = rootVisualElement.Q<SlotVisualElement>(mainHUDUIPanelPreset.spellOneSlotName);

            spellTwoSlot = rootVisualElement.Q<SlotVisualElement>(mainHUDUIPanelPreset.spellTwoSlotName);

            spellThreeSlot = rootVisualElement.Q<SlotVisualElement>(mainHUDUIPanelPreset.spellThreeSlotName);

            spellFourSlot = rootVisualElement.Q<SlotVisualElement>(mainHUDUIPanelPreset.spellFourSlotName);

            spellOneSlot.SetSlotProvider(PlayerManager.GetThisPlayer()?.spellOne);
            spellOneSlot.SetSource(this);

            spellTwoSlot.SetSlotProvider(PlayerManager.GetThisPlayer()?.spellTwo);
            spellTwoSlot.SetSource(this);

            spellThreeSlot.SetSlotProvider(PlayerManager.GetThisPlayer()?.spellThree);
            spellThreeSlot.SetSource(this);

            spellFourSlot.SetSlotProvider(PlayerManager.GetThisPlayer()?.spellFour);
            spellFourSlot.SetSource(this);


            GameEventManager.AddCallback(PlayerGameEvents.SPELL_ONE_CAST, CastSpellOne);
            GameEventManager.AddCallback(PlayerGameEvents.SPELL_TWO_CAST, CastSpellTwo);
            GameEventManager.AddCallback(PlayerGameEvents.SPELL_THREE_CAST, CastSpellThree);
            GameEventManager.AddCallback(PlayerGameEvents.SPELL_FOUR_CAST, CastSpellFour);
        }

        private void CastSpellOne(BoolInputGameEvent boolEvent)
        {
            SpellManager.Cast(PlayerManager.GetThisPlayer().spellOne, new Spell.SpellCastInfo()
            {
                caster = PlayerManager.GetThisPlayer(),
                mainDirection = PlayerSelection.GetThisPlayerToMouseDirection(),
                targetType = SpellTargetType.Direction
            });
        }

        private void CastSpellTwo(BoolInputGameEvent boolEvent)
        {
            SpellManager.Cast(PlayerManager.GetThisPlayer().spellTwo, new Spell.SpellCastInfo()
            {
                caster = PlayerManager.GetThisPlayer(),
                mainDirection = PlayerSelection.GetThisPlayerToMouseDirection(),
                targetType = SpellTargetType.Direction
            });
        }
        
        private void CastSpellThree(BoolInputGameEvent boolEvent)
        {
            SpellManager.Cast(PlayerManager.GetThisPlayer().spellThree, new Spell.SpellCastInfo()
            {
                caster = PlayerManager.GetThisPlayer(),
                mainDirection = PlayerSelection.GetThisPlayerToMouseDirection(),
                targetType = SpellTargetType.Direction
            });
        }
        
        private void CastSpellFour(BoolInputGameEvent boolEvent)
        {
            SpellManager.Cast(PlayerManager.GetThisPlayer().spellFour, new Spell.SpellCastInfo()
            {
                caster = PlayerManager.GetThisPlayer(),
                mainDirection = PlayerSelection.GetThisPlayerToMouseDirection(),
                targetType = SpellTargetType.Direction
            });
        }

        #endregion
    }
}