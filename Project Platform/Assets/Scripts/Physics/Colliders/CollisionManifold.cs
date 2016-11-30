using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Physics.Colliders
{
    public class CollisionManifold
    {

        public ABCollider ColliderA { get; private set; }
        public ABCollider ColliderB { get; private set; }

        public bool ContactDetected { get; private set; }

        /// <summary>
        /// Collision normal vector.
        /// </summary>
        public Vector2 Normal { get; private set; }

        /// <summary>
        /// Collision penetration distance.
        /// </summary>
        public float Penetration { get; private set; }

        /// <summary>
        /// Returns the sum of bother colliders inverse mass.
        /// </summary>
        public float InvMassSum
        {
            get { return ColliderA.RigidBody.InvMass + ColliderB.RigidBody.InvMass; }
        }


        public CollisionManifold(ABCollider _a, ABCollider _b)
        {
            ContactDetected = false;
            ColliderA = _a;
            ColliderB = _b;
            Normal = Vector2.zero;
            Penetration = 0.0f;
        }

        /// <summary>
        /// Solve the contact info for this given collision pair.
        /// </summary>
        public void Solve()
        {
            var typeA = ColliderA.GetType();
            var typeB = ColliderB.GetType();

            if (typeA == typeof(ABBoxCollider) && typeB == typeof(ABBoxCollider))
            {
                AABB_AABB((ABBoxCollider)ColliderA, (ABBoxCollider)ColliderB);
            }
            else if (typeA == typeof(ABCircleCollider) && typeB == typeof(ABCircleCollider))
            {
                Circle_Circle((ABCircleCollider)ColliderA, (ABCircleCollider)ColliderB);
            }
            else if (typeA == typeof(ABCircleCollider) && typeB == typeof(ABBoxCollider))
            {
                Circle_AABB((ABCircleCollider)ColliderA, (ABBoxCollider)ColliderB);
            }
            else if (typeA == typeof(ABBoxCollider) && typeB == typeof(ABCircleCollider))
            {
                AABB_Circle((ABBoxCollider)ColliderA, (ABCircleCollider)ColliderB);
            }
        }

        /// <summary>
        /// Apply the collision impulse to seperate two colliding bodies.
        /// </summary>
        public void ApplyImpulse()
        {
            // If both bodies have infinite mass (0) then just clear their velocity and break out.
            if (InvMassSum == 0.0f)
            {
                CorrectInfiniteMass();
                return;
            }

            // Dont resolve a trigger collision.
            if(ColliderA.IsTrigger || ColliderB.IsTrigger)
            {
                return;
            }

            var relativeVelocity = ColliderB.RigidBody.LinearVelocity - ColliderA.RigidBody.LinearVelocity;
            var velocityAlongNormal = Vector2.Dot(relativeVelocity, Normal);

            if (velocityAlongNormal > 0)
            {
                // The 2 bodies are already seperating.
                return;
            }

            // Restitution  0.0f - Inelastic  1.0f - full elastic
            var e = 0f;

            if(relativeVelocity.sqrMagnitude < (Time.fixedDeltaTime * World.Current.PhysicsWorld.Gravity).sqrMagnitude + Mathf.Epsilon)
            {
                // resting collision if only gravity is effecting the bodies.
                e = 0.0f;
            }

            var j = -(1.0f + e) * velocityAlongNormal; // Impulse magnitude
            j /= InvMassSum;

            var impulse = j * Normal;

            // Apply collision impulse
            ColliderA.RigidBody.AddImpulse(-impulse);
            ColliderB.RigidBody.AddImpulse(impulse);

            // Calculate and apply friction impulse. (Coulomb's Law)

            // recalculate relative velocity
            relativeVelocity = ColliderB.RigidBody.LinearVelocity - ColliderA.RigidBody.LinearVelocity;

            var tangent = relativeVelocity - Vector2.Dot(relativeVelocity, Normal) * Normal;
            tangent.Normalize();

            var jt = -Vector2.Dot(relativeVelocity, tangent);
            jt /= InvMassSum;

            var staticFriction = Mathf.Sqrt(ColliderA.RigidBody.Material.StaticFriction * ColliderB.RigidBody.Material.StaticFriction);
            var dynamicFriction = Mathf.Sqrt(ColliderA.RigidBody.Material.DynamicFriction * ColliderB.RigidBody.Material.DynamicFriction);

            Vector2 tangentImpulse;

            if(Mathf.Abs(jt) < j * staticFriction)
            {
                tangentImpulse = tangent * jt;
            }
            else
            {
                tangentImpulse = tangent * -j * dynamicFriction;
            }

            ColliderA.RigidBody.AddImpulse(-tangentImpulse);
            ColliderB.RigidBody.AddImpulse(tangentImpulse);
        }

        /// <summary>
        /// Correct body position to prevent bodies from sinking into eachother due to gravity.
        /// </summary>
        public void CorrectPosition()
        {
            if (InvMassSum == 0.0f)
            {
                return;
            }

            if(ColliderA.IsTrigger || ColliderB.IsTrigger)
            {
                return;
            }

            var pentrationAllowance = 0.01f;
            var penetrationCorrection = 0.6f; // % correction

            var correctionVector = Mathf.Max(Penetration - pentrationAllowance, 0.0f) / InvMassSum * Normal * penetrationCorrection;
            var correctionA = correctionVector;
            var correctionB = correctionVector;

            if(ColliderA.RigidBody.HasConstraint(Constraints.LOCK_POSITION_X))
            {
                correctionA.x = 0.0f;
            }

            if(ColliderA.RigidBody.HasConstraint(Constraints.LOCK_POSITION_Y))
            {
                correctionA.y = 0.0f;
            }

            if (ColliderB.RigidBody.HasConstraint(Constraints.LOCK_POSITION_X))
            {
                correctionB.x = 0.0f;
            }

            if (ColliderB.RigidBody.HasConstraint(Constraints.LOCK_POSITION_Y))
            {
                correctionB.y = 0.0f;
            }

            ColliderA.RigidBody.Position -= correctionA * ColliderA.RigidBody.InvMass;
            ColliderB.RigidBody.Position += correctionB * ColliderB.RigidBody.InvMass;
        }

        /// <summary>
        /// Ensure any infinite mass bodies remain at 0 velocity.
        /// </summary>
        private void CorrectInfiniteMass()
        {
            ColliderA.RigidBody.LinearVelocity = Vector2.zero;
            ColliderB.RigidBody.LinearVelocity = Vector2.zero;
        }

        /// <summary>
        /// Check and create collison info for two given AABB colliders.
        /// </summary>
        /// <param name="_aabb1"></param>
        /// <param name="_aabb2"></param>
        private void AABB_AABB(ABBoxCollider _aabb1, ABBoxCollider _aabb2)
        {
            _aabb1.ComputeAABB();
            _aabb2.ComputeAABB();

            if (_aabb1.Min.y > _aabb2.Max.y &&
                _aabb1.Max.y < _aabb2.Min.y &&
                _aabb1.Min.x < _aabb2.Max.x &&
                _aabb1.Max.x > _aabb2.Min.x)
            {
                // Collision occured between colliders.
                ContactDetected = true;

                // Difference between the 2 colliders.
                var difference = _aabb1.Position - _aabb2.Position;

                var xPenetration = _aabb1.Size.x / 2 + _aabb2.Size.x / 2 - Mathf.Abs(difference.x);

                if (xPenetration > 0.0f)
                {
                    var yPenetration = _aabb1.Size.y / 2 + _aabb2.Size.y / 2 - Mathf.Abs(difference.y);

                    if (yPenetration > 0.0f)
                    {
                        if (xPenetration < yPenetration)
                        {
                            Normal = difference.x < 0 ? Vector2.right : Vector2.left;
                            Penetration = xPenetration;
                        }
                        else
                        {
                            Normal = difference.y <= 0 ? Vector2.up : Vector2.down;
                            Penetration = yPenetration;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check and create collision info for two given circle colliders.
        /// </summary>
        /// <param name="_circle1"></param>
        /// <param name="_circle2"></param>
        private void Circle_Circle(ABCircleCollider _circle1, ABCircleCollider _circle2)
        {
            var direction = _circle2.Position - _circle1.Position;
            var radiusSum = _circle1.Radius + _circle2.Radius;

            if (direction.sqrMagnitude > radiusSum * radiusSum)
            {
                // No contact
                ContactDetected = false;
                return;
            }

            var dist = direction.magnitude;

            // Circles are right on top of eachother
            if(dist == 0.0f)
            {
                ContactDetected = true;
                Penetration = _circle1.Radius;
                Normal = Vector2.up;
            }
            else
            {
                ContactDetected = true;
                Penetration = radiusSum - dist;
                Normal = direction / dist;
            }
        }

        /// <summary>
        /// Check and create collision info between an AABB and circle collider.
        /// </summary>
        /// <param name="_aabb"></param>
        /// <param name="_circle"></param>
        private void AABB_Circle(ABBoxCollider _aabb, ABCircleCollider _circle)
        {
            _aabb.ComputeAABB();

            var xNear = Mathf.Max(_aabb.Min.x, Mathf.Min(_circle.Position.x, _aabb.Max.x));
            var yNear = Mathf.Max(_aabb.Max.y, Mathf.Min(_circle.Position.y, _aabb.Min.y));

            var closesPointToRect = new Vector2(xNear, yNear);

            var normal = _circle.Position - closesPointToRect;

            if (normal.sqrMagnitude < _circle.Radius * _circle.Radius)
            {
                // Collision between the circle and aabb.
                ContactDetected = true;
                Penetration = _circle.Radius - normal.magnitude;
                Normal = normal;
            }    
        }

        /// <summary>
        /// Check and create collision info between a circle and AABB collider.
        /// </summary>
        /// <param name="_circle"></param>
        /// <param name="_aabb"></param>
        private void Circle_AABB(ABCircleCollider _circle, ABBoxCollider _aabb)
        {
            AABB_Circle(_aabb, _circle);

            // because pair order gets flipped here, the normal also needs to be flipped.
            Normal = -Normal;
        }
    }
}
