using Sirenix.OdinInspector;
using UnityEngine;

namespace TH.Entities
{
    public class PlayerGrounded : MonoBehaviour
    {
        [ShowInInspector]
        public bool isGrounded { get; private set; }

        [ShowInInspector]
        public bool fallGrounded => isGrounded && rigidbody.velocity.y <= 0;

        private new Rigidbody2D rigidbody;

        [SerializeField]
        private Vector2 checkPivot;

        [SerializeField]
        private float checkRadius = 0.2f;

        [SerializeField]
        private LayerMask layerMask;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            isGrounded = Physics2D.OverlapCircle(
                (Vector2)transform.position + checkPivot, checkRadius, layerMask);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Vector2)transform.position + checkPivot, checkRadius);
        }
    }
}
