using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using Sirenix.OdinInspector;
using TH.Containers;
using TH.GameCore;
using UnityEngine;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.Procedure;

namespace TH.Entities
{
    [ManagerCreationProvider(nameof(GameManagerType.Entity))]
    public class PlayerManager : NetworkManagerBehaviour<PlayerManager>
    {
        [ShowInInspector]
        private static Dictionary<int, Player> allPlayers = new();

        [ShowInInspector]
        private static Player thisPlayer;

        public static bool isThisPlayerInitialized { get; private set; }

        [ServerRpc(RequireOwnership = false)]
        [Button]
        public void RequestCreatePlayer(string playerID, NetworkConnection connection = null)
        {
            if (playerID.IsNullOrEmpty() || GamePrefabManager.ContainsGamePrefab(playerID) == false)
            {
                playerID = GameSetting.playerGeneralSetting.defaultPlayerID;
            }

            if (allPlayers.TryGetValue(connection.ClientId, out var existedPlayer))
            {
                EntityManager.DestroyEntity(existedPlayer);
                
                UnregisterPlayer(connection.ClientId);
            }
            
            var player = IGameItem.Create<Player>(playerID);

            EntityManager.CreateEntity(player, Vector2.zero, connection);

            RegisterPlayer(connection.ClientId, player);
        }

        #region Register & Unregister

        public static void RegisterPlayer(int ownerID, Player player)
        {
            allPlayers[ownerID] = player;

            if (_instance.IsClientStarted)
            {
                if (ownerID == _instance.ClientManager.Connection.ClientId)
                {
                    thisPlayer = player;

                    isThisPlayerInitialized = true;
                }
            }
        }

        public static void UnregisterPlayer(int ownerID)
        {
            allPlayers.Remove(ownerID);

            if (_instance.IsClientStarted)
            {
                if (ownerID == _instance.ClientManager.Connection.ClientId)
                {
                    thisPlayer = null;

                    isThisPlayerInitialized = false;
                }
            }
        }

        #endregion

        #region Get Player

        public static bool TryGetPlayer(int ownerID, out Player player)
        {
            return allPlayers.TryGetValue(ownerID, out player);
        }

        public static Player GetPlayer(int ownerID)
        {
            return TryGetPlayer(ownerID, out var player) ? player : null;
        }

        #endregion

        #region Get This Player

        public static bool TryGetThisPlayer(out Player player)
        {
            player = thisPlayer;

            return player != null;
        }

        public static Player GetThisPlayer()
        {
            return thisPlayer;
        }

        public static PlayerController GetThisPlayerController()
        {
            return thisPlayer?.controller;
        }

        public static bool TryGetThisPlayerController(out PlayerController playerController)
        {
            playerController = thisPlayer?.controller;

            return playerController != null;
        }

        #endregion

        #region Get This Player Inventory

        public static RelicInventory GetThisPlayerRelicInventory()
        {
            return GetThisPlayer()?.relicInventory;
        }

        #endregion
    }
}
