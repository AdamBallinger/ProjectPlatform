using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class BoxColliderComponent : MonoBehaviour
    {

        public ABBoxCollider Collider { get; set; }

        [SerializeField]
        private Vector2 Size = Vector2.one;

        [SerializeField]
        private bool inspectorCreated = false;

        public void Start()
        {
            if(inspectorCreated)
                Create(Size);
        }

        public void Create(Vector2 _size)
        {
            ClearCollider();
            Collider = new ABBoxCollider(GetComponent<RigidBodyComponent>().RigidBody);
            Collider.Size = _size;
        }
        
        // When unity destroys this object, make sure if the game isnt being closed, then the collider is removed from the physics world
        public void Destroy()
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
            if (!inspectorCreated) return;
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(gameObject.transform.position, Size);
        }
    }
}
