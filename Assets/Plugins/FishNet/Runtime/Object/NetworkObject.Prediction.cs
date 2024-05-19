﻿namespace FishNet.Object
{
#if PREDICTION_V2
    public partial class NetworkObject : MonoBehaviour
    {
        #region Types.
#if PREDICTION_V2
        /// <summary>
        /// Type of prediction movement being used.
        /// </summary>
        [System.Serializable]
        internal enum PredictionType : byte
        {
            Other = 0,
            Rigidbody = 1,
            Rigidbody2D = 2
        }
#endif
        #endregion

        #region Public.
        /// <summary>
        /// Last tick this object replicated.
        /// </summary>
        internal EstimatedTick ReplicateTick { get; private set; } = new EstimatedTick();
        /// <summary>
        /// Last tick to replicate even if out of order. This could be from tick events or even replaying inputs.
        /// </summary>
        internal uint LastUnorderedReplicateTick;
        #endregion

        #region Internal.
        /// <summary>
        /// Pauses rigidbodies for prediction.
        /// </summary>
        public RigidbodyPauser RigidbodyPauser { get; private set; }
        #endregion

        #region Serialized.
#if PREDICTION_V2
        /// <summary>
        /// True if this object uses prediciton methods.
        /// </summary>
        public bool EnablePrediction => _enablePrediction;
        [Tooltip("True if this object uses prediction methods.")]
        [SerializeField]
        private bool _enablePrediction;
        /// <summary>
        /// What type of component is being used for prediction? If not using rigidbodies set to other.
        /// </summary>
        [Tooltip("What type of component is being used for prediction? If not using rigidbodies set to other.")]
        [SerializeField]
        private PredictionType _predictionType = PredictionType.Other;
        /// <summary>
        /// Object containing graphics when using prediction. This should be child of the predicted root.
        /// </summary>
        [Tooltip("Object containing graphics when using prediction. This should be child of the predicted root.")]
        [SerializeField]
        private Transform _graphicalObject;
        /// <summary>
        /// True to forward replicate and reconcile states to all clients. This is ideal with games where you want all clients and server to run the same inputs. False to only use prediction on the owner, and synchronize to spectators using other means such as a NetworkTransform.
        /// </summary>
        [Tooltip("True to forward replicate and reconcile states to all clients. This is ideal with games where you want all clients and server to run the same inputs. False to only use prediction on the owner, and synchronize to spectators using other means such as a NetworkTransform.")]
        [SerializeField]
        private bool _enableStateForwarding = true;
        /// <summary>
        /// How many ticks to interpolate graphics on objects owned by the client. Typically low as 1 can be used to smooth over the frames between ticks.
        /// </summary>
        [Tooltip("How many ticks to interpolate graphics on objects owned by the client. Typically low as 1 can be used to smooth over the frames between ticks.")]
        [Range(1, byte.MaxValue)]
        [SerializeField]
        private byte _ownerInterpolation = 1;
        /// <summary>
        /// True to enable teleport threshhold.
        /// </summary>
        [Tooltip("True to enable teleport threshhold.")]
        [SerializeField]
        private bool _enableTeleport;
        /// <summary>
        /// Distance the graphical object must move between ticks to teleport the transform properties.
        /// </summary>
        [Tooltip("Distance the graphical object must move between ticks to teleport the transform properties.")]
        [Range(0.001f, ushort.MaxValue)]
        [SerializeField]
        private float _teleportThreshold = 1f;
#endif
        #endregion

        #region Private.
        /// <summary>
        /// Graphical smoother to use when using set for owner.
        /// </summary>
        private PredictionTickSmoother _tickSmoother;
        /// <summary>
        /// NetworkBehaviours which use prediction.
        /// </summary>
        private List<NetworkBehaviour> _predictionBehaviours = new List<NetworkBehaviour>();
        ///// <summary>
        ///// Tick when CollionStayed last called. This only has value if using prediction.
        ///// </summary>
        //private uint _collisionStayedTick;
        ///// <summary>
        ///// Local client objects this object is currently colliding with.
        ///// </summary>
        //private HashSet<GameObject> _localClientCollidedObjects = new HashSet<GameObject>();
        #endregion

        private void Prediction_Update()
        {
            if (!_enablePrediction)
                return;

            _tickSmoother?.Update();
        }

        private void TimeManager_OnPreTick()
        {
            _tickSmoother?.OnPreTick();
        }
        private void TimeManager_OnPostTick()
        {
            _tickSmoother?.OnPostTick();
            //TrySetCollisionExited();
        }

        private void Prediction_Preinitialize(NetworkManager manager, bool asServer)
        {
            if (!_enablePrediction)
                return;

            ReplicateTick.Initialize(manager.TimeManager);
            InitializeSmoothers();

            if (asServer)
                return;

            if (_predictionBehaviours.Count > 0)
            {
                manager.PredictionManager.OnPreReconcile += PredictionManager_OnPreReconcile;
                manager.PredictionManager.OnReplicateReplay += PredictionManager_OnReplicateReplay;
                manager.PredictionManager.OnPostReconcile += PredictionManager_OnPostReconcile;
                manager.TimeManager.OnPreTick += TimeManager_OnPreTick;
                manager.TimeManager.OnPostTick += TimeManager_OnPostTick;
            }
        }

        private void Prediction_Deinitialize(bool asServer)
        {
            if (!_enablePrediction)
                return;

            DeinitializeSmoothers();
            /* Only the client needs to unsubscribe from these but
             * asServer may not invoke as false if the client is suddenly
             * dropping their connection. */
            if (_predictionBehaviours.Count > 0 && NetworkManager != null)
            {
                NetworkManager.PredictionManager.OnPreReconcile -= PredictionManager_OnPreReconcile;
                NetworkManager.PredictionManager.OnReplicateReplay -= PredictionManager_OnReplicateReplay;
                NetworkManager.PredictionManager.OnPostReconcile -= PredictionManager_OnPostReconcile;
                NetworkManager.TimeManager.OnPreTick -= TimeManager_OnPreTick;
                NetworkManager.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }


        /// <summary>
        /// Initializes tick smoothing.
        /// </summary>
        private void InitializeSmoothers()
        {
            bool usesRb = (_predictionType == PredictionType.Rigidbody);
            bool usesRb2d = (_predictionType == PredictionType.Rigidbody2D);
            if (usesRb || usesRb2d)
            {
                RigidbodyPauser = new RigidbodyPauser();
                RigidbodyType rbType = (usesRb) ? RigidbodyType.Rigidbody : RigidbodyType.Rigidbody2D;
                RigidbodyPauser.UpdateRigidbodies(transform, rbType, true);
            }

            if (_graphicalObject == null)
            {
                Debug.Log($"GraphicalObject is null on {this.ToString()}. This may be intentional, and acceptable, if you are smoothing between ticks yourself. Otherwise consider assigning the GraphicalObject field.");
            }
            else
            {
                _tickSmoother = new PredictionTickSmoother();
                float teleportT = (_enableTeleport) ? _teleportThreshold : MoveRatesCls.UNSET_VALUE;
                _tickSmoother.InitializeOnce(_graphicalObject, teleportT, this, _ownerInterpolation);
            }
        }

        /// <summary>
        /// Initializes tick smoothing.
        /// </summary>
        private void DeinitializeSmoothers()
        {
            _tickSmoother?.Deinitialize();
        }

        private void PredictionManager_OnPreReconcile(uint clientReconcileTick, uint serverReconcileTick)
        {
            for (int i = 0; i < _predictionBehaviours.Count; i++)
                _predictionBehaviours[i].Reconcile_Client_Start();
        }

        private void PredictionManager_OnPostReconcile(uint clientReconcileTick, uint serverReconcileTick)
        {
            for (int i = 0; i < _predictionBehaviours.Count; i++)
                _predictionBehaviours[i].Reconcile_Client_End();

            /* Unpause rigidbody pauser. It's okay to do that here rather
             * than per NB, where the pausing occurs, because once here
             * the entire object is out of the replay cycle so there's
             * no reason to try and unpause per NB. */
            RigidbodyPauser?.Unpause();
        }


        private void PredictionManager_OnReplicateReplay(uint clientTick, uint serverTick)
        {
            uint replayTick = (IsOwner) ? clientTick : serverTick;
            for (int i = 0; i < _predictionBehaviours.Count; i++)
                _predictionBehaviours[i].Replicate_Replay_Start(replayTick + 1);
        }

        ///// <summary>
        ///// Returns if this object is colliding with any local client objects.
        ///// </summary>
        ///// <returns></returns>
        //internal bool CollidingWithLocalClient()
        //{
        //    /* If it's been more than 1 tick since collision stayed
        //     * then do not consider as collided. */
        //    return (TimeManager.LocalTick - _collisionStayedTick) <= 1;
        //}

        ///// <summary>
        ///// Called when colliding with another object.
        ///// </summary>
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (!IsClientInitialized)
        //        return;
        //    if (_predictionType != PredictionType.Rigidbody)
        //        return;

        //    GameObject go = collision.gameObject;
        //    if (CollisionEnteredLocalClientObject(go))
        //        CollisionEntered(go);
        //}

        ///// <summary>
        ///// Called when collision has entered a local clients object.
        ///// </summary>
        //private void CollisionEntered(GameObject go)
        //{
        //    if (_graphicalObject == null)
        //        return;

        //    _collisionStayedTick = TimeManager.LocalTick;
        //    _localClientCollidedObjects.Add(go);
        //}

        ///// <summary>
        ///// Called when colliding with another object.
        ///// </summary>
        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    if (_graphicalObject == null)
        //        return;
        //    if (!IsClientInitialized)
        //        return;
        //    if (_predictionType != PredictionType.Rigidbody2D)
        //        return;

        //    GameObject go = collision.gameObject;
        //    if (CollisionEnteredLocalClientObject(go))
        //        CollisionEntered(go);
        //}


        ///// <summary>
        ///// Called when staying in collision with another object.
        ///// </summary>
        //private void OnCollisionStay(Collision collision)
        //{
        //    if (!IsClientInitialized)
        //        return;
        //    if (_predictionType != PredictionType.Rigidbody)
        //        return;

        //    if (_localClientCollidedObjects.Contains(collision.gameObject))
        //        _collisionStayedTick = TimeManager.LocalTick;
        //}
        ///// <summary>
        ///// Called when staying in collision with another object.
        ///// </summary>
        //private void OnCollisionStay2D(Collision2D collision)
        //{
        //    if (!IsClientInitialized)
        //        return;
        //    if (_predictionType != PredictionType.Rigidbody2D)
        //        return;

        //    if (_localClientCollidedObjects.Contains(collision.gameObject))
        //        _collisionStayedTick = TimeManager.LocalTick;
        //}

        ///// <summary>
        ///// Called when a collision occurs and the smoothing type must perform operations.
        ///// </summary>
        //private bool CollisionEnteredLocalClientObject(GameObject go)
        //{
        //    if (go.TryGetComponent<NetworkObject>(out NetworkObject nob))
        //        return nob.Owner.IsLocalClient;

        //    //Fall through.
        //    return false;
        //}


        ///// <summary>
        ///// Called when collision has exited a local clients object.
        ///// </summary>
        //private void TrySetCollisionExited()
        //{
        //    /* If this object is no longer
        //     * colliding with local client objects
        //     * then unset collision.
        //     * This is done here instead of using
        //     * OnCollisionExit because often collisionexit
        //     * will be missed due to ignored ticks. 
        //     * While not ignoring ticks is always an option
        //     * its not ideal because ignoring ticks helps
        //    * prevent over predicting. */
        //    TimeManager tm = TimeManager;
        //    if (tm == null || (_collisionStayedTick != 0 && (tm.LocalTick != _collisionStayedTick)))
        //    {
        //        _localClientCollidedObjects.Clear();
        //        _collisionStayedTick = 0;
        //    }
        //}

        /// <summary>
        /// Registers a NetworkBehaviour that uses prediction with the NetworkObject.
        /// This method should only be called once throughout the entire lifetime of this object.
        /// </summary>
        internal void RegisterPredictionBehaviourOnce(NetworkBehaviour nb)
        {
            _predictionBehaviours.Add(nb);
        }

        /// <summary>
        /// Resets replicate tick and unordered replicate tick.
        /// </summary>
        internal void ResetReplicateTick()
        {
            ReplicateTick.Reset();
            LastUnorderedReplicateTick = 0;
        }

        /// <summary>
        /// Sets the last tick this NetworkBehaviour replicated with.
        /// </summary>
        internal void SetReplicateTick(uint value, bool setUnordered)
        {
            if (setUnordered)
                LastUnorderedReplicateTick = value;

            ReplicateTick.Update(NetworkManager.TimeManager, value, EstimatedTick.OldTickOption.Discard);
            if (Owner.IsValid)
                Owner.ReplicateTick.Update(NetworkManager.TimeManager, value, EstimatedTick.OldTickOption.Discard);
        }


    }
#endif
}

