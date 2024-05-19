using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using Sirenix.OdinInspector;
using TH.Buffs;
using UnityEngine;
using VMFramework.Core;
using VMFramework.OdinExtensions;

namespace TH.Entities
{
    public partial class Entity
    {
        [ShowInInspector]
        private Dictionary<string, IBuff> validBuffs = new();

        [ShowInInspector]
        private Dictionary<string, List<IBuff>> allBackgroundBuffs = new();

        public event Action<IBuff> OnBuffAddedEvent;
        public event Action<IBuff> OnBuffRemovedEvent;

        #region Has IBuff

        public bool HasBuff(string buffID)
        {
            if (TryGetValidBuff(buffID, out _))
            {
                return true;
            }

            if (TryGetBackgroundBuff(buffID, out _))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Get IBuff

        public IEnumerable<IBuff> GetAllBuffs()
        {
            return GetAllValidBuffs().Concat(GetAllBackgroundBuffs());
        }

        public IEnumerable<IBuff> GetBuff(string buffID)
        {
            if (TryGetValidBuff(buffID, out var buff))
            {
                yield return buff;
            }

            foreach (var backgroundBuff in GetBackgroundBuff(buffID))
            {
                yield return backgroundBuff;
            }
        }

        #endregion

        #region Add IBuff

        public void AddBuff(IBuff newBuff)
        {
            if (isDebugging)
            {
                Debug.LogWarning($"{this}准备添加新Buff:{newBuff}");
            }

            if (validBuffs.TryGetValue(newBuff.id, out var oldBuff))
            {
                if (newBuff.AllowBackgroundRun())
                {
                    var shouldDropOff = GetBuff(newBuff.id).Select(existedBuff =>
                        existedBuff.ShouldOtherBuffDropOffBeforeRunningBackground(newBuff)).Any();

                    if (shouldDropOff)
                    {
                        return;
                    }

                    foreach (var buff in GetAllBuffs())
                    {
                        if (buff.CompareStrength(newBuff) == 0)
                        {
                            buff.Stack(newBuff);
                            return;
                        }
                    }

                    if (newBuff.CompareStrength(oldBuff) > 0)
                    {
                        MoveValidBuffToBackground(oldBuff.id);

                        AddValidBuff(newBuff);
                    }
                    else
                    {
                        AddBackgroundBuff(newBuff);
                    }
                }
                else
                {
                    if (oldBuff.CanStack(newBuff))
                    {
                        oldBuff.Stack(newBuff);
                    }
                }
            }
            else
            {
                AddValidBuff(newBuff);
            }
        }

        protected virtual void OnBuffAddedOnServer(IBuff buff)
        {
            if (isDebugging)
            {
                Debug.LogWarning($"{this}获得Buff:{buff} On Server");
            }
        }

        protected virtual void OnBuffAddedOnClient(IBuff buff)
        {
            OnBuffAddedEvent?.Invoke(buff);

            if (isDebugging)
            {
                Debug.LogWarning($"{this}获得Buff:{buff} On Client");
            }
        }

        #endregion

        #region Remove IBuff

        public void RemoveBuff(IBuff buff)
        {
            if (RemoveValidBuff(buff))
            {
                if (HasBackgroundBuff(buff.id))
                {
                    FillValidBuffFromBackgroundBuff(buff.id);
                }
            }
            else if (RemoveBackgroundBuff(buff) == false)
            {
                Debug.LogWarning($"想要移除的{buff}不存在于实体：{this}上");
            }
        }

        protected virtual void OnBuffRemovedOnServer(IBuff buff)
        {
            if (isDebugging)
            {
                Debug.LogWarning($"{this}失去Buff:{buff} On Server");
            }
        }

        protected virtual void OnBuffRemovedOnClient(IBuff buff)
        {
            OnBuffRemovedEvent?.Invoke(buff);

            if (isDebugging)
            {
                Debug.LogWarning($"{this}失去Buff:{buff} On Client");
            }
        }

        #endregion

        #region Valid IBuff Utility

        #region Add & Remove

        private void AddOrReplaceValidBuff(IBuff buff)
        {
            RemoveValidBuff(buff.id);

            AddValidBuff(buff);
        }

        private bool AddValidBuff(IBuff buff)
        {
            if (validBuffs.ContainsKey(buff.id) == false)
            {
                buff.SetOwner(this);

                buff.OnAddToEntity(this);

                if (isServer)
                {
                    OnBuffAddedOnServer(buff);
                }

                if (isClient)
                {
                    OnBuffAddedOnClient(buff);
                }

                validBuffs.Add(buff.id, buff);

                return true;
            }

            return false;
        }

        private bool RemoveValidBuff(IBuff buff)
        {
            if (validBuffs.TryGetValue(buff.id, out var existedBuff))
            {
                if (existedBuff == buff)
                {
                    validBuffs.Remove(buff.id);

                    buff.SetOwner(null);

                    buff.OnRemoveFromEntity(this);

                    if (isServer)
                    {
                        OnBuffRemovedOnServer(buff);
                    }

                    if (isClient)
                    {
                        OnBuffRemovedOnClient(buff);
                    }

                    return true;
                }
            }

            return false;
        }

        private bool RemoveValidBuff(string buffID)
        {
            return RemoveValidBuff(buffID, out _);
        }

        private bool RemoveValidBuff(string buffID, out IBuff buff)
        {
            if (validBuffs.TryGetValue(buffID, out buff))
            {
                buff.OnRemoveFromEntity(this);

                if (isServer)
                {
                    OnBuffRemovedOnServer(buff);
                }

                if (isClient)
                {
                    OnBuffRemovedOnClient(buff);
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Get

        public IEnumerable<IBuff> GetAllValidBuffs()
        {
            return validBuffs.Values;
        }

        public IBuff GetValidBuff(string buffID)
        {
            return validBuffs.GetValueOrDefault(buffID);
        }

        public bool TryGetValidBuff(string buffID, out IBuff buff)
        {
            return validBuffs.TryGetValue(buffID, out buff);
        }

        #endregion

        #region Has

        public bool HasValidBuff(string buffID)
        {
            return validBuffs.ContainsKey(buffID);
        }

        public bool HasValidBuff()
        {
            return validBuffs.Count > 0;
        }

        #endregion

        #endregion

        #region Background IBuff Utility

        #region Add & Remove

        private void AddBackgroundBuff(IBuff buff)
        {
            if (allBackgroundBuffs.ContainsKey(buff.id) == false)
            {
                allBackgroundBuffs[buff.id] = new();
            }

            allBackgroundBuffs[buff.id].Add(buff);

            buff.SetOwner(this);
        }

        private bool RemoveBackgroundBuff(IBuff buff)
        {
            if (allBackgroundBuffs.TryGetValue(buff.id, out var buffList))
            {
                if (buffList.Remove(buff))
                {
                    buff.SetOwner(null);
                    return true;
                }
            }

            return false;
        }

        private bool RemoveHigherLevelBackgroundBuff(string buffID, out IBuff backgroundBuff)
        {
            backgroundBuff = null;

            if (allBackgroundBuffs.TryGetValue(buffID, out var buffList))
            {
                if (buffList.Count != 0)
                {
                    var higherLevelBuff = buffList.SelectMax(buff => buff.level);

                    RemoveBackgroundBuff(higherLevelBuff);

                    backgroundBuff = higherLevelBuff;
                }
            }

            return false;
        }

        #endregion

        #region Get

        public IEnumerable<IBuff> GetAllBackgroundBuffs()
        {
            foreach (var backgroundBuffs in allBackgroundBuffs.Values)
            {
                foreach (var backgroundBuff in backgroundBuffs)
                {
                    yield return backgroundBuff;
                }
            }
        }

        public IEnumerable<IBuff> GetBackgroundBuff(string buffID)
        {
            if (allBackgroundBuffs.TryGetValue(buffID, out var buffList))
            {
                foreach (var buff in buffList)
                {
                    yield return buff;
                }
            }
        }

        public bool TryGetBackgroundBuff(string buffID, out IEnumerable<IBuff> buffs)
        {
            if (allBackgroundBuffs.TryGetValue(buffID, out var buffList))
            {
                buffs = buffList;
                return true;
            }

            buffs = Enumerable.Empty<IBuff>();
            return false;
        }

        #endregion

        #region Has

        public bool HasBackgroundBuff()
        {
            return allBackgroundBuffs.Count > 0;
        }

        public bool HasBackgroundBuff(string buffID)
        {
            if (allBackgroundBuffs.TryGetValue(buffID, out var backgroundBuffs))
            {
                if (backgroundBuffs.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region Utilities

        private void MoveValidBuffToBackground(string buffID)
        {
            if (RemoveValidBuff(buffID, out var buff))
            {
                AddBackgroundBuff(buff);
            }
            else
            {
                Debug.LogWarning($"将ID:{buffID}移入背景Buff失败，因为不存在此ID对应的有效Buff");
            }
        }

        private void FillValidBuffFromBackgroundBuff(string buffID)
        {
            if (RemoveHigherLevelBackgroundBuff(buffID, out var backgroundBuff))
            {
                if (AddValidBuff(backgroundBuff) == false)
                {
                    Debug.LogWarning($"将ID:{buffID}移入有效Buff失败，" + $"因为已经存在此ID对应的有效Buff");
                }
            }
            else
            {
                Debug.LogWarning($"将ID:{buffID}移入有效Buff失败，因为不存在此ID对应的背景Buff");
            }
        }

        #endregion

        #region Debug

        [Server]
        [Button]
        private void _AddBuff(
            [GamePrefabID(typeof(BuffPreset))]
            string buffID,
            float duration = 120, int level = 1)
        {
            var buff = IBuff.Create<Buff>(buffID);
        
            buff.duration.value = duration;
            buff.level.value = level;
        
            BuffManager.AddBuffOnServer(buff, this);
        }
        
        [Server]
        [Button]
        private void _RemoveBuff(
            [GamePrefabID(typeof(BuffPreset))]
            string buffID)
        {
            if (TryGetValidBuff(buffID, out var buff))
            {
                BuffManager.RemoveBuffOnServer((Buff)buff, this);
            }
        }

        #endregion
    }
}