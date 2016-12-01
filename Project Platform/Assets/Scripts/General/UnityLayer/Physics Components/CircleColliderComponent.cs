using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.Physics_Components
{
    public class CircleColliderComponent : MonoBehaviour
    {

        public ABCircleCollider Collider { get; private set; }

        [SerializeField]
        private float radius = 1.0f;

        [SerializeField]
        private Vector2 offset = Vector2.zero;

        [SerializeField]
        private bool inspectorCreated = false;

        [SerializeField]
        private bool isTrigger = false;

        [SerializeField]
        private bool drawGizmo = false;

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
            var rigidBodyComponent = GetComponent<RigidBodyComponent>() ?? transform.root.gameObject.GetComponent<RigidBodyComponent>();

            if (rigidBodyComponent == null)
            {
                Debug.LogError("You can't create a box collider component on a gameobject without a rigidbody at its root.");
                return;
            }

            if (Collider != null)
                ClearCollider();

            Collider = new ABCircleCollider(rigidBodyComponent.RigidBody);
            Collider.Radius = _radius;
            Collider.Offset = offset;
            Collider.IsTrigger = isTrigger;
        }

        private void ClearCollider()
        {
            World.Current.PhysicsWorld.RemoveCollider(Collider);
        }

        public void OnDrawGizmos()
        {
            if (!inspectorCreated || !drawGizmo)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + (Vector3)offset, radius);
        }
    }
}
