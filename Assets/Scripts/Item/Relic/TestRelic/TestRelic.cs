using TH.Damage;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Containers;
using VMFramework.Procedure;

namespace TH.Items
{
    /// <summary>
    /// 测试用的遗物
    /// 具有生命值回复和增加最大生命值的功能
    /// </summary>
    public class TestRelic : Relic, IContainerItem
    {
        protected TestRelicPreset testRelicPreset => (TestRelicPreset)gamePrefab;

        private IHealthOwner healthOwner;

        private float restoreTimer = 0;

        protected override void OnCreate()
        {
            base.OnCreate();

            healthOwner = null;
            restoreTimer = 0;
            
            count.OnValueChanged += OnCountChanged;
        }

        // 增加或减少数量时，更新遗物所属者的最大生命值
        private void OnCountChanged(int previous, int current)
        {
            if (healthOwner == null)
            {
                return;
            }

            // Debug.LogError($"{previous} - {current}");
            healthOwner.maxHealthBase += (current - previous) * testRelicPreset.maxHealthBonus;
        }

        private void Update()
        {
            restoreTimer += Time.deltaTime;

            if (restoreTimer >= testRelicPreset.healthRestoreInterval)
            {
                restoreTimer = 0;

                if (healthOwner.health >= healthOwner.maxHealth)
                {
                    return;
                }
                
                // 回复 遗物数量*回复量 的生命值
                int restoreAmount = count * testRelicPreset.healthRestore.GetValue();
                
                // 回复的血量不能超过此遗物本身限制的最大回复量
                restoreAmount = restoreAmount.ClampMax(testRelicPreset.maxHealthRestore);
                
                // 不能超过此遗物所属者的最大生命值
                restoreAmount = restoreAmount.ClampMax(healthOwner.maxHealth - healthOwner.health);
                healthOwner.health += restoreAmount;
            }
        }

        void IContainerItem.OnAddToContainer(Container container)
        {
            if (isServer == false)
            {
                return;
            }

            // 如果此遗物之前属于其他人，则先移除原属主的生命值更新委托
            if (healthOwner != null)
            {
                UpdateDelegateManager.RemoveUpdateDelegate(UpdateType.Update, Update);
            }
            
            // 绑定生命值更新委托
            if (container.owner is IHealthOwner newOwner)
            {
                healthOwner = newOwner;
                
                UpdateDelegateManager.AddUpdateDelegate(UpdateType.Update, Update);

                // Debug.LogError($"{count}");
                healthOwner.maxHealthBase += count * testRelicPreset.maxHealthBonus;
            }
        }

        void IContainerItem.OnRemoveFromContainer(Container container)
        {
            if (isServer == false)
            {
                return;
            }

            // 移除生命值更新委托
            if (healthOwner != null)
            {
                UpdateDelegateManager.RemoveUpdateDelegate(UpdateType.Update, Update);
                // Debug.LogError($"{count}");
                healthOwner.maxHealthBase -= count * testRelicPreset.maxHealthBonus;
                healthOwner = null;
            }
        }
    }
}