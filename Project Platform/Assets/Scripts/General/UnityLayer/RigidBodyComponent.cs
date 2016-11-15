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

        public bool inspectorCreated = false;

        [SerializeField]
        private float mass = 1.0f;

        [SerializeField]
        private bool ignoreGravity;

        public void Start()
        {
            if(inspectorCreated)
            {
                Init();
                RigidBody.AddImpulse(Vector2.right * 4f);
            }
        }

        public void Init()
        {
            ClearBody();
            RigidBody = new ABRigidBody(gameObject);
            RigidBody.Mass = mass;
            RigidBody.IgnoreGravity = ignoreGravity;
        }

        /// <summary>
        /// When unity destroys an object, make sure its cleared from the world.
        /// </summary>
        public void OnDestroy()
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
