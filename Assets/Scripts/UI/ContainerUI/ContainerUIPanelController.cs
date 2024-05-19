using VMFramework.Core;
using VMFramework.UI;

namespace TH.UI
{
    public class ContainerUIPanelController : ContainerUIBaseController
    {
        protected ContainerUIPanelPreset containerUIPanelPreset =>
            preset as ContainerUIPanelPreset;

        protected override void OnPreInit(UIPanelPreset preset)
        {
            base.OnPreInit(preset);

            containerUIPanelPreset.AssertIsNotNull(nameof(containerUIPanelPreset));

            containerUICore.OnSetSlotVisualElement += SetSlotVisualElement;
        }

        #region Set Slot Visual Element

        private void SetSlotVisualElement(SlotVisualElement slotVisualElement, int slotIndex)
        {

        }

        #endregion
    }
}
