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

        public void AddRigidBody(ABRigidBody _body)
        {
            if(RigidBodies.Contains(_body))
            {
                Debug.LogWarning("Can't add duplicate rigid body!");
                return;
            }

            RigidBodies.Add(_body);
        }

        public void AddCollider(ABCollider _collider)
        {
            if(Colliders.Contains(_collider))
            {
                Debug.LogWarning("Can't add duplicate collider!");
                return;
            }

            Colliders.Add(_collider);
        }

        public void RemoveBody(ABRigidBody _body)
        {
            if(RigidBodies.Contains(_body))
            {
                RigidBodies.Remove(_body);
                return;
            }

            Debug.LogWarning("Attempted to remove a body from world that didn't exist.");
        }

        public void RemoveCollider(ABCollider _collider)
        {
            if(Colliders.Contains(_collider))
            {
                Colliders.Remove(_collider);
                return;
            }

            Debug.LogWarning("Attempted to remove a collider from the world that didn't exist.");
        }

        public void Step()
        {
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
            }

            // Generate collision pairs
            GeneratePairs();

            foreach(var contact in Contacts)
            {
                // Solve collisions by n iterations (Impulse collision)
                for(var i = 0; i < 1; i++)
                {
                    contact.ApplyImpulse();
                }
            }

            // Perform position correction.
            foreach (var contact in Contacts)
            {
                contact.CorrectPosition();
            }

            foreach(var body in RigidBodies)
            {
                // Clear forces
                body.Force = Vector2.zero;
                body.Torque = 0f;
            }
        }

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
