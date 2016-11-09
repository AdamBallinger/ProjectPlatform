using System;
using Assets.Scripts.Physics.Shapes;
using UnityEngine;

namespace Assets.Scripts.Physics
{

    [Flags]
    public enum Constraints
    {
        NONE = 0,                       // 00000000
        LOCK_POSITION_X = (1 << 0),     // 00000001
        LOCK_POSITION_Y = (1 << 1),     // 00000010       - Binary flags representation.
        LOCK_ROTATION = (1 << 2),       // 00000100

        LOCK_POSITION = LOCK_POSITION_X | LOCK_POSITION_Y   // 00000011
    }

    public enum RigidBodyType
    {
        STATIC,
        DYNAMIC,
        KINEMATIC
    }

    [Serializable]
    public class ABRigidBody
    {

        public Constraints Constraints { get; private set; }

        public RigidBodyType BodyType { get; private set; }

        private float mass = 1.0f;

        public float Mass
        {
            get { return mass; }
            private set
            {
                mass = value;
                InvMass = mass == 0.0f ? 0.0f : 1.0f / mass;
            }
        }

        public float InvMass { get; set; }

        public GameObject GameObject { get; private set; }

        public Vector2 Position
        {
            get { return GameObject.transform.position; }
            set { GameObject.transform.position = value; }
        }

        public Vector2 LinearVelocity { get; private set; }

        public Vector2 Force { get; private set; }

        public float AngularVelocity { get; private set; }

        private float inertia;

        public float Inertia
        {
            get { return inertia; }
            private set
            {
                inertia = value;
                InvInertia = inertia == 0.0f ? 0.0f : 1.0f / inertia;
            }
        }

        public float InvInertia { get; private set; }

        public bool IgnoreGravity { get; set; }

        public Shape Shape { get; private set; }

        public bool Sleeping { get; set; }

        public ABRigidBody(GameObject _gameObject)
        {
            GameObject = _gameObject;
            Constraints = Constraints.NONE;
            BodyType = RigidBodyType.DYNAMIC;
            Mass = 1.0f;
            Inertia = 0.0f;
            LinearVelocity = Vector2.zero;
            Force = Vector2.zero;
            IgnoreGravity = false;
            Sleeping = false;
        }

        public void AddForce(Vector2 _force)
        {
            Force += _force;
        }

        /// <summary>
        /// Returns if this Rigidbody has the given Constraint.
        /// </summary>
        /// <param name="_constraint"></param>
        /// <returns></returns>
        public bool HasConstraint(Constraints _constraint)
        {
            return (Constraints & _constraint) == _constraint;
        }
    }
}
