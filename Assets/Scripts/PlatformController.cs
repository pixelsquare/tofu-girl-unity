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
        [SerializeField] private BoxCollider2D m_Collider2D = null;

        public Bounds PlatformBounds => m_Collider2D.bounds;

        private Vector3 m_Velocity = Vector3.zero;

        public void Update()
        {
            transform.position += m_Velocity * Time.deltaTime;
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            bool isPlayer = collision.collider.CompareTag("Player");

            if (collision.contactCount >= 2 && isPlayer)
            {
                m_Velocity = Vector3.zero;

                // Enable this if you want to retry again after the platform hits.
                //Vector3 contactNormal = collision.GetContact(0).normal;
                //bool isGameOver = contactNormal.x != 0.0f;

                //if (isGameOver)
                //{
                //    PlatformSpawner.Instance.ResetSpawner();
                //}
            }
        }

        /// <summary>
        /// Initializes platform with move speed and direction.
        /// </summary>
        /// <param name="moveSpeed">Speed</param>
        /// <param name="moveDirection">Direction</param>
        public void Initialize(float moveSpeed, Vector3 moveDirection)
        {
            m_Velocity = moveDirection * moveSpeed;
        }
    }
}
