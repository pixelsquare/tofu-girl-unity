using UnityEngine;
using UnityEngine.Events;

using System.Collections;
using System.Collections.Generic;

namespace TofuGirl
{
    /// <summary>
    /// Custom collision detection implementation.
    /// </summary>
    public class ObjectCollision : MonoBehaviour
    {
        public UnityAction<CollisionInfo> OnCollisionEnter = null;
        public UnityAction<CollisionInfo> OnCollisionStay = null;
        public UnityAction<CollisionInfo> OnCollisionExit = null;

        [SerializeField] private Collider2D m_Collider2D = null;

        public Collider2D Collider2D => m_Collider2D;

        private List<Collider2D> m_ColliderHits = new List<Collider2D>();
        private List<Collider2D> m_ActiveColliders = new List<Collider2D>();

        public void Update()
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(1 << LayerMask.NameToLayer("Default"));

            int collisionCount = m_Collider2D.OverlapCollider(contactFilter, m_ColliderHits);

            for(int i = 0; i < collisionCount; i++)
            {
                if(!m_ActiveColliders.Contains(m_ColliderHits[i]))
                {
                    m_ActiveColliders.Add(m_ColliderHits[i]);

                    CollisionInfo info = new CollisionInfo()
                    {
                        collider = m_Collider2D,
                        otherCollider = m_ColliderHits[i],
                        normal = GetColliderNormal(m_Collider2D, m_ColliderHits[i], false),
                        otherNormal = GetColliderNormal(m_Collider2D, m_ColliderHits[i], true)
                    };

                    OnCollisionEnter?.Invoke(info);
                }
            }

            for(int i = 0; i < m_ActiveColliders.Count; i++)
            {
                CollisionInfo info = new CollisionInfo()
                {
                    collider = m_Collider2D,
                    otherCollider = m_ActiveColliders[i],
                    normal = GetColliderNormal(m_Collider2D, m_ActiveColliders[i], false),
                    otherNormal = GetColliderNormal(m_Collider2D, m_ActiveColliders[i], true)
                };

                OnCollisionStay?.Invoke(info);
            }

            if(collisionCount != m_ActiveColliders.Count)
            {
                for(int i = 0; i < m_ActiveColliders.Count; i++)
                {
                    if(!m_ColliderHits.Contains(m_ActiveColliders[i]))
                    {
                        CollisionInfo info = new CollisionInfo()
                        {
                            collider = m_Collider2D,
                            otherCollider = m_ActiveColliders[i],
                            normal = GetColliderNormal(m_Collider2D, m_ActiveColliders[i], false),
                            otherNormal = GetColliderNormal(m_Collider2D, m_ActiveColliders[i], true)
                        };

                        OnCollisionExit?.Invoke(info);
                        m_ActiveColliders.Remove(m_ActiveColliders[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the surface normal of the collision.
        /// </summary>
        /// <param name="collider">Collider</param>
        /// <param name="otherCollider">Other collider</param>
        /// <param name="isOther">Use other normal</param>
        /// <returns></returns>
        private Vector2 GetColliderNormal(Collider2D collider, Collider2D otherCollider, bool isOther)
        {
            Bounds colliderBounds = collider.bounds;
            Bounds otherColliderBounds = otherCollider.bounds;

            float dx = isOther 
                ? collider.transform.position.x - otherCollider.transform.position.x 
                : otherCollider.transform.position.x - collider.transform.position.x;
            float px = (otherColliderBounds.size.x + colliderBounds.size.x) - Mathf.Abs(dx);

            float dy = isOther 
                ? collider.transform.position.y - otherCollider.transform.position.y 
                : otherCollider.transform.position.y - collider.transform.position.y;
            float py = (otherColliderBounds.size.y + colliderBounds.size.y) - Mathf.Abs(dy);

            Vector2 otherColliderNormal = Vector2.zero;

            if(px < py)
            {
                otherColliderNormal.x = Mathf.Sign(dx);
            }
            else
            {
                otherColliderNormal.y = Mathf.Sign(dy);
            }

            return otherColliderNormal;
        }

        /// <summary>
        /// Clears all active colliders.
        /// </summary>
        public void ClearActiveColliders()
        {
            m_ActiveColliders.Clear();
        }
    }
}
