using Assets.Scripts.Physics;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    [DisallowMultipleComponent]
    public class PhysicsMaterialComponent : MonoBehaviour
    {
        private PhysicsMaterial physicsMaterial;

        [SerializeField]
        private float bounce = 0.0f;

        [SerializeField]
        private float dynamicFriction = 0.5f;

        [SerializeField]
        private float staticFriction = 0.5f;

        public void Create()
        {
            //PhysicsMaterial = new PhysicsMaterial();
            //PhysicsMaterial.Restitution = bounce;
            //PhysicsMaterial.StaticFriction = staticFriction;
            //PhysicsMaterial.DynamicFriction = dynamicFriction;
        }
    }
}
