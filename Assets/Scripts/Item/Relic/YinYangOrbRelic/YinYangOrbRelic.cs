using TH.Damage;
using VMFramework.Containers;

namespace TH.Items
{
    /// <summary>
    /// 阴阳玉遗物
    /// 提升攻击力效果
    /// </summary>
    public class YinYangOrbRelic : Relic, IContainerItem
    {
        protected YinYangOrbRelicPreset yinYangOrbRelicPreset => (YinYangOrbRelicPreset)gamePrefab;

        private IAttackOwner attackOwner;

        private float restoreTimer = 0;

        protected override void OnCreate()
        {
            base.OnCreate();

            attackOwner = null;
            count.OnValueChanged += OnCountChanged;
        }

        // 增加或减少数量时，更新遗物所属者的攻击力
        private void OnCountChanged(int previous, int current)
        {
            if (attackOwner == null)
            {
                return;
            }

            attackOwner.attackBase += (current - previous) * yinYangOrbRelicPreset.attackBasePower;
        }

        void IContainerItem.OnAddToContainer(Container container)
        {
            if (isServer == false)
            {
                return;
            }

            // 绑定更新委托
            if (container.owner is IAttackOwner newOwner)
            {
                attackOwner = newOwner;
                attackOwner.attackBase += count * yinYangOrbRelicPreset.attackBasePower;
            }
        }

        void IContainerItem.OnRemoveFromContainer(Container container)
        {
            if (isServer == false)
            {
                return;
            }

            // 移除更新委托
            if (attackOwner != null)
            {
                attackOwner.attackBase -= count * yinYangOrbRelicPreset.attackBasePower;
                attackOwner = null;
            }
        }
    }
}