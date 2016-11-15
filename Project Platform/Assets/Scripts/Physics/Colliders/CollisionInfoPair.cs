

using Assets.Scripts.General.UnityLayer;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.Physics.Colliders
{
    public class CollisionInfoPair
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


        public CollisionInfoPair(ABCollider _a, ABCollider _b)
        {
            ContactDetected = false;
            ColliderA = _a;
            ColliderB = _b;
            Normal = Vector2.zero;
            Penetration = 0.0f;
        }

        public void Solve()
        {
            if(ColliderA.GetType() == typeof(ABBoxCollider) && ColliderB.GetType() == typeof(ABBoxCollider))
            {
                AABB_AABB((ABBoxCollider)ColliderA, (ABBoxCollider)ColliderB);
            }
        }

        public void ApplyImpulse()
        {
            // If both bodies have infinite mass (0) then just clear their velocity and break out.
            if(ColliderA.RigidBody.Mass + ColliderB.RigidBody.Mass <= 0.0f)
            {
                CorrectInfiniteMass();
                return;
            }

            var relativeVelocity = ColliderB.RigidBody.LinearVelocity - ColliderA.RigidBody.LinearVelocity;
            var velocityAlongNormal = Vector2.Dot(relativeVelocity, Normal);

            if (velocityAlongNormal > 0)
            {
                // The 2 bodies are already seperating.
                return;
            }

            var e = 0f; // Restitution  0.0f - Inelastic  1.0f - full elastic
            var massSum = ColliderA.RigidBody.InvMass + ColliderB.RigidBody.InvMass;
            var j = -(1.0f + e) * velocityAlongNormal; // Impulse magnitude
            j /= massSum;

            var impulse = j * Normal;

            ColliderA.RigidBody.LinearVelocity -= ColliderA.RigidBody.InvMass * impulse;
            ColliderB.RigidBody.LinearVelocity += ColliderB.RigidBody.InvMass * impulse;

            // TODO: Friction impulse

        }

        public void CorrectPosition()
        {
            var slop = 0.0001f; // Penetration allowance
            var percent = 1f; // Penetration percentage to correct
            var correctionVector = (Mathf.Max(Penetration - slop, 0.0f) / (ColliderA.RigidBody.InvMass + ColliderB.RigidBody.InvMass)) * Normal * percent;
            ColliderA.RigidBody.Position -= correctionVector * ColliderA.RigidBody.InvMass;
            ColliderB.RigidBody.Position += correctionVector * ColliderB.RigidBody.InvMass;
        }

        private void CorrectInfiniteMass()
        {
            ColliderA.RigidBody.LinearVelocity = Vector2.zero;
            ColliderB.RigidBody.LinearVelocity = Vector2.zero;
        }

        private void AABB_AABB(ABBoxCollider _aabb1, ABBoxCollider _aabb2)
        {
            _aabb1.ComputeAABB();
            _aabb2.ComputeAABB();

            if( _aabb1.Min.y > _aabb2.Max.y &&
                _aabb1.Max.y < _aabb2.Min.y &&
                _aabb1.Min.x < _aabb2.Max.x &&
                _aabb1.Max.x > _aabb2.Min.x)
            {
                // Collision occured between colliders.
                ContactDetected = true;

                // Direction between the 2 colliders.
                var direction = _aabb1.RigidBody.GameObject.transform.position - _aabb2.RigidBody.GameObject.transform.position;

                var xPenetration = _aabb1.Size.x / 2 + _aabb2.Size.x / 2 - Mathf.Abs(direction.x);

                if(xPenetration > 0)
                {
                    var yPenetration = _aabb1.Size.y / 2 + _aabb2.Size.y / 2 - Mathf.Abs(direction.y);

                    if(yPenetration > 0)
                    {
                        if(xPenetration < yPenetration)
                        {
                            Normal = direction.x < 0 ? Vector2.right : Vector2.left;
                            Penetration = xPenetration;
                        }
                        else
                        {
                            Normal = direction.y < 0 ? Vector2.up : Vector2.down;
                            Penetration = yPenetration;
                        }
                    }
                }
            }         
        }
    }
}
