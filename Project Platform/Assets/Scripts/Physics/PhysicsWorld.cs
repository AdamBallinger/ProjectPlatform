using System.Collections.Generic;
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
        /// Number of iterations to solve collisions. Higher value nets better collision results at the cost of performance.
        /// Any value over 10 is typically not needed.
        /// </summary>
        public int SolveIterations { get; set; }

        /// <summary>
        /// Maximum allowed velocity magnitude for a rigidbody
        /// </summary>
        public float MaxBodyVelocity { get; set; }

        /// <summary>
        /// A list of rigid bodies for the current physics world.
        /// </summary>
        private List<ABRigidBody> RigidBodies { get; set; }

        /// <summary>
        /// A list of all collider objects in the current physics world.
        /// </summary>
        private List<ABCollider> Colliders { get; set; }

        /// <summary>
        /// A list of all spring joints in the current physics world.
        /// </summary>
        private List<ABSpringJoint> Springs { get; set; }

        /// <summary>
        /// Store a list of collision pairs that have collided.
        /// </summary>
        private List<CollisionManifold> Contacts { get; set; }

        /// <summary>
        /// Initialize the physics world with physics settings.
        /// </summary>
        /// <param name="_gravity">Gravity value in meters per second.</param>
        /// <param name="_iterations"></param>
        /// <param name="_maxVelocity"></param>
        public void Initialize(Vector2 _gravity, int _iterations, float _maxVelocity)
        {
            Gravity = _gravity;
            SolveIterations = _iterations;
            MaxBodyVelocity = _maxVelocity;
            RigidBodies = new List<ABRigidBody>();
            Colliders = new List<ABCollider>();
            Springs = new List<ABSpringJoint>();
            Contacts = new List<CollisionManifold>();
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

            Colliders.Add(_collider);
        }

        public void AddSpringJoint(ABSpringJoint _joint)
        {
            if(Springs.Contains(_joint))
            {
                Debug.LogWarning("Can't add duplicate spring joints!");
                return;
            }

            Springs.Add(_joint);
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
        /// Removes a given spring joint from the world if it exists.
        /// </summary>
        /// <param name="_joint"></param>
        public void RemoveSpringJoint(ABSpringJoint _joint)
        {
            if(Springs.Contains(_joint))
            {
                Springs.Remove(_joint);
                return;
            }

            Debug.LogWarning("Attempted to remove a spring joint from world that didn't exist.");
        }

        /// <summary>
        /// Performs the physics step for the world.
        /// </summary>
        public void Step()
        {
            // Apply spring forces
            foreach (var spring in Springs)
            {
                // Hook's law
                var f = -spring.Stiffness * (spring.Distance.magnitude - spring.RestLength);
                var force = spring.Distance.normalized * f;
                spring.BodyA.AddForce(-force - spring.BodyA.LinearVelocity * spring.Dampen);
                spring.BodyB.AddForce(force - spring.BodyB.LinearVelocity * spring.Dampen);
            }

            // Apply forces to each rigid body in the world.
            foreach (var body in RigidBodies)
            {
                // Ignore infinite mass bodies, sleeping bodies, and bodies with the lock position constraints.
                if(body.InvMass == 0.0f || body.Sleeping || body.HasConstraint(Constraints.LOCK_POSITION)) continue;
                
                var acceleration = body.Force * body.InvMass;

                if(body.IgnoreGravity)
                {
                    body.LinearVelocity += acceleration * Time.fixedDeltaTime;
                }
                else
                {
                    body.LinearVelocity += (Gravity + acceleration) * Time.fixedDeltaTime;
                }

                //var angular_acceleration = body.Torque == 0.0f ? 0.0f : body.Torque / body.Inertia;
                //body.AngularVelocity = angular_acceleration / Time.fixedDeltaTime;

                // Clamp body velocity to the maximum allowed amount
                body.LinearVelocity = Vector2.ClampMagnitude(body.LinearVelocity, MaxBodyVelocity);

                body.Position += body.LinearVelocity * Time.fixedDeltaTime;

                // Clear forces
                body.Force = Vector2.zero;
                body.Torque = 0f;
            }

            // Generate collision pairs
            GeneratePairs();

            // Handle collision listeners for collisions.
            foreach (var contact in Contacts)
            {
                contact.ColliderA.CollisionListener.Handle(contact.ColliderB);
                contact.ColliderB.CollisionListener.Handle(contact.ColliderA);
            }

            // Iterative solve
            for (var i = 0; i < SolveIterations; i++)
            {
                foreach(var contact in Contacts)
                {
                    contact.ApplyImpulse();
                    contact.CorrectPosition();
                    contact.Solve();
                }
            }

            // Delete objects that are under the world (but not directly under). An object is under the world if its y position is less than 0
            for(var i = RigidBodies.Count - 1; i >= 0; i--)
            {
                var body = RigidBodies[i];
                if(body.Position.y < -10.0f)
                {
                    Object.DestroyImmediate(body.GameObject);
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
                for(var j = i + 1; j < Colliders.Count; ++j)
                {
                    // Prevent collider self check.
                    if (Colliders[i] == Colliders[j]) continue;

                    if (Colliders[i].RigidBody.InvMass == 0.0f && Colliders[j].RigidBody.InvMass == 0.0f) continue;

                    // Check distance between 2 colliders.
                    var dist = Vector2.Distance(Colliders[i].Position, Colliders[j].Position);


                    // Check collisions between each collider.
                    var colliderPair = new CollisionManifold(Colliders[i], Colliders[j]);

                    // Limit the distance to check for collisions. Hacky but for now works
                    // until I get around to TODO: spatial partitioning.
                    if (dist <= 10.0f)
                    {
                        colliderPair.Solve();

                        if (colliderPair.ContactDetected)
                        {
                            Contacts.Add(colliderPair);
                        }
                        else
                        {
                            // if no contact detected for this pair, check if the pair should trigger any collision exit callbacks.
                            colliderPair.ColliderA.CollisionListener.HandleExit(colliderPair.ColliderB);
                            colliderPair.ColliderB.CollisionListener.HandleExit(colliderPair.ColliderA);
                        }
                    }           
                }
            }
        }
    }
}
