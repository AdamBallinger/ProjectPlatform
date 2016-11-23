using UnityEngine;

namespace Assets.Scripts.Physics
{
    public class PhysicsMaterial
    {

        private const float defaultRestitution = 0.0f;

        private const float defaultStaticFriction = 1.0f;

        private const float defaultDynamicFriction = 0.8f;

        public float Restitution { get; set; }

        public float StaticFriction { get; set; }

        public float DynamicFriction { get; set; }

        public PhysicsMaterial()
        {
            Default();
        }

        public PhysicsMaterial(PhysicsMaterial _other)
        {
            Restitution = _other.Restitution;
            StaticFriction = _other.StaticFriction;
            DynamicFriction = _other.DynamicFriction;
        }

        /// <summary>
        /// Set this material values to their default values.
        /// </summary>
        public void Default()
        {
            Restitution = defaultRestitution;
            StaticFriction = defaultStaticFriction;
            DynamicFriction = defaultDynamicFriction;
        }
    }
}
