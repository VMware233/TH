using System;
using Sirenix.OdinInspector;
using FishNet.Serializing;
using TH.Damage;
using VMFramework.Property;

namespace TH.Entities
{
    public class HealthOwnerEntity : Entity, IDamageable, IDefenseOwner, IHealthOwner
    {
        protected HealthOwnerEntityConfig healthOwnerEntityConfig => (HealthOwnerEntityConfig)gamePrefab;

        [LabelText("生命值")]
        [ShowInInspector]
        public BaseIntProperty health;

        [LabelText("最大生命值")]
        [ShowInInspector]
        public BaseBoostIntProperty maxHealth;

        [LabelText("防御力")]
        [ShowInInspector]
        public BaseBoostIntProperty defense;
        
        [LabelText("百分比防御力")]
        [ShowInInspector]
        public BaseBoostFloatProperty defensePercent;

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            maxHealth = new(healthOwnerEntityConfig.defaultMaxHealth);
            health = new(maxHealth);

            defense = new(healthOwnerEntityConfig.defaultDefense);
            defensePercent = new(healthOwnerEntityConfig.defaultDefensePercent);
        }

        protected override void OnInit()
        {
            base.OnInit();

            health.OnValueChanged += OnHealthChanged;
        }

        #endregion

        #region Health Changed

        private void OnHealthChanged(int previous, int current)
        {
            if (isClient)
            {
                DamageUIUtility.PopupHealthChange(current - previous, controller.graphicTransform.position);
            }

            if (current <= 0)
            {
                if (healthOwnerEntityConfig.autoDestroyWhenHealthZero)
                {
                    EntityManager.DestroyEntity(this);
                }
            }
        }

        #endregion

        #region Damage

        public event Action<DamageResult> OnDamageTaken;

        protected virtual void ProcessDamageResult(DamageResult result)
        {
            health.value += result.healthChange;
        }

        void IDamageable.ProcessDamageResult(DamageResult result)
        {
            ProcessDamageResult(result);
            
            OnDamageTaken?.Invoke(result);
        }

        #endregion

        #region Defense & Health Owner

        int IDefenseOwner.defense => defense;

        int IDefenseOwner.defenseBase
        {
            get => defense.baseValue;
            set => defense.baseValue = value;
        }

        float IDefenseOwner.defenseBoost
        {
            get => defense.boostValue;
            set => defense.boostValue = value;
        }

        float IDefenseOwner.defensePercent => defensePercent;

        float IDefenseOwner.defensePercentBase
        {
            get => defensePercent.baseValue;
            set => defensePercent.baseValue = value;
        }

        float IDefenseOwner.defensePercentBoost
        {
            get => defensePercent.boostValue;
            set => defensePercent.boostValue = value;
        }

        int IHealthOwner.health
        {
            get => health;
            set => health.value = value;
        }

        int IHealthOwner.maxHealth => maxHealth;

        int IHealthOwner.maxHealthBase
        {
            get => maxHealth.baseValue;
            set => maxHealth.baseValue = value;
        }

        float IHealthOwner.maxHealthBoost
        {
            get => maxHealth.boostValue;
            set => maxHealth.boostValue = value;
        }

        #endregion

        #region Net Serialization

        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);

            writer.WriteInt32(health);
            writer.WriteBaseBoostIntProperty(maxHealth);

            writer.WriteBaseBoostIntProperty(defense);
            writer.WriteBaseBoostFloatProperty(defensePercent);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            health.value = reader.ReadInt32();
            maxHealth = reader.ReadBaseBoostIntProperty();

            defense = reader.ReadBaseBoostIntProperty();
            defensePercent = reader.ReadBaseBoostFloatProperty();
        }

        #endregion
    }
}
