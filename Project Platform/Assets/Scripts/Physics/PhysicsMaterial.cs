using UnityEngine;

namespace Assets.Scripts.Physics
{
    public class PhysicsMaterial
    {

        public float Restitution { get; set; }

        public float StaticFriction { get; set; }

        public float DynamicFriction { get; set; }

        public PhysicsMaterial()
        {
            Restitution = 0.0f;
            StaticFriction = 1.0f;
            DynamicFriction = 0.8f;
        }

    }
}
