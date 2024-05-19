using VMFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Entities
{
    public sealed partial class ProjectileGeneralSetting : GeneralSettingBase
    {
        [field: LabelText("投射物Layer")]
        [field: Layer]
        [field: SerializeField]
        public int projectileLayer { get; private set; }
        
        #region Check Settings

        public override void CheckSettings()
        {
            base.CheckSettings();

            projectileLayer.AssertIsAboveOrEqual(0, nameof(projectileLayer));
        }

        #endregion
    }
}
