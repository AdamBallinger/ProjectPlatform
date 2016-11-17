using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class PlayerController : MonoBehaviour
    {

        public float force = 1.0f;

        private RigidBodyComponent rigidBodyComponent;

        public void Start()
        {
            rigidBodyComponent = GetComponent<RigidBodyComponent>();
        }

        public void FixedUpdate()
        {
            if(Input.GetKey(KeyCode.Space) && rigidBodyComponent.RigidBody.IsColliding)
            {
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.up * force);
            }

            if(Input.GetKey(KeyCode.A))
            {
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.left * force);
            }

            if(Input.GetKey(KeyCode.D))
            {
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.right * force);
            }
        }
    }
}
