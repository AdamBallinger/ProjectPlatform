

using Assets.Scripts.General.UnityLayer;
using UnityEngine;

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

            // Apply impulse resolution
            var relativeVelocity = ColliderB.RigidBody.LinearVelocity - ColliderA.RigidBody.LinearVelocity;

            var velAlongNormal = Vector2.Dot(relativeVelocity, Normal);

            if (velAlongNormal > 0)
                return;

            var e = 0.2f;
            var j = -(1.0f + e) * velAlongNormal;
            j /= ColliderA.RigidBody.InvMass + ColliderB.RigidBody.InvMass;

            var impulse = j * Normal;
            ColliderA.RigidBody.AddImpulse(-impulse);
            ColliderB.RigidBody.AddImpulse(impulse);
        }

        public void CorrectPosition()
        {
            var slop = 0.05f; // Penetration allowance
            var percent = 0.2f; // Penetration percentage to correct
            var correctionVector = (Penetration - slop / (ColliderA.RigidBody.InvMass + ColliderB.RigidBody.InvMass)) * percent * Normal;
            ColliderA.RigidBody.AddImpulse(correctionVector);
            ColliderB.RigidBody.AddImpulse(-correctionVector);
        }

        private void CorrectInfiniteMass()
        {
            ColliderA.RigidBody.LinearVelocity = Vector2.zero;
            ColliderB.RigidBody.LinearVelocity = Vector2.zero;
        }

        private void AABB_AABB(ABBoxCollider _aabb1, ABBoxCollider _aabb2)
        {
            if(_aabb1.Min.x < _aabb2.Max.x &&
                _aabb1.Max.x > _aabb2.Min.x &&
                _aabb1.Min.y < _aabb2.Min.y + _aabb2.Size.y &&
                _aabb1.Min.y + _aabb1.Size.y > _aabb2.Min.y)
            {
                // Collision occured between colliders.
                ContactDetected = true;

                // Distance between the 2 colliders.
                var dist = _aabb1.RigidBody.GameObject.transform.position - _aabb2.RigidBody.GameObject.transform.position;

                // Find penetration distances for each axis.
                var xPenetration = (_aabb1.Max.x - _aabb1.Min.x) / 2 + (_aabb2.Max.x - _aabb2.Min.x) / 2 - Mathf.Abs(dist.x);
                var yPenetration = (_aabb1.Max.y - _aabb1.Min.y) / 2 + (_aabb2.Max.y - _aabb2.Min.y) / 2 - Mathf.Abs(dist.y);

                // If the X axis penetrates less than the Y axis, then the normal is either (-1,0)(left) or (1,0)(right) face of the AABB.
                if (xPenetration < yPenetration)
                {
                    // Check which face collided with on x axis based on dist x.
                    Normal = dist.x < 0 ? Vector2.left : Vector2.right;
                    Penetration = xPenetration;
                }
                else
                {
                    // Check which face collided with on y axis based on dist y.
                    Normal = dist.y < 0 ? Vector2.up : Vector2.down;
                    Penetration = yPenetration;
                }
            }         
        }
    }
}
