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
    public class BuffManager : UUIDManager<BuffManager, Buff>
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

            foreach (var buffInfo in GetAllOwnerInfos().ToList())
            {
                if (buffInfo.owner.owner == null)
                {
                    Unregister(buffInfo.owner);
                }
            }

            foreach (var buffInfo in GetAllOwnerInfos())
            {
                if (IsServerStarted)
                {
                    buffInfo.owner.Update();

                    if (buffInfo.owner.duration <= 0)
                    {
                        var buff = buffInfo.owner;
                        var entity = buff.owner;
                        if (entity != null)
                        {
                            RemoveBuffOnServer(buff, entity);
                        }
                    }
                }
                else
                {
                    if (buffInfo.isObserver)
                    {
                        buffInfo.owner.Update();
                    }
                }
            }

            if (IsServerStarted)
            {
                reconcileTimer += Time.deltaTime;

                if (reconcileTimer > reconcileInterval)
                {
                    reconcileTimer = 0;

                    foreach (var buffInfo in GetAllOwnerInfos())
                    {
                        ReconcileDurationOnObservers(buffInfo);
                    }
                }
            }
        }

        [Server]
        private static void ReconcileDurationOnObservers(OwnerInfo buffInfo)
        {
            foreach (var observer in buffInfo.observers)
            {
                ReconcileDurationOnTargetObserve(observer, buffInfo);
            }
        }

        [Server]
        private static void ReconcileDurationOnTargetObserve(int observer, OwnerInfo buffInfo)
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

            instance.ReconcileDuration(observerConnection, buffInfo.owner.uuid, buffInfo.owner.duration);
        }

        [TargetRpc]
        private void ReconcileDuration(NetworkConnection connection, string uuid, float duration)
        {
            if (TryGetInfo(uuid, out var buffInfo))
            {
                buffInfo.owner.duration.value = duration;
            }
            else
            {
                Debug.LogWarning($"不存在此{nameof(uuid)}:{uuid}对应的{nameof(buffInfo)}");
            }
        }

        #endregion

        #region Observe & Unobserve

        protected override void OnObserved(Buff buff, bool isDirty, NetworkConnection connection)
        {
            base.OnObserved(buff, isDirty, connection);

            ReconcileDuration(connection, buff.owner.uuid, buff.duration);
        }

        #endregion

        #region Add

        [ObserversRpc(ExcludeServer = true)]
        private void AddBuffOnClient(Buff buff, string entityUUID)
        {
            if (EntityManager.TryGetOwner(entityUUID, out var entity))
            {
                entity.AddBuff(buff);
            }
            else
            {
                Debug.LogWarning($"{entityUUID}对应的{nameof(Entity)}不存在");
            }
        }

        [Server]
        public static void AddBuffOnServer(Buff buff, Entity entity)
        {
            entity.AddBuff(buff);
            instance.AddBuffOnClient(buff, entity.uuid);
        }

        #endregion

        #region Remove

        [ObserversRpc(ExcludeServer = true)]
        private void RemoveBuffOnClient(string buffUUID, string entityUUID)
        {
            if (EntityManager.TryGetOwner(entityUUID, out var entity))
            {
                if (TryGetOwner(buffUUID, out var buff))
                {
                    entity.RemoveBuff(buff);
                }
                else
                {
                    Debug.LogWarning($"{buffUUID}对应的{nameof(Buff)}不存在");
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