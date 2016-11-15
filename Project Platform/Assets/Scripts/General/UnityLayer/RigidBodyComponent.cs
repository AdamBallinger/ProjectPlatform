using Assets.Scripts.Physics;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    /// <summary>
    /// Rigid body component to allow custom rigid body class to interact with Unity component system.
    /// </summary>
    public class RigidBodyComponent : MonoBehaviour
    {

        public ABRigidBody RigidBody { get; private set; }

        [SerializeField]
        private float mass = 1.0f;

        [SerializeField]
        private bool ignoreGravity;

        public void Start()
        {
            Init(new ABRigidBody(gameObject));
        }

        public void Init(ABRigidBody _rigidBody)
        {
            RigidBody = _rigidBody;
            RigidBody.Mass = mass;
            RigidBody.IgnoreGravity = ignoreGravity;
            //RigidBody.AddImpulse(Vector2.right * 4f);
        }

        /// <summary>
        /// When unity destroys an object, make sure its cleared from the world.
        /// </summary>
        public void Destroy()
        {
            ClearBody();
        }

        public void SetMass(float _mass)
        {
            mass = _mass;
        }

        public void SetIgnoreGravity(bool _gravity)
        {
            ignoreGravity = _gravity;
        }

        /// <summary>
        /// Removes the current body from the physics world.
        /// </summary>
        public void ClearBody()
        {
            World.Current.PhysicsWorld.RemoveBody(RigidBody);
        }
    }
}
