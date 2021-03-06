﻿using System;
using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Physics
{

    [Flags]
    public enum Constraints
    {
        NONE = 0,                       // 00000000
        LOCK_POSITION_X = (1 << 0),     // 00000001
        LOCK_POSITION_Y = (1 << 1),     // 00000010       - Binary flags representation.

        LOCK_POSITION = LOCK_POSITION_X | LOCK_POSITION_Y   // 00000011
    }

    [Serializable]
    public class ABRigidBody
    {

        public Constraints Constraints { get; private set; }

        /// <summary>
        /// Physics material contains the physical properties of this rigidbody (Restitution and friction etc.)
        /// </summary>
        public PhysicsMaterial Material { get; set; }

        private float mass = 1.0f;

        /// <summary>
        /// Rigidbody mass.
        /// </summary>
        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;
                InvMass = mass == 0.0f ? 0.0f : 1.0f / mass;
            }
        }

        /// <summary>
        /// Rigidbody inverse mass.
        /// </summary>
        public float InvMass { get; private set; }

        /// <summary>
        /// Unity GameObject reference for this Rigidbody.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// World position / center of this rigid body.
        /// </summary>
        public Vector2 Position
        {
            get { return GameObject != null ? (Vector2)GameObject.transform.position : Vector2.zero; }
            set { GameObject.transform.position = value; }
        }

        /// <summary>
        /// Current velocity for the rigidbody.
        /// </summary>
        public Vector2 LinearVelocity { get; set; }

        /// <summary>
        /// Current body force
        /// </summary>
        public Vector2 Force { get; set; }

        /// <summary>
        /// Current angular velocity for the body.
        /// </summary>
        public float AngularVelocity { get; set; }

        /// <summary>
        /// Current body torque.
        /// </summary>
        public float Torque { get; set; }

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

        /// <summary>
        /// Should this body ignore effects of gravity.
        /// </summary>
        public bool IgnoreGravity { get; set; }

        /// <summary>
        /// If the body is sleeping, it receives no physics updates until it is woken up.
        /// </summary>
        public bool Sleeping { get; set; }


        public ABRigidBody(GameObject _gameObject)
        {
            GameObject = _gameObject;
            Constraints = Constraints.NONE;
            Material = new PhysicsMaterial();
            Mass = 100.0f;
            Inertia = 10.0f;
            LinearVelocity = Vector2.zero;
            Force = Vector2.zero;
            IgnoreGravity = false;
            Sleeping = false;

            World.Current.PhysicsWorld.AddRigidBody(this);
        }

        /// <summary>
        /// Add a force to the rigid body.
        /// </summary>
        /// <param name="_force"></param>
        public void AddForce(Vector2 _force)
        {
            if(HasConstraint(Constraints.LOCK_POSITION_X))
            {
                _force.x = 0.0f;
            }

            if(HasConstraint(Constraints.LOCK_POSITION_Y))
            {
                _force.y = 0.0f;
            }

            Force += _force;
        }

        /// <summary>
        /// Adds an immediate force to the rigid body.
        /// </summary>
        /// <param name="_impulse"></param>
        public void AddImpulse(Vector2 _impulse)
        {
            if (HasConstraint(Constraints.LOCK_POSITION_X))
            {
                _impulse.x = 0.0f;
            }

            if (HasConstraint(Constraints.LOCK_POSITION_Y))
            {
                _impulse.y = 0.0f;
            }

            LinearVelocity += InvMass * _impulse;
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

        /// <summary>
        /// Adds a given constraint to the body.
        /// </summary>
        /// <param name="_constraint"></param>
        public void AddConstraint(Constraints _constraint)
        {
            // remove the none consraint if the given constraint isn't none.
            if(_constraint != Constraints.NONE)
            {
                Constraints &= ~Constraints.NONE;
                Constraints |= _constraint;
            }
            else
            {
                // If adding the NONE constraint set the current constraints to none, clearing any previous constraints.
                Constraints = Constraints.NONE;
            }           
        }

        /// <summary>
        /// Sets this bodies constraints to the given constraints, clearing any previously set constraints.
        /// </summary>
        /// <param name="_constraints"></param>
        public void SetConstraints(Constraints _constraints)
        {
            Constraints = _constraints;
        }
    }
}
