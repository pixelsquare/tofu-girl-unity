using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace TofuGirl
{
    /// <summary>
    /// Controls the player movement and collision.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float m_JumpForce = 50.0f;
        [SerializeField] private float m_GravityFactor = 0.5f;
        [SerializeField] private ObjectCollision m_PlayerCollision = null;

        private bool m_IsGameOver = false;
        private bool m_IsGrounded = false;
        private Vector3 m_Velocity = Vector3.zero;

        public void OnEnable()
        {
            m_PlayerCollision.OnCollisionEnter += OnPlayerCollisionEnter;
        }

        public void OnDisable()
        {
            m_PlayerCollision.OnCollisionEnter -= OnPlayerCollisionEnter;
        }

        public void Start()
        {
            m_IsGameOver = false;
            m_IsGrounded = false;
            m_Velocity = Vector3.zero;
        }

        public void Update()
        {
            if(m_IsGameOver)
            {
                if(Input.GetMouseButtonDown(1))
                {
                    m_IsGameOver = false;
                    m_PlayerCollision.ClearActiveColliders();
                    PlatformSpawner.Instance.ResetSpawner();
                }

                return;
            }

            if(Input.GetMouseButtonDown(0) && m_IsGrounded)
            {
                m_IsGrounded = false;
                m_Velocity = Vector3.up * m_JumpForce;
            }
        }

        public void FixedUpdate()
        {
            if(m_IsGameOver)
            {
                return;
            }

            m_Velocity += (Vector3)Physics2D.gravity * m_GravityFactor;

            if(m_IsGrounded)
            {
                m_Velocity = Vector3.zero;
            }

            transform.position += m_Velocity * Time.fixedDeltaTime;
        }

        /// <summary>
        /// Used to detect collision detection.
        /// </summary>
        /// <param name="collisionInfo">Collision Info</param>
        public void OnPlayerCollisionEnter(CollisionInfo collisionInfo)
        {
            m_IsGameOver = collisionInfo.otherNormal.x != 0.0f;

            if(collisionInfo.otherNormal.y > 0.0f)
            {
                m_IsGrounded = true;
                m_Velocity = Vector3.zero;

                Vector3 position = transform.position;
                position.y = collisionInfo.otherCollider.bounds.max.y;

                if(collisionInfo.otherCollider.CompareTag("Platform"))
                {
                    position.y = 0.0f;
                    PlatformSpawner.Instance.SpawnPlatform();
                }

                transform.position = position;
            }
        }
    }
}
