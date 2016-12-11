using System.Collections;
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

        private GameObject waypointObject;

        public float maxSpeed = 4.0f;
        public float movementForce = 40.0f;

        public float jumpHeight = 4.0f;

        public PathFinder pathFinder;
        public Path currentPath;

        private int currentPathIndex = 0;
        public bool isGrounded = true;
        public bool isCollidingLeftWall = false;
        public bool isCollidingRightWall = false;

        // Update path n timers per second.
        public float pathUpdateRate = 5.0f;

        public void Start()
        {
            pathFinder = new PathFinder(OnPathComplete);
            ClearPath();

            groundCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnGroundTriggerStay);
            groundCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnGroundTriggerLeave);

            leftWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnLeftWallTriggerStay);
            leftWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnLeftWallTriggerLeave);

            rightWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnRightWallTriggerStay);
            rightWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnRightWallTriggerLeave);

            waypointObject = GameObject.FindGameObjectWithTag("AIWaypoint");
        }

        public void StartPathing(Vector2 _start, Vector2 _end)
        {
            ClearPath();
            StopAllCoroutines();

            pathFinder.FindPath(_start, _end);

            StartCoroutine(UpdatePath());
        }

        /// <summary>
        /// Callback for when the pathfinder has done finding a path.
        /// </summary>
        /// <param name="_path"></param>
        public void OnPathComplete(Path _path)
        {
            if (_path.Valid)
            {
                currentPath = _path;
                currentPathIndex = 0;
                //Debug.Log("Created path in " + currentPath.CreationTime + " ms");
            }
        }

        /// <summary>
        /// Coroutine for updating the agents path n(the value of pathUpdateRate) times per second.
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdatePath()
        {
            if (currentPath != null)
            {
                pathFinder.FindPath(rigidBodyComponent.RigidBody.Position, new Vector2(currentPath.EndNode.X, currentPath.EndNode.Y));
            }

            yield return new WaitForSeconds(1.0f / pathUpdateRate);

            StartCoroutine(UpdatePath());
        }

        /// <summary>
        /// Clear the agents current path.
        /// </summary>
        public void ClearPath()
        {
            if (currentPath != null)
            {
                StopAllCoroutines();
                currentPath = null;
                currentPathIndex = 0;

                if (pathRenderer != null)
                {
                    pathRenderer.numPositions = 0;

                    if (waypointObject != null)
                    {
                        // Move waypoint off the screen when path is done.
                        var waypointPos = new Vector2(-100.0f, 0.0f);
                        waypointObject.transform.position = waypointPos;
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            if (currentPath == null) return;

            // If the path index is 0, a new path must have been created so reset the path renderer to the new path.
            // This can't be placed in OnPathComplete as pathing is threaded and Unity API isn't thread-safe.
            if (currentPathIndex == 0)
            {
                if (pathRenderer != null)
                {
                    pathRenderer.numPositions = currentPath.GetPathLength();
                    foreach (var vec in currentPath.VectorPath)
                    {
                        pathRenderer.SetPosition(currentPathIndex, vec);
                        currentPathIndex++;
                    }

                    currentPathIndex = 1;
                }
            }

            if (currentPathIndex >= currentPath.GetPathLength())
            {
                ClearPath();
                rigidBodyComponent.RigidBody.LinearVelocity = Vector2.zero;
                return;
            }

            var node = currentPath.NodePath[currentPathIndex];
            var nodePos = new Vector2(node.X, node.Y);

            var distTo = Vector2.Distance(transform.position, nodePos);
            var direction = nodePos - (Vector2)transform.position;

            var moveDirX = nodePos.x < transform.position.x ? Vector2.left : Vector2.right;

            if (waypointObject != null)
                waypointObject.transform.position = nodePos;

            if (isCollidingLeftWall && moveDirX == Vector2.left)
                moveDirX = Vector2.zero;

            if (isCollidingRightWall && moveDirX == Vector2.right)
                moveDirX = Vector2.zero;

            if (distTo <= 0.15f)
            {
                if (currentPathIndex + 1 < currentPath.GetPathLength())
                {
                    currentPathIndex++;
                }
            }
            else
            {
                if (Mathf.Abs(transform.position.x - nodePos.x) >= 0.1f && direction.y <= 0.3f)
                {
                    // move left/right
                    rigidBodyComponent.RigidBody.AddImpulse(moveDirX * movementForce);
                }

                if (direction.y >= 0.3f && isGrounded)
                {
                    var velTmp = rigidBodyComponent.RigidBody.LinearVelocity;
                    velTmp.y = 0.0f;
                    velTmp.x = 0.0f;
                    rigidBodyComponent.RigidBody.LinearVelocity = velTmp;

                    rigidBodyComponent.RigidBody.AddImpulse(moveDirX * movementForce);
                    rigidBodyComponent.RigidBody.AddImpulse(Vector2.up * (jumpHeight * Mathf.Clamp01(distTo)) * rigidBodyComponent.RigidBody.Mass * 2.0f);
                }
            }

            // clamp X speed for the AI to its max speed.
            var vel = rigidBodyComponent.RigidBody.LinearVelocity;
            vel.x = Mathf.Clamp(vel.x, -maxSpeed, maxSpeed);
            rigidBodyComponent.RigidBody.LinearVelocity = vel;
        }

        public void OnGroundTriggerStay(ABCollider _collider)
        {
            if (_collider.RigidBody.GameObject.tag == "Coin") return;
            isGrounded = true;
        }

        public void OnGroundTriggerLeave(ABCollider _collider)
        {
            if (_collider.RigidBody.GameObject.tag == "Coin") return;
            isGrounded = false;
        }

        public void OnLeftWallTriggerStay(ABCollider _collider)
        {
            if (_collider.RigidBody.GameObject.tag == "Tile" || _collider.RigidBody.GameObject.tag == "BouncePad")
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
            if (_collider.RigidBody.GameObject.tag == "Tile" || _collider.RigidBody.GameObject.tag == "BouncePad")
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
