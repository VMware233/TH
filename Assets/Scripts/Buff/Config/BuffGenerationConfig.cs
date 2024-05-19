using Sirenix.OdinInspector;
using VMFramework.Configuration;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Buffs.Config
{
    public class BuffGenerationConfig : BaseConfig
    {
        [LabelText("Buff")]
        [GamePrefabID(typeof(BuffPreset))]
        [IsNotNullOrEmpty]
        public string buffID;

        [LabelText("持续时间")]
        [MinValue(0)]
        public float duration;

        [LabelText("层级")]
        [PropertyRange(1, nameof(GetMaxLevel))]
        public int level = 1;

        private int GetMaxLevel()
        {
            if (buffID.IsNullOrEmpty())
            {
                return 1;
            }

            var buffPreset = GamePrefabManager.GetGamePrefab<BuffPreset>(buffID);

            if (buffPreset == null)
            {
                return 1;
            }

            return buffPreset.maxLevel;
        }

        public IBuff GenerateBuff()
        {
            var newBuff = IGameItem.Create<IBuff>(buffID);

            newBuff.duration = duration;
            newBuff.level = level;

            return newBuff;
        }
    }
}