
using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Physics.Colliders
{
    public abstract class ABCollider
    {

        public ABRigidBody RigidBody { get; protected set; }

        protected ABCollider(ABRigidBody _body)
        {
            RigidBody = _body;
            World.Current.PhysicsWorld.AddCollider(this);
        }

    }
}
