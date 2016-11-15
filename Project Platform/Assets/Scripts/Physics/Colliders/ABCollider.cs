
using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Physics.Colliders
{
    public abstract class ABCollider
    {

        public ABRigidBody RigidBody { get; protected set; }

        /// <summary>
        /// Return position of the collider in world coordinates.
        /// </summary>
        public Vector2 Position
        {
            get { return RigidBody.GameObject.transform.position; }
        }

        protected ABCollider(ABRigidBody _body)
        {
            RigidBody = _body;
            World.Current.PhysicsWorld.AddCollider(this);
        }

    }
}
