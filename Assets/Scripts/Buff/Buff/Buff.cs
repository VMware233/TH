using System;
using FishNet.Connection;
using TH.Entities;
using UnityEngine;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.Network;
using VMFramework.Property;

namespace TH.Buffs
{
    public partial class Buff : VisualGameItem, IBuff
    {
        protected BuffPreset buffPreset => (BuffPreset)gamePrefab;
        
        public Sprite icon => buffPreset.icon;

        public BaseFloatProperty duration;

        public BaseIntProperty level;

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            duration = new(0);
            level = new(1);
        }

        #endregion

        #region IBuff

        float IBuff.duration
        {
            get => duration.value;
            set => duration.value = value;
        }

        int IBuff.level
        {
            get => level.value;
            set => level.value = value;
        }

        void IBuff.OnAddToEntity(Entity entity)
        {
            OnAddToEntity(entity);
        }

        void IBuff.OnRemoveFromEntity(Entity entity)
        {
            OnRemoveFromEntity(entity);
        }

        #endregion

        #region Event

        protected virtual void OnAddToEntity(Entity entity)
        {
            
        }
        
        protected virtual void OnRemoveFromEntity(Entity entity)
        {
            
        }

        #endregion

        #region UUID Owner
        
        public string uuid { get; private set; }

        string IUUIDOwner.uuid
        {
            get => uuid;
            set => uuid = value;
        }

        bool IUUIDOwner.isDirty
        {
            get => false;
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
        
        #region Owner

        public Entity owner { get; private set; }

        public void SetOwner(Entity owner)
        {
            if (owner == null)
            {
                this.owner = null;
            }
            else if (this.owner == null)
            {
                this.owner = owner;
            }
            else
            {
                Debug.LogWarning($"试图修改已经设置的{nameof(owner)}");
            }
        }

        #endregion
        
        public virtual void Update()
        {
            if (duration >= 0)
            {
                duration.value -= Time.deltaTime;
            }
        }
        
        public virtual bool AllowBackgroundRun()
        {
            if (buffPreset.isLevelStackable)
            {
                return false;
            }

            if (buffPreset.isIndependentBuff == false)
            {
                return false;
            }

            return true;
        }

        public virtual bool ShouldOtherBuffDropOffBeforeRunningBackground(IBuff otherBuff)
        {
            return otherBuff.level <= level && otherBuff.duration < duration;
        }

        public virtual int CompareStrength(IBuff otherBuff)
        {
            return level.value.CompareTo(otherBuff.level);
        }

        public virtual bool CanStack(IBuff otherBuff)
        {
            if (id != otherBuff.id)
            {
                return false;
            }

            return true;
        }

        public virtual void Stack(IBuff otherBuff)
        {
            if (buffPreset.isLevelStackable)
            {
                if (buffPreset.levelStackType == LevelStackType.SimpleStack)
                {
                    level.value += otherBuff.level;
                }
                else if (buffPreset.levelStackType == LevelStackType.StackWhileHavingLongerDuration)
                {
                    if (otherBuff.duration >= duration)
                    {
                        level.value += otherBuff.level;
                    }
                }
                else if (buffPreset.levelStackType == LevelStackType.StackToHigherLevelWhileHavingLongerDuration)
                {
                    if (otherBuff.duration >= duration)
                    {
                        level.value = otherBuff.level.Max(level);
                    }
                }
            }

            if (buffPreset.durationStackType == DurationStackType.SimpleStack)
            {
                duration.value += otherBuff.duration;
            }
            else if (buffPreset.durationStackType == DurationStackType.StackToShorter)
            {
                duration.value = otherBuff.duration.Min(duration);
            }
            else if (buffPreset.durationStackType == DurationStackType.StackToLonger)
            {
                duration.value = otherBuff.duration.Max(duration);
            }
            else if (buffPreset.durationStackType == DurationStackType.StackWhileHavingHigherLevel)
            {
                if (otherBuff.level >= level)
                {
                    duration.value += otherBuff.duration;
                }
            }
            else if (buffPreset.durationStackType == DurationStackType.StackToLongerWhileHavingHigherLevel)
            {
                if (otherBuff.level >= level)
                {
                    duration.value = otherBuff.duration.Max(duration);
                }
            }
        }
    }
}