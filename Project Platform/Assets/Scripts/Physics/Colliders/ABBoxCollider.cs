using UnityEngine;

namespace Assets.Scripts.Physics.Colliders
{
    public class ABBoxCollider : ABCollider
    {

        /// <summary>
        /// Collider width and height.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Returns min bounds (top left) of AABB.
        /// </summary>
        public Vector2 Min
        {
            get
            {
                var pos = RigidBody.GameObject.transform.position;
                return new Vector2(pos.x - Size.x / 2, pos.y + Size.y / 2);
            }
        }

        /// <summary>
        /// Returns max bounds (bottom right) of AABB.
        /// </summary>
        public Vector2 Max
        {
            get
            {
                var pos = RigidBody.GameObject.transform.position;
                return new Vector2(pos.x + Size.x / 2, pos.y - Size.y / 2);
            }
        }

        /// <summary>
        /// Return position of the collider in world coordinates.
        /// </summary>
        public Vector2 Position
        {
            get { return RigidBody.GameObject.transform.position; }
        }

        public ABBoxCollider(ABRigidBody _body) : base(_body)
        {
            Size = new Vector2(1f, 1f);
        }
    }
}
