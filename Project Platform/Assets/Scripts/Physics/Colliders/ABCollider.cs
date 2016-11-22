
using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Physics.Colliders
{
    public abstract class ABCollider
    {

        public ABRigidBody RigidBody { get; protected set; }

        public Vector2 Offset { get; set; }

        /// <summary>
        /// Return position of the collider in world coordinates.
        /// </summary>
        public Vector2 Position
        {
            get { return RigidBody.Position + Offset; }
        }

        public CollisionListener CollisionListener { get; private set; }

        /// <summary>
        /// Is the collider a trigger? I.e Triggers allow collisions to pass through but will fire an OnTriggerEnter callback to the object this collider enters.
        /// </summary>
        public bool IsTrigger { get; set; }

        protected ABCollider(ABRigidBody _body)
        {
            RigidBody = _body;
            CollisionListener = new CollisionListener(this);
            IsTrigger = false;
            World.Current.PhysicsWorld.AddCollider(this);
        }

    }
}
