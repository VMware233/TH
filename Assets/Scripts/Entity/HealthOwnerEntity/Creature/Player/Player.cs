using System.Collections.Generic;
using VMFramework.Core;
using FishNet.Serializing;
using Sirenix.OdinInspector;
using TH.Damage;
using TH.GameCore;
using TH.Containers;
using TH.Spells;
using UnityEngine;
using VMFramework.Containers;
using VMFramework.GameLogicArchitecture;
using VMFramework.Property;

namespace TH.Entities
{
    public class Player : Creature, IContainerOwner, ISpellOwner, ISpellCaster
    {
        public new PlayerController controller { get; private set; }

        protected PlayerConfig playerConfig => (PlayerConfig)gamePrefab;

        [ShowInInspector]
        public RelicInventory relicInventory { get; private set; }

        [ShowInInspector]
        public BaseIntProperty maxJumpTimes;

        [ShowInInspector]
        public BaseBoostFloatProperty jumpForce;

        [ShowInInspector]
        public BaseBoostFloatProperty flySpeed;

        [ShowInInspector]
        public BaseIntProperty luck;

        [ShowInInspector]
        public BaseIntProperty coinCount;

        #region Spell

        [ShowInInspector]
        public Spell spellOne;

        [ShowInInspector]
        public Spell spellTwo;

        [ShowInInspector]
        public Spell spellThree;

        [ShowInInspector]
        public Spell spellFour;

        #endregion

        #region Level

        [ShowInInspector]
        public BaseIntProperty experience;

        public int level => experience.value / GameSetting.playerGeneralSetting.experiencePerLevel;

        [ShowInInspector]
        public BaseBoostFloatProperty experienceBoost;

        #endregion

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            maxJumpTimes = new(playerConfig.defaultMaxJumpTimes);
            jumpForce = new(playerConfig.defaultJumpForce);

            flySpeed = new(playerConfig.defaultFlySpeed);

            luck = new(playerConfig.defaultLuck);

            coinCount = new(playerConfig.defaultCoinCount);

            spellOne = IGameItem.Create<Spell>(playerConfig.spellOneID);
            spellOne.SetOwner(this);

            spellTwo = IGameItem.Create<Spell>(playerConfig.spellTwoID);
            spellTwo.SetOwner(this);

            spellThree = IGameItem.Create<Spell>(playerConfig.spellThreeID);
            spellThree.SetOwner(this);

            spellFour = IGameItem.Create<Spell>(playerConfig.spellFourID);
            spellFour.SetOwner(this);

            relicInventory = IGameItem.Create<RelicInventory>(playerConfig.relicInventoryID);
            relicInventory.SetOwner(this);

            if (isServer)
            {
                foreach (var initialRelic in playerConfig.initialRelics)
                {
                    relicInventory.AddItem(initialRelic.GenerateItem());
                }
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            controller = base.controller as PlayerController;

            controller.AssertIsNotNull(nameof(controller));
        }

        #endregion

        #region Net Serialization

        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);

            writer.WriteString(relicInventory.uuid);

            writer.WriteInt32(maxJumpTimes);
            writer.WriteBaseBoostFloatProperty(jumpForce);

            writer.WriteBaseBoostFloatProperty(flySpeed);

            writer.WriteInt32(luck);

            writer.WriteInt32(coinCount);

            writer.WriteInt32(experience);
            writer.WriteBaseBoostFloatProperty(experienceBoost);

            writer.WriteString(spellOne.uuid);
            writer.WriteString(spellTwo.uuid);
            writer.WriteString(spellThree.uuid);
            writer.WriteString(spellFour.uuid);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            relicInventory.SetUUID(reader.ReadString());

            maxJumpTimes.value = reader.ReadInt32();
            jumpForce = reader.ReadBaseBoostFloatProperty();

            flySpeed = reader.ReadBaseBoostFloatProperty();

            luck.value = reader.ReadInt32();

            coinCount.value = reader.ReadInt32();

            experience.value = reader.ReadInt32();
            experienceBoost = reader.ReadBaseBoostFloatProperty();

            spellOne.SetUUID(reader.ReadString());
            spellTwo.SetUUID(reader.ReadString());
            spellThree.SetUUID(reader.ReadString());
            spellFour.SetUUID(reader.ReadString());
        }

        #endregion

        #region Player Action State

        public IEnumerable<IInitialPlayerActionStateConfig> GetInitialPlayerActionStateConfigs()
        {
            return playerConfig.initialPlayerActionStateConfigs;
        }

        #endregion

        #region Container Owner

        public IEnumerable<IContainer> GetContainers()
        {
            yield return relicInventory;
        }

        #endregion

        #region Spell Owner & Caster

        Vector2 ISpellOwner.castPosition => controller.castPositionTransform.position.XY();

        void ISpellCaster.ProduceDamagePacket(IDamageable target, out DamagePacket packet)
        {
            ProduceDamagePacket(target, out packet);
        }

        Vector2 ISpellCaster.castPosition => controller.castPositionTransform.position.XY();

        public Vector2 casterPosition => controller.transform.position.XY();

        #endregion
    }
}
