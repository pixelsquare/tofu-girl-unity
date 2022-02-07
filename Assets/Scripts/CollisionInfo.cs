using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace TofuGirl
{
    /// <summary>
    /// Basic custom collision information.
    /// </summary>
    public struct CollisionInfo
    {
        public Collider2D collider;
        public Collider2D otherCollider;

        public Vector2 normal;
        public Vector2 otherNormal;
    }
}
