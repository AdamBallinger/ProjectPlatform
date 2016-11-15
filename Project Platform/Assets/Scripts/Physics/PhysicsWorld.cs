using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Assets.Scripts.General.UnityLayer;
using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.Physics
{
    public class PhysicsWorld
    {

        /// <summary>
        /// World gravity per axis. (m/s)
        /// </summary>
        public Vector2 Gravity { get; set; }

        /// <summary>
        /// A list of rigid bodies for the current physics world.
        /// </summary>
        private List<ABRigidBody> RigidBodies { get; set; }

        /// <summary>
        /// A list of all collider objects in the current physics world.
        /// </summary>
        private List<ABCollider> Colliders { get; set; }

        /// <summary>
        /// Store a list of collision pairs that have collided.
        /// </summary>
        private List<CollisionInfoPair> Contacts { get; set; }

        /// <summary>
        /// Initialize the physics world with physics settings.
        /// </summary>
        /// <param name="_gravity">Gravity value in meters per second.</param>
        public void Initialize(Vector2 _gravity)
        {
            RigidBodies = new List<ABRigidBody>();
            Colliders = new List<ABCollider>();
            Contacts = new List<CollisionInfoPair>();
            Gravity = _gravity;
        }

        /// <summary>
        /// Adds a rigidbody to the world if it doesn't already exist.
        /// </summary>
        /// <param name="_body"></param>
        public void AddRigidBody(ABRigidBody _body)
        {
            if(RigidBodies.Contains(_body))
            {
                Debug.LogWarning("Can't add duplicate rigid body!");
                return;
            }

            Debug.Log("Added Body: " + _body.GameObject.name);
            RigidBodies.Add(_body);
        }

        /// <summary>
        /// Adds a collider to the world if it doesn't already exist.
        /// </summary>
        /// <param name="_collider"></param>
        public void AddCollider(ABCollider _collider)
        {
            if(Colliders.Contains(_collider))
            {
                Debug.LogWarning("Can't add duplicate collider!");
                return;
            }

            Debug.Log("Added collider: " + _collider.RigidBody.GameObject.name);
            Colliders.Add(_collider);
        }

        /// <summary>
        /// Removes a rigidbody from the world if it exists.
        /// </summary>
        /// <param name="_body"></param>
        public void RemoveBody(ABRigidBody _body)
        {
            if(RigidBodies.Contains(_body))
            {
                RigidBodies.Remove(_body);
                return;
            }

            Debug.LogWarning("Attempted to remove a body from world that didn't exist.");
        }

        /// <summary>
        /// Removes a collider from the world if it exists.
        /// </summary>
        /// <param name="_collider"></param>
        public void RemoveCollider(ABCollider _collider)
        {
            if(Colliders.Contains(_collider))
            {
                Colliders.Remove(_collider);
                return;
            }

            Debug.LogWarning("Attempted to remove a collider from the world that didn't exist.");
        }

        /// <summary>
        /// Performs the physics step for the world.
        /// </summary>
        public void Step()
        {
            // Apply forces to each rigid body in the world.
            foreach (var body in RigidBodies)
            {
                // Ignore infinite mass bodies, sleeping bodies and static bodies.
                if(body.InvMass == 0.0f || body.Sleeping || body.BodyType == RigidBodyType.STATIC) continue;
                
                var acceleration = body.Force * body.InvMass;

                if(body.IgnoreGravity)
                {
                    body.LinearVelocity += acceleration * Time.fixedDeltaTime;
                }
                else
                {
                    body.LinearVelocity += (Gravity + acceleration) * Time.fixedDeltaTime;
                }

                //var angular_acceleration = body.Torque == 0.0f ? 0.0f : body.Torque / body.Mass;

                //body.AngularVelocity = angular_acceleration / Time.fixedDeltaTime;

                body.Position += body.LinearVelocity * Time.fixedDeltaTime;

                // Clear forces
                body.Force = Vector2.zero;
                body.Torque = 0f;
            }

            // Generate collision pairs
            GeneratePairs();

            // Resolve any collisions.
            foreach(var contact in Contacts)
            {
                // Resolve collisions (Impulse method)
                for (var i = 0; i < 5; i++)
                {
                    contact.ApplyImpulse();
                }
                contact.CorrectPosition();
            }

            // Perform position correction.
            foreach (var contact in Contacts)
            {
                for (var i = 0; i < 1; i++)
                {
                   // contact.CorrectPosition();
                }
            }
        }

        // Broadphase
        private void GeneratePairs()
        {
            // Clear previous contacts and collision pairs.
            Contacts.Clear();

            for(var i = 0; i < Colliders.Count; ++i)
            {
                var collider1 = Colliders[i];
                for(var j = i + 1; j < Colliders.Count; ++j)
                {
                    var collider2 = Colliders[j];
                    // Prevent collider self check.
                    if (collider2 == collider1) continue;

                    // Check collisions between each collider.
                    var colliderPair = new CollisionInfoPair(collider1, collider2);
                   
                    colliderPair.Solve();

                    if(colliderPair.ContactDetected)
                    {
                        Contacts.Add(colliderPair);
                    }
                }
            }
        }
    }
}
