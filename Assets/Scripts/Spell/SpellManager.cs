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

            foreach (var spell in GetAllOwners())
            {
                if (IsServerStarted)
                {
                    spell.Update();
                }
                else
                {
                    if (spell.IsObserver())
                    {
                        spell.Update();
                    }
                }
            }

            if (IsServerStarted)
            {
                reconcileTimer += Time.deltaTime;

                if (reconcileTimer > reconcileInterval)
                {
                    reconcileTimer = 0;

                    foreach (var spell in GetAllOwners())
                    {
                        ReconcileCooldownOnObservers(spell);
                    }
                }
            }
        }

        [Server]
        private static void ReconcileCooldownOnObservers(Spell spell)
        {
            foreach (var observer in spell.GetObservers())
            {
                ReconcileCooldownOnTargetObserve(observer, spell);
            }
        }

        [Server]
        private static void ReconcileCooldownOnTargetObserve(int observer, Spell spell)
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

            _instance.ReconcileCooldown(observerConnection, spell.uuid, spell.cooldown);
        }

        [TargetRpc]
        private void ReconcileCooldown(NetworkConnection connection, string uuid, float cooldown)
        {
            if (UUIDCoreManager.TryGetOwnerWithWarning(uuid, out Spell spell))
            {
                spell.cooldown.value = cooldown;
            }
        }

        #endregion

        #region Cast

        [Server]
        private static void CastInstantaneously(string uuid, Spell.SpellCastInfo spellCastInfo)
        {
            if (UUIDCoreManager.TryGetOwnerWithWarning(uuid, out Spell spell))
            {
                spell.Cast(spellCastInfo);

                ReconcileCooldownOnObservers(spell);
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
