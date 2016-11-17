using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    [RequireComponent(typeof(RigidBodyComponent))]
    public class CircleColliderComponent : MonoBehaviour
    {

        public ABCircleCollider Collider { get; private set; }

        [SerializeField]
        private float radius = 1.0f;

        [SerializeField]
        private bool inspectorCreated = false;

        public void Start()
        {
            if(inspectorCreated)
            {
                Create(radius);
            }
        }

        public void OnDestroy()
        {
            ClearCollider();
        }

        /// <summary>
        /// Creates a new circle collider instance for this component with the given radius. Also destroys the existing (if one exists) from the world.
        /// </summary>
        /// <param name="_radius"></param>
        public void Create(float _radius)
        {
            if(Collider != null)
                ClearCollider();

            Collider = new ABCircleCollider(GetComponent<RigidBodyComponent>().RigidBody);
            Collider.Radius = _radius;
        }

        private void ClearCollider()
        {
            World.Current.PhysicsWorld.RemoveCollider(Collider);
        }

        public void OnDrawGizmos()
        {
            if (!inspectorCreated)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
