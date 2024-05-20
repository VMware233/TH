using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using Sirenix.OdinInspector;
using TH.Entities;
using TH.GameCore;
using UnityEngine;
using VMFramework.Network;
using VMFramework.Procedure;

namespace TH.Buffs
{
    [ManagerCreationProvider(nameof(GameManagerType.Buff))]
    public class BuffManager : UUIDManager<BuffManager, IBuff>
    {
        #region Update

        [SerializeField]
        private float reconcileInterval = 1f;

        [ShowInInspector]
        private float reconcileTimer = 0;

        private void Update()
        {
            if (IsNetworked == false)
            {
                return;
            }

            foreach (var buff in GetAllOwners().Where(buff => buff.owner == null).ToList())
            {
                UUIDCoreManager.Unregister(buff);
            }

            foreach (var buff in GetAllOwners())
            {
                if (IsServerStarted)
                {
                    buff.Update();

                    if (buff.duration <= 0)
                    {
                        var entity = buff.owner;
                        if (entity != null)
                        {
                            RemoveBuffOnServer(buff, entity);
                        }
                    }
                }
                else
                {
                    if (buff.IsObserver())
                    {
                        buff.Update();
                    }
                }
            }

            if (IsServerStarted)
            {
                reconcileTimer += Time.deltaTime;

                if (reconcileTimer > reconcileInterval)
                {
                    reconcileTimer = 0;

                    foreach (var buff in GetAllOwners())
                    {
                        ReconcileDurationOnObservers(buff);
                    }
                }
            }
        }

        [Server]
        private static void ReconcileDurationOnObservers(IBuff buff)
        {
            foreach (var observer in buff.GetObservers())
            {
                ReconcileDurationOnTargetObserve(observer, buff);
            }
        }

        [Server]
        private static void ReconcileDurationOnTargetObserve(int observer, IBuff buff)
        {
            if (instance.ServerManager.Clients.TryGetValue(observer, out var observerConnection) == false)
            {
                Debug.LogWarning($"不存在此观察者:{observer}对应的{nameof(NetworkConnection)}");
                return;
            }

            if (observerConnection.IsHost)
            {
                return;
            }

            instance.ReconcileDuration(observerConnection, buff.uuid, buff.duration);
        }

        [TargetRpc]
        private void ReconcileDuration(NetworkConnection connection, string uuid, float duration)
        {
            if (UUIDCoreManager.TryGetOwnerWithWarning(uuid, out IBuff buff))
            {
                buff.duration = duration;
            }
        }

        #endregion

        #region Observe & Unobserve

        protected override void OnObserved(IBuff buff, bool isDirty, NetworkConnection connection)
        {
            base.OnObserved(buff, isDirty, connection);

            ReconcileDuration(connection, buff.owner.uuid, buff.duration);
        }

        #endregion

        #region Add

        [ObserversRpc(ExcludeServer = true)]
        private void AddBuffOnClient(IBuff buff, string entityUUID)
        {
            if (UUIDCoreManager.TryGetOwner(entityUUID, out Entity entity))
            {
                entity.AddBuff(buff);
            }
            else
            {
                Debug.LogWarning($"{entityUUID}对应的{nameof(Entity)}不存在");
            }
        }

        [Server]
        public static void AddBuffOnServer(IBuff buff, Entity entity)
        {
            entity.AddBuff(buff);
            instance.AddBuffOnClient(buff, entity.uuid);
        }

        #endregion

        #region Remove

        [ObserversRpc(ExcludeServer = true)]
        private void RemoveBuffOnClient(string buffUUID, string entityUUID)
        {
            if (UUIDCoreManager.TryGetOwner(entityUUID, out Entity entity))
            {
                if (UUIDCoreManager.TryGetOwner(buffUUID, out IBuff buff))
                {
                    entity.RemoveBuff(buff);
                }
                else
                {
                    Debug.LogWarning($"{buffUUID}对应的{nameof(IBuff)}不存在");
                }
            }
            else
            {
                Debug.LogWarning($"{entityUUID}对应的{nameof(Entity)}不存在");
            }
        }

        [Server]
        public static void RemoveBuffOnServer(IBuff buff, Entity entity)
        {
            if (entity == null)
            {
                return;
            }

            entity.RemoveBuff(buff);
            instance.RemoveBuffOnClient(buff.uuid, entity.uuid);
        }

        #endregion
    }
}