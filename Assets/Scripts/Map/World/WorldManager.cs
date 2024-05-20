using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using TH.GameCore;
using UnityEngine;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.Network;
using VMFramework.Procedure;

namespace TH.Map
{
    [ManagerCreationProvider(nameof(GameManagerType.Map))]
    public class WorldManager : UUIDManager<WorldManager, World>
    {
        [ShowInInspector]
        private static readonly Dictionary<World, bool> isDownloadCompletedDict = new();

        public static WorldGeneralSetting setting => GameSetting.worldGeneralSetting;

        [ShowInInspector]
        private readonly SyncVar<string> _currentWorldUUID =
            new(new SyncTypeSettings(WritePermission.ServerOnly));

        [ShowInInspector]
        private static World currentWorld;

        public static string currentWorldUUID => _instance._currentWorldUUID.Value;

        private void Awake()
        {
            _currentWorldUUID.OnChange += OnCurrentWorldChanged;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            SetCurrentWorld(CreateWorldOnServer(setting.defaultWorldPreset));
        }

        #region Create

        [Server]
        public static World CreateWorldOnServer(string worldPresetID)
        {
            var worldContainer = setting.container.GetContainer();
            var prefab = setting.prefab;

            var gameMapObject = Instantiate(prefab.gameObject, worldContainer);

            InstanceFinder.ServerManager.Spawn(gameMapObject);

            var gameMapNetwork = gameMapObject.GetComponent<GameMapNetwork>();

            var world = IGameItem.Create<World>(worldPresetID);

            gameMapObject.name = world.name;

            gameMapNetwork.Init(world);

            return world;
        }

        #endregion

        #region Current World

        public static World GetCurrentWorld()
        {
            if (currentWorldUUID.IsNullOrEmpty())
            {
                return null;
            }

            if (UUIDCoreManager.TryGetOwnerWithWarning(currentWorldUUID, out World world) == false)
            {
                return null;
            }

            return world;
        }

        public static GameMap GetCurrentGameMap()
        {
            var world = GetCurrentWorld();

            if (world == null)
            {
                return null;
            }

            return world.gameMap;
        }

        public static GameMapNetwork GetCurrentGameMapNetwork()
        {
            var world = GetCurrentWorld();

            if (world == null)
            {
                return null;
            }

            return world.gameMapNetwork;
        }

        private static void SetCurrentWorld(World world)
        {
            currentWorld = world;

            _instance._currentWorldUUID.Value = world.uuid;
        }

        private void OnCurrentWorldChanged(string oldWorldUUID, string newWorldUUID, bool asServer)
        {
            if (asServer)
            {
                return;
            }

            if (ClientManager.Connection.IsHost)
            {
                return;
            }

            RequestDownloadAllChunks(newWorldUUID);
        }

        #endregion

        #region Chunks

        [ServerRpc(RequireOwnership = false)]
        private void RequestDownloadAllChunks(string worldUUID, NetworkConnection connection = null)
        {
            if (UUIDCoreManager.TryGetOwnerWithWarning(worldUUID, out World world) == false)
            {
                return;
            }

            world.gameMapNetwork.ResponseAllChunksDownload(connection, DownloadAllChunksComplete);
        }

        [TargetRpc(ExcludeServer = true)]
        private void SendAllChunksDownloadCompleteToClient(NetworkConnection connection, string worldUUID)
        {
            if (UUIDCoreManager.TryGetOwnerWithWarning(worldUUID, out World world) == false)
            {
                return;
            }

            SetDownloadCompleted(world, true);
        }

        private static void DownloadAllChunksComplete(NetworkConnection connection, string worldUUID)
        {
            _instance.SendAllChunksDownloadCompleteToClient(connection, worldUUID);
        }

        #endregion

        #region Download Completed

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetDownloadCompleted(World world, bool value)
        {
            isDownloadCompletedDict[world] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDownloadCompleted(World world)
        {
            return CollectionExtensions.GetValueOrDefault(isDownloadCompletedDict, world, false);
        }

        #endregion
    }
}