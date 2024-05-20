using System;
using VMFramework.GameLogicArchitecture;
using System.Collections.Generic;
using VMFramework.UI;
using FishNet.Connection;
using Sirenix.OdinInspector;
using TH.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using VMFramework.Network;
using VMFramework.Property;

namespace TH.Spells
{
    public abstract class Spell : VisualGameItem, ISlotProvider, IUUIDOwner
    {
        protected SpellPreset spellPreset => (SpellPreset)gamePrefab;

        [ShowInInspector]
        public string uuid { get; private set; }

        [ShowInInspector]
        public BaseFloatProperty cooldown;

        [ShowInInspector]
        public ISpellOwner owner { get; private set; }

        #region UUID Owner

        string IUUIDOwner.uuid
        {
            get => uuid;
            set => uuid = value;
        }

        bool IUUIDOwner.isDirty
        {
            get => true;
            set { }
        }

        public event Action<IUUIDOwner, bool, NetworkConnection> OnObservedEvent;
        public event Action<IUUIDOwner, NetworkConnection> OnUnobservedEvent;

        void IUUIDOwner.OnObserved(bool isDirty, NetworkConnection connection)
        {
            OnObservedEvent?.Invoke(this, isDirty, connection);
        }

        void IUUIDOwner.OnUnobserved(NetworkConnection connection)
        {
            OnUnobservedEvent?.Invoke(this, connection);
        }

        #endregion

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            cooldown = new(0);
        }

        #endregion

        #region Set Owner

        public void SetOwner(ISpellOwner owner)
        {
            if (this.owner != null)
            {
                throw new InvalidOperationException("Spell已经拥有了Owner");
            }

            this.owner = owner;
        }

        #endregion

        #region Update

        public void Update()
        {
            if (cooldown >= 0)
            {
                cooldown.value -= Time.deltaTime;
            }
        }

        #endregion

        #region Cast & Abort

        public struct SpellCastInfo
        {
            public ISpellCaster caster { get; init; }
            public SpellTargetType targetType { get; init; }
            public IEnumerable<Entity> entities { get; init; }
            public Vector2 mainDirection { get; init; }
            public Vector2 mainPosition { get; init; }
        }

        public struct SpellAbortInfo
        {

        }

        public abstract void Cast(SpellCastInfo spellCastInfo);

        public abstract void Abort(SpellAbortInfo spellAbortInfo);

        #endregion

        #region Slot Provider

        StyleBackground ISlotProvider.GetIconImage()
        {
            return new StyleBackground(spellPreset.icon);
        }

        string ISlotProvider.GetDescriptionText()
        {
            return string.Empty;
        }

        void ISlotProvider.HandleMouseEnterEvent(UIPanelController source)
        {
            if (isDebugging)
            {
                Debug.LogWarning($"{this}被鼠标进入");
            }

            TracingTooltipManager.Open(this, source);
        }

        void ISlotProvider.HandleMouseLeaveEvent(UIPanelController source)
        {
            if (isDebugging)
            {
                Debug.LogWarning($"{this}被鼠标退出");
            }

            TracingTooltipManager.Close(this);
        }

        #endregion
    }
}