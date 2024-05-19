using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TH.Map
{
    [RequireComponent(typeof(GameMap))]
    [DisallowMultipleComponent]

    public class GameMapNetwork : NetworkBehaviour
    {
        public struct ChunkLoadData
        {
            public Vector2Int chunkPos;
            public Dictionary<Vector2Int, Floor> allFloorsData;
        }

        public GameMap gameMap => GetComponent<GameMap>();

        [ShowInInspector]
        protected World world { get; private set; }

        [ShowInInspector]
        public bool initDone { get; private set; }

        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);

            // Debug.LogError($"客户端:{connection.ClientId}加载世界：{world}");
            InitOnTargetClient(connection, world);
        }

        [TargetRpc(ExcludeServer = true)]
        private void InitOnTargetClient(NetworkConnection connection, World world)
        {
            Init(world);
        }

        public void Init(World world)
        {
            this.world = world;

            gameMap.Init();

            WorldManager.Register(world);

            Debug.Log($"加载世界：{world}");

            world.Init(this);

            if (IsServerStarted)
            {
                var info = WorldGenerator.GenerateMap(world.generationRule);
                WorldGenerator.RenderRooms(gameMap, info);
            }

            initDone = true;
        }

        [Button(nameof(Test))]
        public void Test(Vector2Int pos)
        {
            //var data = gameMap.GetChunkData(Vector2Int.zero);
            //DestroyBlock(pos);
        }

        #region Chunk

        [TargetRpc]
        public void ResponseChunkDownload(NetworkConnection conn, ChunkLoadData data)
        {
            Debug.Log($"区块：{data.chunkPos}下载完成");

            gameMap.LoadChunkData(data);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestDownloadChunk(Vector2Int chunkPos, NetworkConnection conn = null)
        {
            Debug.Log($"{conn?.ClientId}正在下载区块:{chunkPos}");

            var data = gameMap.GetChunkData(chunkPos);

            ResponseChunkDownload(conn, data);
        }

        public async void ResponseAllChunksDownload(NetworkConnection connection,
            Action<NetworkConnection, string> onComplete)
        {
            foreach (var chunkPos in gameMap.GetAllChunksPos())
            {
                Debug.Log($"{connection.ClientId}正在下载区块:{chunkPos}");

                ResponseChunkDownload(connection, gameMap.GetChunkData(chunkPos));

                await UniTask.NextFrame();
            }

            onComplete(connection, world.uuid);
        }

        #region Debug

        [Button("请求下载区块")]
        private void RequestDownloadChunkDebugging(Vector2Int chunkPos)
        {
            RequestDownloadChunk(chunkPos);
        }

        #endregion

        #endregion

        [Server]
        public void DestroyBlockOnServer(Vector2Int pos, FloorDestructionInfo info)
        {
            //Debug.LogWarning($"破坏{pos}");
            gameMap.DestroyFloor(pos, info);
            DestroyBlockOnClients(pos);
        }

        [ObserversRpc(ExcludeServer = true)]
        private void DestroyBlockOnClients(Vector2Int pos)
        {
            //Debug.LogWarning($"破坏{pos}");
            gameMap.DestroyFloor(pos, new FloorDestructionInfo()
            {
                enableDroppings = false
            });
        }

        //[ServerRpc(RequireOwnership = false)]
        //public void DestroyBlockRequest(Vector2Int pos, Block block, NetworkConnection conn = null)
        //{
        //    if (block == null)
        //    {
        //        return;
        //    }

        //    var currentBlock = gameMap.GetBlock(pos);

        //    if (currentBlock == null)
        //    {
        //        DestroyBlockReconcileOnClients(pos, null);
        //        return;
        //    }

        //    if (currentBlock.IsSameAs(block) == false)
        //    {
        //        DestroyBlockReconcileOnClients(pos, currentBlock);
        //        return;
        //    }

        //    gameMap.DestroyBlock(pos, oldBlock =>
        //    {
        //        oldBlock.OnDestroyOnServer(null);

        //        if (IsClient)
        //        {
        //            oldBlock.OnDestroyOnClient(null);
        //        }
        //    });
        //    DestroyBlockReconcileOnClients(pos, null);
        //}

        //[ObserversRpc(ExcludeServer = true)]
        //private void DestroyBlockReconcileOnClients(Vector2Int pos, Block blockOnServer)
        //{
        //    if (blockOnServer == null)
        //    {
        //        gameMap.DestroyBlock(pos, oldBlock => oldBlock.OnDestroyOnClient(null));
        //    }
        //    else
        //    {
        //        gameMap.ReplaceBlock(pos, blockOnServer);
        //    }
        //}

        //[ServerRpc(RequireOwnership = false)]
        //public void PlaceBlockRequest(Vector2Int pos, Block block, NetworkConnection conn = null)
        //{
        //    if (block == null)
        //    {
        //        return;
        //    }

        //    var currentBlock = gameMap.GetBlock(pos);

        //    if (currentBlock != null)
        //    {
        //        PlaceBlockReconcileOnClients(pos, currentBlock);
        //        return;
        //    }

        //    gameMap.FillBlock(pos, block, newBlock =>
        //    {
        //        newBlock.OnPlaceOnServer();

        //        if (IsClient)
        //        {
        //            newBlock.OnPlaceOnClient();
        //        }
        //    });
        //    PlaceBlockReconcileOnClients(pos, block);
        //}

        //[ObserversRpc(ExcludeServer = true)]
        //private void PlaceBlockReconcileOnClients(Vector2Int pos, Block blockOnServer)
        //{
        //    if (blockOnServer == null)
        //    {
        //        Debug.LogWarning($"{nameof(PlaceBlockReconcileOnClients)}的block参数为Null");
        //        return;
        //    }

        //    var blockOnClient = gameMap.GetBlock(pos);

        //    if (blockOnClient == null)
        //    {
        //        gameMap.FillBlock(pos, blockOnServer, newBlock => newBlock.OnPlaceOnClient());
        //    }
        //    else
        //    {
        //        if (blockOnClient.IsSameAs(blockOnServer) == false)
        //        {
        //            gameMap.ReplaceBlock(pos, blockOnServer, newBlock => newBlock.OnPlaceOnClient());
        //        }
        //    }
        //}

    }
}
