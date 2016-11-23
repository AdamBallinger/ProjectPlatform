using Assets.Scripts.Physics.Colliders;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class PlayerController : MonoBehaviour
    {

        public BoxColliderComponent groundCheck;
        public BoxColliderComponent leftWallCheck;
        public BoxColliderComponent rightWallCheck;

        public float maxSpeed = 5f;
        public float force = 50.0f;
        public float jumpHeight = 5.0f;

        private bool isGrounded = false;
        private bool isCollidingLeftWall = false;
        private bool isCollidingRightWall = false;

        private RigidBodyComponent rigidBodyComponent;

        public void Start()
        {
            rigidBodyComponent = GetComponent<RigidBodyComponent>();

            if(groundCheck != null)
            {
                groundCheck.Collider.CollisionListener.RegisterTriggerEnterCallback(OnGroundTriggerEnter);
                groundCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnGroundTriggerLeave);
            }

            if (leftWallCheck != null)
            {
                leftWallCheck.Collider.CollisionListener.RegisterTriggerEnterCallback(OnLeftWallTriggerEnter);
                leftWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnLeftWallTriggerLeave);
            }

            if (rightWallCheck != null)
            {
                rightWallCheck.Collider.CollisionListener.RegisterTriggerEnterCallback(OnRightWallTriggerEnter);
                rightWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnRightWallTriggerLeave);
            }
        }

        public void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.up * jumpHeight * (rigidBodyComponent.RigidBody.Mass * 2));
            }

            if(Input.GetKey(KeyCode.A) && !isCollidingLeftWall)
            {
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.left * force);              
            }

            if(Input.GetKey(KeyCode.D) && !isCollidingRightWall)
            {
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.right * force);
            }

            var vel = rigidBodyComponent.RigidBody.LinearVelocity;
            vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
            rigidBodyComponent.RigidBody.LinearVelocity = vel;
        }

        public void OnGroundTriggerEnter(ABCollider _collider)
        {
            isGrounded = true;
        }

        public void OnGroundTriggerLeave(ABCollider _collider)
        {
            isGrounded = false;
        }

        public void OnLeftWallTriggerEnter(ABCollider _collider)
        {
            isCollidingLeftWall = true;
        }

        public void OnLeftWallTriggerLeave(ABCollider _collider)
        {
            // only allow left movment again if grounded (because triggers enter and leave as the object falls).
            if(isGrounded)
                isCollidingLeftWall = false;
        }

        public void OnRightWallTriggerEnter(ABCollider _collider)
        {
            isCollidingRightWall = true;
        }

        public void OnRightWallTriggerLeave(ABCollider _collider)
        {
            // only allow right movment again if grounded (because triggers enter and leave as the object falls).
            if (isGrounded)
                isCollidingRightWall = false;
        }
    }
}
