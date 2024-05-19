using System.Collections.Generic;
using System.Linq;
using VMFramework.Configuration;
using VMFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TH.Entities
{
    public class PlayerCrouchTrigger : SerializedMonoBehaviour
    {
        [LabelText("下蹲触发区域")]
        [SerializeField]
        private RectangleFloatConfig crouchTriggerArea = new();

        [LabelText("射线检测层")]
        [SerializeField]
        private LayerMask raycastLayerMask;

        public IEnumerable<ICrouchActivatable> Raycast(int raycastCount)
        {
            var targets = new HashSet<Collider2D>();

            foreach (var x in crouchTriggerArea.xRange.GetUniformlySpacedPoints(raycastCount))
            {
                var origin = transform.position.XY() + new Vector2(x, crouchTriggerArea.max.y);

                var raycastHit2D = Physics2D.Raycast(origin, Vector2.down, crouchTriggerArea.size.y,
                    raycastLayerMask);

                if (raycastHit2D)
                {
                    targets.Add(raycastHit2D.collider);
                }
            }

            foreach (var target in targets)
            {
                if (target.TryGetComponent<ICrouchActivatableController>(out var controller))
                {
                    if (controller.crouchActivatable != null)
                    {
                        yield return controller.crouchActivatable;
                    }
                }
            }
        }

        #region Debug

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            var startPoint = transform.position.XY() + crouchTriggerArea.min;
            var endPoint = transform.position.XY() + crouchTriggerArea.max;
            GizmosUtility.DrawWireRectByMinAndMax(startPoint, endPoint);

            Gizmos.color = new(0.5f, 0.5f, 1, 1);

            foreach (var x in crouchTriggerArea.xRange.GetUniformlySpacedPoints(6))
            {

                Gizmos.DrawLine(transform.position + new Vector3(x, crouchTriggerArea.max.y),
                    transform.position + new Vector3(x, crouchTriggerArea.min.y));
            }
        }

        [Button("射线测试")]
        private List<ICrouchActivatable> RaycastTest(int raycastCount = 6)
        {
            return Raycast(raycastCount).ToList();
        }

        #endregion
    }
}
