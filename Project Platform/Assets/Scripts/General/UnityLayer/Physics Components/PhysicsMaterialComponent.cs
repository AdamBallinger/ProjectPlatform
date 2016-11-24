using Assets.Scripts.Physics;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.Physics_Components
{
    [DisallowMultipleComponent]
    public class PhysicsMaterialComponent : MonoBehaviour
    {
        private PhysicsMaterial material;

        [SerializeField]
        private float bounce = 0.5f;

        [SerializeField]
        private float dynamicFriction = 0.5f;

        [SerializeField]
        private float staticFriction = 0.5f;

        public void Start()
        {
            Create();
        }

        public void Create()
        {
            material = GetComponent<RigidBodyComponent>().RigidBody.Material;
            material.Restitution = bounce;
            material.StaticFriction = staticFriction;
            material.DynamicFriction = dynamicFriction;
        }

        public void SetBounce(float _bounce)
        {
            bounce = _bounce;
            material.Restitution = bounce;
        }

        public void SetStaticFriction(float _static)
        {
            staticFriction = _static;
            material.StaticFriction = staticFriction;
        }

        public void SetDynamicFriction(float _dynamic)
        {
            dynamicFriction = _dynamic;
            material.DynamicFriction = dynamicFriction;
        }
    }
}
