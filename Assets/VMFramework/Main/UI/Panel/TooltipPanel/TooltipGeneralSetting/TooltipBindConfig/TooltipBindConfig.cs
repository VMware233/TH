using Sirenix.OdinInspector;
using VMFramework.Configuration;
using VMFramework.Core;
using VMFramework.OdinExtensions;

namespace VMFramework.UI
{
    public class TooltipBindConfig : BaseConfig, IIDOwner
    {
        [HideLabel]
        [GameTypeID]
        [IsNotNullOrEmpty]
        public string gameTypeID;

        [UIPresetID(typeof(ITooltipPreset))]
        [IsNotNullOrEmpty]
        public string tooltipID;

        string IIDOwner<string>.id => gameTypeID;
    }
}