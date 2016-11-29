using Assets.Scripts.Physics;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.Physics_Components
{
    public class SpringJointComponent : MonoBehaviour
    {

        public ABSpringJoint Joint { get; private set; }

        public RigidBodyComponent BodyA;

        public RigidBodyComponent BodyB;

        public float stiffness = 20000.0f;

        public float restLength = 1.0f;

        public float dampen = 200.0f;

        public Constraints jointConstraints = Constraints.LOCK_POSITION_X;

        [SerializeField]
        private bool inspectorCreated = false;


        public void Start()
        {
            if(inspectorCreated)
            {
                Create(BodyA.RigidBody, BodyB.RigidBody);
            }
        }

        public void Create(ABRigidBody _a, ABRigidBody _b)
        {
            if(Joint != null)
            {
                ClearSpringJoint();
            }

            _a.SetConstraints(jointConstraints);
            Joint = new ABSpringJoint(_a, _b);
            Joint.Stiffness = stiffness;
            Joint.RestLength = restLength;
            Joint.Dampen = dampen;

            World.Current.PhysicsWorld.AddSpringJoint(Joint);
        }

        // When unity destroys this object, make sure if the game isnt being closed, then the spring is removed from the physics world
        public void OnDestroy()
        {
            ClearSpringJoint();
        }

        /// <summary>
        /// Removes the collider for this component from the world.
        /// </summary>
        public void ClearSpringJoint()
        {
            if (Joint == null) return;

            World.Current.PhysicsWorld.RemoveSpringJoint(Joint);
            Joint = null;
        }
    }
}
