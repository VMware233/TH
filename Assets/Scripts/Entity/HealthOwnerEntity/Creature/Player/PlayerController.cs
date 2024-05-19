using System;
using VMFramework.Core;
using FishNet.Connection;
using Sirenix.OdinInspector;
using Spine;
using UnityEngine;
using Spine.Unity;
using TH.Map;
using VMFramework.Core.FSM;
using VMFramework.GameLogicArchitecture;
using AnimationState = Spine.AnimationState;

namespace TH.Entities
{
    public class PlayerController : CreatureController
    {
        #region Config

        [field: LabelText("施法位置")]
        [field: Required]
        [field: SerializeField]
        public Transform castPositionTransform { get; private set; }

        #endregion

        public new Rigidbody2D rigidbody { get; private set; }

        public Player player { get; private set; }

        public SkeletonAnimation skeletonAnimation;
        AnimationState animationState;
        Skeleton skeleton;

        public PlayerGrounded playerGrounded { get; private set; }

        public bool isGrounded => playerGrounded.isGrounded;

        public bool fallGrounded => playerGrounded.fallGrounded;

        public PlayerCrouchTrigger playerCrouchTrigger { get; private set; }

        #region Init

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.AssertIsNotNull(nameof(rigidbody));

            playerGrounded = GetComponent<PlayerGrounded>();
            playerGrounded.AssertIsNotNull(nameof(playerGrounded));

            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            skeletonAnimation.AssertIsNotNull(nameof(skeletonAnimation));
            skeleton = skeletonAnimation.Skeleton;
            animationState = skeletonAnimation.AnimationState;

            playerCrouchTrigger = GetComponent<PlayerCrouchTrigger>();
            playerCrouchTrigger.AssertIsNotNull(nameof(playerCrouchTrigger));
        }

        protected override void OnPreInit()
        {
            base.OnPreInit();

            player = entity as Player;
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            InitFSM();
        }

        protected override void OnPostInit()
        {
            base.OnPostInit();
            
            if (IsServerStarted == false)
            {
                PlayerManager.RegisterPlayer(Owner.ClientId, player);
            }

            if (Owner.IsLocalClient)
            {
                CameraManager.mainCameraController.SetFollowTarget(transform);

                InitChunkPos();
            }
        }

        #endregion

        #region Despawn

        public override void OnDespawnServer(NetworkConnection connection)
        {
            base.OnDespawnServer(connection);
        }

        #endregion

        #region Update

        private void Update()
        {
            if (initDone == false)
            {
                return;
            }

            if (IsOwner)
            {
                fsm.Update();
            }

            UpdateChunkPos();
        }

        #endregion

        #region Chunk Pos

        [ShowInInspector]
        public Vector2Int chunkPos { get; private set; }

        public event Action<Vector2Int, Vector2Int> OnChunkPosChanged;

        private void InitChunkPos()
        {
            var world = WorldManager.GetCurrentWorld();

            chunkPos = world.gameMap.GetChunkPosByRealPosition(transform.position);

            OnChunkPosChanged += (oldChunkPos, newChunkPos) =>
            {
                WorldManager.GetCurrentWorld().gameMap.RenderChunksFromPivot(newChunkPos);
            };

            OnChunkPosChanged?.Invoke(chunkPos, chunkPos);
        }

        private void UpdateChunkPos()
        {
            var world = WorldManager.GetCurrentWorld();

            var newChunkPos = world.gameMap.GetChunkPosByRealPosition(transform.position);

            if (newChunkPos != chunkPos)
            {
                chunkPos = newChunkPos;
                OnChunkPosChanged?.Invoke(chunkPos, newChunkPos);
            }
        }

        #endregion

        #region FixedUpdate

        private void FixedUpdate()
        {
            if (fsm is { initDone: true } && IsOwner)
            {
                fsm.FixedUpdate();
            }
        }

        #endregion

        #region IMultiStateFSM

        [ShowInInspector]
        protected IMultiFSM<string, PlayerController> fsm;

        private void InitFSM()
        {
            fsm = new MultiFSM<string, PlayerController>();

            foreach (var config in player.GetInitialPlayerActionStateConfigs())
            {
                fsm.AddState(IGameItem.Create<PlayerActionState>(config.playerActionStateID));
            }

            fsm.Init(this);

            foreach (var config in player.GetInitialPlayerActionStateConfigs())
            {
                if (config.autoEnter)
                {
                    fsm.EnterState(config.playerActionStateID);
                }
            }
        }

        #endregion
    }
}
