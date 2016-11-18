using UnityEngine;

namespace Assets.Scripts.Physics.Colliders
{
    public class ABBoxCollider : ABCollider
    {

        /// <summary>
        /// Collider width and height.
        /// </summary>
        public Vector2 Size { get; set; }

        public Vector2 Min { get; private set; }

        public Vector2 Max { get; private set; }

        public ABBoxCollider(ABRigidBody _body) : base(_body)
        {
            Size = new Vector2(1f, 1f);
        }

        /// <summary>
        /// Computes the AABB min and max bounds.
        /// </summary>
        public void ComputeAABB()
        {
            Min = new Vector2(RigidBody.Position.x - Size.x / 2, RigidBody.Position.y + Size.y / 2);
            Max = new Vector2(RigidBody.Position.x + Size.x / 2, RigidBody.Position.y - Size.y / 2);
        }
    }
}
