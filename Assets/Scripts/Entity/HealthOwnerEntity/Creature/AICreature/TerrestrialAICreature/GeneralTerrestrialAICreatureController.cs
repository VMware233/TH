using System.Collections.Generic;
using VMFramework.Configuration;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.OdinExtensions;
using VMFramework.Core;

namespace TH.Entities
{
    public class GeneralTerrestrialAICreatureController : AICreatureController
    {
        #region Config

        [LabelText("检测点")]
        [Required]
        [SerializeField]
        private Transform detectionTransform;

        [LabelText("检测半径")]
        [MinValue(0)]
        [SerializeField]
        private float detectionRadius;

        [LabelText("检测角度范围")]
        [Minimum(-360), Maximum(360)]
        [SerializeField]
        private RangeFloatConfig detectionAngleRange = new();

        [LabelText("检测射线数量")]
        [MinValue(2)]
        [SerializeField]
        private int detectionRayCount;

        [LabelText("检测间隔")]
        [MinValue(0)]
        [SerializeField]
        private float detectionInterval = 1f;

        #endregion

        public GeneralTerrestrialAICreature generalTerrestrialAICreature =>
            entity as GeneralTerrestrialAICreature;

        private new Rigidbody2D rigidbody;

        #region Init

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        #endregion

        #region Sight Detection

        private IEnumerable<Vector2> GetDetectionRayDirections(LeftRightDirection direction)
        {
            if (direction.HasFlag(LeftRightDirection.Right))
            {
                foreach (var rayDirection in detectionAngleRange.min
                             .GetUniformlySpacedClockwiseAngleDirections(detectionAngleRange.max,
                                 detectionRayCount))
                {
                    yield return rayDirection;
                }
            }

            if (direction.HasFlag(LeftRightDirection.Left))
            {
                foreach (var rayDirection in (detectionAngleRange.min + 180)
                         .GetUniformlySpacedClockwiseAngleDirections(detectionAngleRange.max + 180,
                             detectionRayCount))
                {
                    yield return rayDirection;
                }
            }
        }

        private IEnumerable<Entity> DetectEntitiesWithinSight(LeftRightDirection direction)
        {
            foreach (var rayDirection in GetDetectionRayDirections(direction))
            {
                var hits = Physics2D.RaycastAll(detectionTransform.position, rayDirection, detectionRadius);

                foreach (var hit in hits)
                {
                    if (hit && hit.TryGetEntity(out var entity))
                    {
                        yield return entity;
                    }
                }
            }
        }

        private bool IsPlayerWithinSight(LeftRightDirection direction, out Player player)
        {
            foreach (var entity in DetectEntitiesWithinSight(direction))
            {
                if (entity is Player playerEntity)
                {
                    player = playerEntity;
                    return true;
                }
            }

            player = null;
            return false;
        }

        #endregion

        #region Pursuit

        private Transform pursuitTargetTransform;

        public void Pursue(Transform targetTransform)
        {
            pursuitTargetTransform = targetTransform;
        }

        public void StopPursuing()
        {
            pursuitTargetTransform = null;
        }

        public bool IsPursuing()
        {
            return pursuitTargetTransform != null;
        }

        private void MoveTowardsPursuitTarget()
        {
            if (IsPursuing() == false)
            {
                return;
            }

            var direction = (pursuitTargetTransform.position - transform.position).x.Sign();

            rigidbody.velocity =
                rigidbody.velocity.ReplaceX(direction * generalTerrestrialAICreature.movementSpeed);
        }

        #endregion

        #region Patrol

        private void Patrol()
        {
            rigidbody.velocity = rigidbody.velocity.ReplaceX(0);
        }

        #endregion

        private float detectionTimer = 0;

        private void Update()
        {
            if (IsServerStarted)
            {
                if (detectionTimer <= 0)
                {
                    if (IsPlayerWithinSight(LeftRightDirection.All, out var player))
                    {
                        Pursue(player.controller.transform);
                    }
                    else
                    {
                        StopPursuing();
                    }

                    detectionTimer = detectionInterval;
                }
                else
                {
                    detectionTimer -= Time.deltaTime;
                }

                if (IsPursuing())
                {
                    MoveTowardsPursuitTarget();
                }
                else
                {
                    Patrol();
                }
            }
        }

        #region Debug

        private void OnDrawGizmos()
        {
            if (detectionTransform == null)
            {
                return;
            }

            foreach (var rayDirection in GetDetectionRayDirections(LeftRightDirection.All))
            {
                Gizmos.DrawRay(detectionTransform.position, rayDirection * detectionRadius);
            }
        }

        #endregion
    }
}
