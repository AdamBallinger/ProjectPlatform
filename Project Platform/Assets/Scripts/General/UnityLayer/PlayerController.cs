using Assets.Scripts.General.UnityLayer.Physics_Components;
using Assets.Scripts.Physics.Colliders;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts.General.UnityLayer
{
    public class PlayerController : MonoBehaviour
    {

        public BoxColliderComponent groundCheck;
        public BoxColliderComponent leftWallCheck;
        public BoxColliderComponent rightWallCheck;

        public BoxColliderComponent playerBody;

        public int score = 0;

        public float maxSpeed = 5f;
        public float force = 50.0f;

        public int jumpHeight = 5;

        // Control whether or not player jumping should handle 
        public bool enableJumpSensitivity = true;
        public float jumpPower = 0.0f;
        public float jumpPowerIncrement = 1f;

        public bool isGrounded = false;
        public bool isCollidingLeftWall = false;
        public bool isCollidingRightWall = false;

        private RigidBodyComponent rigidBodyComponent;

        public void Start()
        {
            rigidBodyComponent = GetComponent<RigidBodyComponent>();

            if (rigidBodyComponent.RigidBody == null)
            {
                rigidBodyComponent.Create();
            }

            if (groundCheck != null)
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

            if (playerBody != null)
            {
                playerBody.Collider.CollisionListener.RegisterOnCollisionCallback(OnBodyCollision);
            }
        }

        public void FixedUpdate()
        {
            if (rigidBodyComponent.RigidBody == null)
            {
                return;
            }

            if(Input.GetKey(KeyCode.Space) && isGrounded)
            {
                jumpPower += jumpPowerIncrement;
                jumpPower = Mathf.Clamp01(jumpPower);
            }

            //if ((Input.GetKeyUp(KeyCode.Space) || jumpPower == 1.0f) && isGrounded)
            if((enableJumpSensitivity && (Input.GetKeyUp(KeyCode.Space) || jumpPower >= 1.0f) && isGrounded) 
                || (!enableJumpSensitivity && Input.GetKeyDown(KeyCode.Space) && isGrounded))
            {
                if(!enableJumpSensitivity)
                {
                    jumpPower = 1.0f;
                }

                rigidBodyComponent.RigidBody.AddImpulse(Vector2.up * (jumpHeight * jumpPower) * rigidBodyComponent.RigidBody.Mass * 2);
                jumpPower = 0.0f;
            }

            if (Input.GetKey(KeyCode.A) && !isCollidingLeftWall)
            {
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.left * force);
            }

            if (Input.GetKey(KeyCode.D) && !isCollidingRightWall)
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
            if (_collider.RigidBody.GameObject.tag == "Tile")
            {
                isCollidingLeftWall = true;
            }
        }

        public void OnLeftWallTriggerLeave(ABCollider _collider)
        {
            isCollidingLeftWall = false;
        }

        public void OnRightWallTriggerStay(ABCollider _collider)
        {
            if (_collider.RigidBody.GameObject.tag == "Tile")
            {
                isCollidingRightWall = true;
            }
        }

        public void OnRightWallTriggerLeave(ABCollider _collider)
        {
            isCollidingRightWall = false;
        }

        public void OnBodyCollision(ABCollider _collider)
        {
            if (_collider.RigidBody.GameObject.tag == "Coin")
            {
                var pos = World.Current.WorldPointToGridPoint(_collider.Position);
                FindObjectOfType<WorldController>().RemoveCoinPickup(pos);
                score++;
            }
        }
    }
}
