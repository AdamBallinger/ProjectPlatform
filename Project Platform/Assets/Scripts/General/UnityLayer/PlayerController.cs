using Assets.Scripts.General.UnityLayer.Physics_Components;
using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class PlayerController : MonoBehaviour
    {

        public BoxColliderComponent groundCheck;
        public BoxColliderComponent leftWallCheck;
        public BoxColliderComponent rightWallCheck;

        public BoxColliderComponent playerBody;

        public float maxSpeed = 5f;
        public float force = 50.0f;

        public int jumpHeight = 5;

        public bool isGrounded = false;
        public bool isCollidingLeftWall = false;
        public bool isCollidingRightWall = false;

        private RigidBodyComponent rigidBodyComponent;

        public void Start()
        {
            rigidBodyComponent = GetComponent<RigidBodyComponent>();

            if(rigidBodyComponent.RigidBody == null)
            {
                rigidBodyComponent.Create();
            }

            if(groundCheck != null)
            {
                groundCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnGroundTriggerStay);
                groundCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnGroundTriggerLeave);
            }

            if (leftWallCheck != null)
            {
                leftWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnLeftWallTriggerStay);
                leftWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnLeftWallTriggerLeave);
            }

            if (rightWallCheck != null)
            {
                rightWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnRightWallTriggerStay);
                rightWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnRightWallTriggerLeave);
            }

            if(playerBody != null)
            {
                playerBody.Collider.CollisionListener.RegisterTriggerEnterCallback(OnBodyTriggerEnter);
            }
        }

        public void FixedUpdate()
        {
            if(rigidBodyComponent.RigidBody == null)
            {
                return;
            }

            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                // add 0.1 to jump height as a slight offset since the player isn't the size of a full tile.
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.up * jumpHeight * rigidBodyComponent.RigidBody.Mass * 2);
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

        public void OnGroundTriggerStay(ABCollider _collider)
        {
            // Set grounded to true during stay as if the trigger enters a new tile, and leaves the previous after, grounded will be false 
            // even though it is actually grounded.
            isGrounded = true;
        }

        public void OnGroundTriggerLeave(ABCollider _collider)
        {
            isGrounded = false;
        }

        public void OnLeftWallTriggerStay(ABCollider _collider)
        {
            isCollidingLeftWall = true;
        }

        public void OnLeftWallTriggerLeave(ABCollider _collider)
        {
            isCollidingLeftWall = false;
        }

        public void OnRightWallTriggerStay(ABCollider _collider)
        {
            isCollidingRightWall = true;
        }

        public void OnRightWallTriggerLeave(ABCollider _collider)
        {
            isCollidingRightWall = false;
        }

        public void OnBodyTriggerEnter(ABCollider _collider)
        {
            if(_collider.RigidBody.GameObject.tag == "Coin")
            {
                // Destroy coin object and add to player score.
                Debug.Log("Collided with coin.");
                Destroy(_collider.RigidBody.GameObject);
            }
        }
    }
}
