using FishNet.Connection;
using FishNet.Object;
using Sirenix.OdinInspector;
using TH.GameCore;
using UnityEngine;
using VMFramework.Network;
using VMFramework.Procedure;

namespace TH.Spells
{
    [ManagerCreationProvider(nameof(GameManagerType.Spell))]
    public class SpellManager : UUIDManager<SpellManager, Spell>
    {
        #region Observe & Unobserve

        protected override void OnObserved(Spell spell, bool isDirty, NetworkConnection connection)
        {
            base.OnObserved(spell, isDirty, connection);

            ReconcileCooldown(connection, spell.uuid, spell.cooldown);
        }

        #endregion

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

            foreach (var spellInfo in GetAllOwnerInfos())
            {
                if (IsServerStarted)
                {
                    spellInfo.owner.Update();
                }
                else
                {
                    if (spellInfo.isObserver)
                    {
                        spellInfo.owner.Update();
                    }
                }
            }

            if (IsServerStarted)
            {
                reconcileTimer += Time.deltaTime;

                if (reconcileTimer > reconcileInterval)
                {
                    reconcileTimer = 0;

                    foreach (var spellInfo in GetAllOwnerInfos())
                    {
                        ReconcileCooldownOnObservers(spellInfo);
                    }
                }
            }
        }

        [Server]
        private static void ReconcileCooldownOnObservers(OwnerInfo spellInfo)
        {
            foreach (var observer in spellInfo.observers)
            {
                ReconcileCooldownOnTargetObserve(observer, spellInfo);
            }
        }

        [Server]
        private static void ReconcileCooldownOnTargetObserve(int observer, OwnerInfo spellInfo)
        {
            if (_instance.ServerManager.Clients.TryGetValue(observer, out var observerConnection) == false)
            {
                Debug.LogWarning($"不存在此观察者:{observer}对应的{nameof(NetworkConnection)}");
                return;
            }

            if (observerConnection.IsHost)
            {
                return;
            }

            _instance.ReconcileCooldown(observerConnection, spellInfo.owner.uuid, spellInfo.owner.cooldown);
        }

        [TargetRpc]
        private void ReconcileCooldown(NetworkConnection connection, string uuid, float cooldown)
        {
            if (TryGetInfo(uuid, out var spellInfo))
            {
                spellInfo.owner.cooldown.value = cooldown;
            }
            else
            {
                Debug.LogWarning($"不存在此{nameof(uuid)}:{uuid}对应的{nameof(spellInfo)}");
            }
        }

        #endregion

        #region Cast

        [Server]
        private static void CastInstantaneously(string uuid, Spell.SpellCastInfo spellCastInfo)
        {
            if (TryGetInfo(uuid, out var spellInfo))
            {
                spellInfo.owner.Cast(spellCastInfo);

                ReconcileCooldownOnObservers(spellInfo);
            }
            else
            {
                Debug.LogWarning($"不存在此{nameof(uuid)}:{uuid}对应的{nameof(spellInfo)}");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void CastRequest(string uuid, Spell.SpellCastInfo spellCastInfo)
        {
            CastInstantaneously(uuid, spellCastInfo);
        }

        public static void Cast(Spell spell, Spell.SpellCastInfo spellCastInfo)
        {
            if (_instance.IsServerStarted)
            {
                CastInstantaneously(spell.uuid, spellCastInfo);
            }
            else
            {
                _instance.CastRequest(spell.uuid, spellCastInfo);
            }
        }

        #endregion
    }
}
