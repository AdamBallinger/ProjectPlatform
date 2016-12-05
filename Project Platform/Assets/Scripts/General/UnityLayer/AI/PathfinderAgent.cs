using Assets.Scripts.AI.Pathfinding;
using Assets.Scripts.General.UnityLayer.Physics_Components;
using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.AI
{
    public class PathfinderAgent : MonoBehaviour
    {

        public RigidBodyComponent rigidBodyComponent;
        public BoxColliderComponent groundCheck;
        public BoxColliderComponent leftWallCheck;
        public BoxColliderComponent rightWallCheck;

        public LineRenderer pathRenderer;

        public float maxSpeed = 4.0f;
        public float movementForce = 40.0f;

        public float jumpHeight = 4.0f;

        private Path currentPath;
        private int currentPathIndex = 0;
        private bool isGrounded = true;
        private bool isCollidingLeftWall = false;
        private bool isCollidingRightWall = false;


        public void Start()
        {
            currentPath = null;

            groundCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnGroundTriggerStay);
            groundCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnGroundTriggerLeave);

            leftWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnLeftWallTriggerStay);
            leftWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnLeftWallTriggerLeave);

            rightWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnRightWallTriggerStay);
            rightWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnRightWallTriggerLeave);
        }

        public void FixedUpdate()
        {
            if (currentPath == null) return;

            // TODO: Improve the way the AI moves from point to point in the calculated path so its not so rough.
            // This current implementation is a placeholder.

            var node = currentPath.NodePath[currentPathIndex];
            var nodePos = new Vector2(node.X, node.Y);

            var distTo = Vector2.Distance(transform.position, nodePos);
            var direction = nodePos - (Vector2)transform.position;

            var moveDirX = nodePos.x < transform.position.x ? Vector2.left : Vector2.right;

            if(isCollidingLeftWall && moveDirX == Vector2.left)
                moveDirX = Vector2.zero;

            if (isCollidingRightWall && moveDirX == Vector2.right)
                moveDirX = Vector2.zero;

            if (distTo <= 0.15f)
            {
                if(currentPathIndex + 1 < currentPath.GetPathLength())
                {
                    currentPathIndex++;
                }
            }
            else
            {
                if(Mathf.Abs(transform.position.x - nodePos.x) >= 0.1f && direction.y <= 0.5f)
                {
                    // move left/right
                    rigidBodyComponent.RigidBody.AddImpulse(moveDirX * movementForce);
                }

                if(direction.y >= 0.9f && isGrounded)
                {
                    rigidBodyComponent.RigidBody.AddImpulse(Vector2.up * (jumpHeight * Mathf.Clamp01(distTo)) * rigidBodyComponent.RigidBody.Mass * 2.0f);
                }
            }

            var vel = rigidBodyComponent.RigidBody.LinearVelocity;
            vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
            rigidBodyComponent.RigidBody.LinearVelocity = vel;
        }

        public void SetPath(Path _path)
        {
            currentPath = _path;
            currentPathIndex = 0;
            transform.position = new Vector2(currentPath.StartNode.X, currentPath.StartNode.Y);

            if(pathRenderer != null)
            {
                pathRenderer.numPositions = currentPath.GetPathLength();
                foreach(var vec in currentPath.VectorPath)
                {
                    pathRenderer.SetPosition(currentPathIndex, vec);
                    currentPathIndex++;
                }

                currentPathIndex = 0;
            }
        }

        public void OnGroundTriggerStay(ABCollider _collider)
        {
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
    }
}
