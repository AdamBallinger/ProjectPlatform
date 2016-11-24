using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.Physics_Components
{
    public class BoxColliderComponent : MonoBehaviour
    {

        public ABBoxCollider Collider { get; set; }

        [SerializeField]
        private Vector2 Size = Vector2.one;

        [SerializeField]
        private Vector2 offset = Vector2.zero;

        [SerializeField]
        private bool inspectorCreated = false;

        [SerializeField]
        private bool isTrigger = false;

        [SerializeField]
        private bool drawGizmo = true;

        public void Start()
        {
            if(inspectorCreated)
            {
                Create(Size);
            }
        }

        public void Create(Vector2 _size)
        {
            var rigidBodyComponent = GetComponent<RigidBodyComponent>() ?? transform.root.gameObject.GetComponent<RigidBodyComponent>();

            if(rigidBodyComponent == null)
            {
                Debug.LogError("You can't create a box collider component on a gameobject without a rigidbody at its root.");
                return;
            }

            if(Collider != null)
                ClearCollider();

            Collider = new ABBoxCollider(rigidBodyComponent.RigidBody);
            Collider.Offset = offset;
            Collider.Size = _size;
            Collider.IsTrigger = isTrigger;
        }
        
        // When unity destroys this object, make sure if the game isnt being closed, then the collider is removed from the physics world
        public void OnDestroy()
        {
            ClearCollider();
        }
        
        /// <summary>
        /// Removes the collider for this component from the world.
        /// </summary>
        public void ClearCollider()
        {
            if (Collider == null) return;

            World.Current.PhysicsWorld.RemoveCollider(Collider);
            Collider = null;
        }    

        public void OnDrawGizmos()
        {
            if (!inspectorCreated || !drawGizmo)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(gameObject.transform.position + (Vector3)offset, Size);
        }
    }
}
