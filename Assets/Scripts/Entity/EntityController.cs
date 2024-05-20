using Cysharp.Threading.Tasks;
using VMFramework.Core;
using FishNet.Connection;
using FishNet.Object;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.GameLogicArchitecture;
using VMFramework.Network;
using VMFramework.UI;

namespace TH.Entities
{
    public class EntityController : NetworkBehaviour, ITracingTooltipProviderController
    {
        #region Config

        [SerializeField]
        [Required]
        private Transform _graphicTransform;

        public Transform graphicTransform => _graphicTransform;

        protected override void Reset()
        {
            base.Reset();

            foreach (var child in transform.GetChildren())
            {
                if (child.name.ToLower().Contains("graphic"))
                {
                    _graphicTransform = child;
                    break;
                }
            }
        }

        #endregion

        [ShowInInspector]
        public Entity entity { get; private set; }

        #region Spawn & Despawn

        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);

            InitOnClient(connection, entity);
        }

        public override void OnDespawnServer(NetworkConnection connection)
        {
            base.OnDespawnServer(connection);

            DestroyOnClient(connection);
        }

        #endregion

        #region RPC

        [TargetRpc(ExcludeServer = true)]
        private void InitOnClient(NetworkConnection connection, Entity entity)
        {
            Init(entity);

            if (connection.IsHost == false)
            {
                UUIDCoreManager.Register(entity);
            }
        }

        [TargetRpc(ExcludeServer = true)]
        private void DestroyOnClient(NetworkConnection connection)
        {
            IGameItem.Destroy(entity);
        }

        #endregion

        #region Init

        public bool initDone { get; private set; }

        public async void Init(Entity entity)
        {
            this.entity = entity;

            OnPreInit();
            
            OnInit();

            entity.Init(this);

            await UniTask.WaitUntil(() => IsNetworked);
            
            OnPostInit();

            initDone = true;

            Enable();
        }

        protected virtual void OnPreInit()
        {

        }

        protected virtual void OnInit()
        {

        }

        protected virtual void OnPostInit()
        {
            
        }

        #endregion

        #region Hide & Show

        public void Hide()
        {
            OnEntityControllerHide();
        }

        public void Show()
        {
            OnEntityControllerShow();
        }

        protected virtual void OnEntityControllerHide()
        {
            graphicTransform.SetActive(false);
        }

        protected virtual void OnEntityControllerShow()
        {
            graphicTransform.SetActive(true);
        }

        #endregion

        #region Enable & Disable

        public void Enable()
        {
            gameObject.SetActive(true);

            OnEntityControllerEnable();
        }

        public void Disable()
        {
            OnEntityControllerDisable();

            gameObject.SetActive(false);
        }

        protected virtual void OnEntityControllerEnable()
        {

        }

        protected virtual void OnEntityControllerDisable()
        {

        }

        #endregion

        #region Debug

        protected const string DEBUGGING_GROUP = "调试";

        [Button("破坏此实体"), TitleGroup(DEBUGGING_GROUP)]
        [Server]
        [HideInEditorMode]
        private void DestroyThisEntity()
        {
            EntityManager.DestroyEntity(entity);
        }

        #endregion

        public ITracingTooltipProvider provider => entity;
    }
}
