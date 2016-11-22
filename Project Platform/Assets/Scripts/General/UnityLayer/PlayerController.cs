using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class PlayerController : MonoBehaviour
    {

        public BoxColliderComponent groundCheck;
        public float force = 50.0f;

        private RigidBodyComponent rigidBodyComponent;

        public void Start()
        {
            rigidBodyComponent = GetComponent<RigidBodyComponent>();

            if(groundCheck != null)
            {
                groundCheck.Collider.CollisionListener.RegisterTriggerEnterCallback(OnABTriggerEnter);
                groundCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnABTriggerStay);
                groundCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnABTriggerLeave);
                groundCheck.Collider.CollisionListener.RegisterOnCollisionCallback(OnCollision);
            }
        }

        public void FixedUpdate()
        {
            if(Input.GetKey(KeyCode.Space))
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

        public void OnABTriggerEnter(ABCollider _collider)
        {
            //Debug.Log(_collider.RigidBody.GameObject.name + " entered " + gameObject.name);
        }

        public void OnABTriggerStay(ABCollider _collider)
        {
            //Debug.Log(_collider.RigidBody.GameObject.name + " stayed inside of " + gameObject.name);
        }

        public void OnABTriggerLeave(ABCollider _collider)
        {
            //Debug.Log(_collider.RigidBody.GameObject.name + " exited " + gameObject.name);
        }

        public void OnCollision(ABCollider _collider)
        {
            //Debug.Log(gameObject.name + " collided with: " + _collider.RigidBody.GameObject.name);
        }
    }
}
