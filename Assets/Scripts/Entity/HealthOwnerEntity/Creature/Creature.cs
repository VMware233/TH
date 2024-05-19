using Sirenix.OdinInspector;
using FishNet.Serializing;
using TH.Damage;
using VMFramework.Property;

namespace TH.Entities
{
    public class Creature : HealthOwnerEntity, IDamageSource, IAttackOwner
    {
        protected CreatureConfig creatureConfig => gamePrefab as CreatureConfig;

        [ShowInInspector]
        public BaseBoostFloatProperty movementSpeed;

        [ShowInInspector]
        public BaseBoostIntProperty attack;
        
        [ShowInInspector]
        public BaseBoostFloatProperty attackMultiplier;

        [ShowInInspector]
        public BaseBoostFloatProperty criticalRate;
        
        [ShowInInspector]
        public BaseBoostFloatProperty criticalDamageMultiplier;

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            movementSpeed = new(creatureConfig.defaultMovementSpeed);
            
            attack = new(creatureConfig.defaultAttack);
            attackMultiplier = new(creatureConfig.defaultAttackMultiplier);
            
            criticalRate = new(creatureConfig.defaultCriticalRate);
            criticalDamageMultiplier = new(creatureConfig.defaultCriticalDamageMultiplier);
        }

        #endregion

        #region Attack Owner

        int IAttackOwner.attack => attack.value;

        int IAttackOwner.attackBase
        {
            get => attack.baseValue;
            set => attack.baseValue = value;
        }

        float IAttackOwner.attackBoost
        {
            get => attack.boostValue;
            set => attack.boostValue = value;
        }

        float IAttackOwner.attackMultiplier => attackMultiplier.value;

        float IAttackOwner.attackMultiplierBase
        {
            get => attackMultiplier.baseValue;
            set => attackMultiplier.baseValue = value;
        }

        float IAttackOwner.attackMultiplierBoost
        {
            get => attackMultiplier.boostValue;
            set => attackMultiplier.boostValue = value;
        }

        float IAttackOwner.criticalRate => criticalRate.value;
        
        float IAttackOwner.criticalRateBase
        {
            get => criticalRate.baseValue;
            set => criticalRate.baseValue = value;
        }

        float IAttackOwner.criticalRateBoost
        {
            get => criticalRate.boostValue;
            set => criticalRate.boostValue = value;
        }

        float IAttackOwner.criticalDamageMultiplier => criticalDamageMultiplier.value;

        float IAttackOwner.criticalDamageMultiplierBase
        {
            get => criticalDamageMultiplier.baseValue;
            set => criticalDamageMultiplier.baseValue = value;
        }

        float IAttackOwner.criticalDamageMultiplierBoost
        {
            get => criticalDamageMultiplier.boostValue;
            set => criticalDamageMultiplier.boostValue = value;
        }

        #endregion

        #region Net Serialization

        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);

            writer.WriteBaseBoostFloatProperty(movementSpeed);

            writer.WriteBaseBoostIntProperty(attack);
            writer.WriteBaseBoostFloatProperty(attackMultiplier);
            
            writer.WriteBaseBoostFloatProperty(criticalRate);
            writer.WriteBaseBoostFloatProperty(criticalDamageMultiplier);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            movementSpeed = reader.ReadBaseBoostFloatProperty();

            attack = reader.ReadBaseBoostIntProperty();
            attackMultiplier = reader.ReadBaseBoostFloatProperty();
            
            criticalRate = reader.ReadBaseBoostFloatProperty();
            criticalDamageMultiplier = reader.ReadBaseBoostFloatProperty();
        }

        #endregion

        #region Damage

        public virtual void ProduceDamagePacket(IDamageable target, out DamagePacket packet)
        {
            packet = new DamagePacket(this)
            {
                physicalDamage = attack.value,
                magicalDamage = attack.value,
                damageMultiplier = attackMultiplier.value,
                criticalRate = criticalRate.value,
                criticalDamageMultiplier = criticalDamageMultiplier.value
            };
        }

        #endregion
    }
}
