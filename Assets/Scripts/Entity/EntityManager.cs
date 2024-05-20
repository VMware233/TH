using FishNet;
using FishNet.Connection;
using FishNet.Object;
using Sirenix.OdinInspector;
using TH.Damage;
using TH.GameCore;
using TH.Items;
using UnityEngine;
using VMFramework.Core;
using VMFramework.GameLogicArchitecture;
using VMFramework.Network;
using VMFramework.OdinExtensions;
using VMFramework.Procedure;

namespace TH.Entities
{
    [ManagerCreationProvider(nameof(GameManagerType.Entity))]
    public class EntityManager : UUIDManager<EntityManager, Entity>
    {
        #region Create & Destroy

        [Server]
        public static EntityController CreateEntity(Entity entity, Vector2 position,
            NetworkConnection ownerConnection = null)
        {
            entity.AssertIsNotNull(nameof(entity));
            entity.prefab.AssertIsNotNull(nameof(entity.prefab));

            var gameObject = InstanceFinder.NetworkManager.GetPooledInstantiated(entity.prefab, true);
            //Instantiate(entity.prefab);

            gameObject.transform.position = position;

            var entityController = gameObject.GetComponent<EntityController>();

            entityController.Init(entity);

            InstanceFinder.ServerManager.Spawn(gameObject, ownerConnection);

            return entityController;
        }

        [Server]
        public static void DestroyEntity(Entity entity)
        {
            if (entity == null)
            {
                Debug.LogWarning("entity为Null，无法破坏");
                return;
            }

            if (UUIDCoreManager.TryGetInfo(entity.uuid, out var _))
            {
                IGameItem.Destroy(entity);

                entity.controller.Disable();

                InstanceFinder.ServerManager.Despawn(entity.controller.gameObject, DespawnType.Pool);
            }
        }

        #endregion

        #region Item Drop

        [Server]
        public static ItemDropController CreateItemDrop(Item item, Vector2 position)
        {
            var itemDrop = IGameItem.Create<ItemDrop>(ItemDropConfig.ID);

            itemDrop.SetItem(item);

            return CreateEntity(itemDrop, position, null) as ItemDropController;
        }

        #endregion

        #region Debug

        [Button("创建实体")]
        [Server]
        public static EntityController CreateEntity(
            [LabelText("实体")] [GamePrefabID(typeof(EntityConfig))] string entityID,
            [LabelText("位置")] Vector2 position)
        {
            var entity = IGameItem.Create<Entity>(entityID);

            return CreateEntity(entity, position);
        }

        [Button("在玩家位置创建实体")]
        [Server]
        public static EntityController CreateEntityOnThisPlayer(
            [LabelText("实体")] [GamePrefabID(typeof(EntityConfig))] string entityID)
        {
            if (PlayerManager.isThisPlayerInitialized == false)
            {
                Debug.LogError("此方法只能在玩家初始化之后调用");
                return null;
            }

            var entity = IGameItem.Create<Entity>(entityID);

            return CreateEntity(entity, PlayerManager.GetThisPlayerController().transform.position);
        }

        [Button("创建掉落物")]
        [Server]
        public static ItemDropController CreateItemDrop(
            [LabelText("物品")] [GamePrefabID(typeof(ItemPreset))] string itemID,
            [LabelText("位置")] Vector2 position)
        {
            var item = Item.Create(itemID, 1);

            return CreateItemDrop(item, position);
        }

        [Button("创建线性投射物")]
        [Server]
        public static GeneralLinearProjectileController CreateLinearProjectile(
            [LabelText("投射物")] [GamePrefabID(typeof(ILinearProjectileConfig))]
            string projectileID, [LabelText("位置")] Vector2 position, [LabelText("速度")] Vector2 velocity,
            [LabelText("伤害")] DamagePacket damagePacket)
        {
            var projectile = IGameItem.Create<ILinearProjectile>(projectileID);

            var initInfo = new GeneralLinearProjectile.InitInfo
            {
                damagePacket = damagePacket,
                initialVelocity = velocity
            };

            projectile.Init(initInfo);

            return CreateEntity((Entity)projectile, position) as GeneralLinearProjectileController;
        }

        [Button("在玩家位置创建固定激光束")]
        [Server]
        public static GeneralFixedLaserBeamController CreateFixedLaserBeamOnThisPlayer(
            [LabelText("投射物")]
            [GamePrefabID(typeof(IFixedLaserBeamConfig))]
            string projectileID, [LabelText("方向")] Vector2 direction,
            [LabelText("伤害")] DamagePacket damagePacket)
        {
            if (PlayerManager.isThisPlayerInitialized == false)
            {
                Debug.LogError("此方法只能在玩家初始化之后调用");
                return null;
            }

            var projectile = IGameItem.Create<IFixedLaserBeam>(projectileID);

            var initInfo = new GeneralFixedLaserBeam.InitInfo
            {
                damagePacket = damagePacket,
                direction = direction
            };

            projectile.Init(initInfo);

            var position = PlayerManager.GetThisPlayerController().castPositionTransform.position;

            return CreateEntity((Entity)projectile, position) as GeneralFixedLaserBeamController;
        }

        #endregion
    }
}
