using UnityEngine;

namespace Assets.Scripts.Physics
{
    public class ABSpringJoint
    {

        public ABRigidBody BodyA { get; private set; }

        public ABRigidBody BodyB { get; private set; }

        public float Stiffness { get; set; }

        public float RestLength { get; set; }

        public float Dampen { get; set; }

        /// <summary>
        /// Return distance between the two bodies.
        /// </summary>
        public Vector2 Distance
        {
            get { return BodyB.Position - BodyA.Position; }
        }


        public ABSpringJoint(ABRigidBody _a, ABRigidBody _b)
        {
            BodyA = _a;
            BodyB = _b;
            Stiffness = 20000.0f;
            RestLength = 1.0f;
            Dampen = 200.0f;
        }
    }
}
