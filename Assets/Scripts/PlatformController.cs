using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace TofuGirl
{
    /// <summary>
    /// Controls the platform movement and collisions.
    /// </summary>
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private ObjectCollision m_ObjectCollision = null;

        public Bounds PlatformBounds => m_ObjectCollision.Collider2D.bounds;

        private Vector3 m_Velocity = Vector3.zero;

        public void OnEnable()
        {
            m_ObjectCollision.OnCollisionEnter += OnPlatformCollisionEnter;
        }

        public void OnDisable()
        {
            m_ObjectCollision.OnCollisionEnter -= OnPlatformCollisionEnter;
        }

        public void Update()
        {
            transform.position += m_Velocity * Time.deltaTime;
        }

        /// <summary>
        /// Initializes platform with move speed and direction.
        /// </summary>
        /// <param name="moveSpeed">Speed</param>
        /// <param name="moveDirection">Direction</param>
        public void Initialize(float moveSpeed, Vector3 moveDirection)
        {
            m_Velocity = moveDirection * moveSpeed;
            m_ObjectCollision.ClearActiveColliders();
        }

        /// <summary>
        /// Used to detect collision detection.
        /// </summary>
        /// <param name="collisionInfo"></param>
        private void OnPlatformCollisionEnter(CollisionInfo collisionInfo)
        {
            if(collisionInfo.otherCollider.CompareTag("Player"))
            {
                m_Velocity = Vector3.zero;
            }
        }
    }
}
