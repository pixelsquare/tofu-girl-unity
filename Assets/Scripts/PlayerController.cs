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

        private bool m_IsGameOver = false;
        private bool m_IsGrounded = false;
        private Vector3 m_Velocity = Vector3.zero;

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

        public void OnCollisionEnter2D(Collision2D collision)
        {
            bool isPlatform = collision.collider.CompareTag("Platform");
            bool isBasePlatform = collision.collider.CompareTag("BasePlatform");

            if(collision.contactCount >= 2 && (isBasePlatform || isPlatform))
            {
                m_Velocity = Vector3.zero;

                Vector3 contactNormal = collision.GetContact(1).normal;
                bool didClear = contactNormal.y > 0.5f;
                m_IsGameOver = contactNormal.x != 0.0f;

                if(didClear)
                {
                    m_IsGrounded = true;
                    Vector3 position = transform.position;
                    position.y = collision.collider.bounds.max.y;

                    if(isPlatform)
                    {
                        position.y = 0.0f;
                        PlatformSpawner.Instance.SpawnPlatform();
                    }

                    transform.position = position;
                }

            }
        }
    }
}
