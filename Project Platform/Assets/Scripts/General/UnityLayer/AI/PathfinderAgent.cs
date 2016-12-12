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

        private Transform target;
        private Vector2 targetVec;

        public void Start()
        {
            groundCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnGroundTriggerStay);
            groundCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnGroundTriggerLeave);

            leftWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnLeftWallTriggerStay);
            leftWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnLeftWallTriggerLeave);

            rightWallCheck.Collider.CollisionListener.RegisterTriggerStayCallback(OnRightWallTriggerStay);
            rightWallCheck.Collider.CollisionListener.RegisterTriggerLeaveCallback(OnRightWallTriggerLeave);

            waypointObject = GameObject.FindGameObjectWithTag("AIWaypoint");
        }

        public void StartPathing(Vector2 _start, Transform _target)
        {
            pathFinder = new PathFinder(OnPathComplete);
            ClearPath();

            StopAllCoroutines();

            target = _target;

            pathFinder.FindPath(_start, target.position);

            StartCoroutine(UpdatePath());
        }

        public void StartPathing(Vector2 _start, Vector2 _end)
        {
            pathFinder = new PathFinder(OnPathComplete);
            ClearPath();

            StopAllCoroutines();

            targetVec = _end;

            pathFinder.FindPath(_start, targetVec);

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
                if(target != null)
                    pathFinder.FindPath(rigidBodyComponent.RigidBody.Position, target.position);
                else
                    pathFinder.FindPath(rigidBodyComponent.RigidBody.Position, targetVec);
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

            // Check if the current path has finished.
            if (currentPathIndex >= currentPath.GetPathLength())
            {
                ClearPath();
                rigidBodyComponent.RigidBody.LinearVelocity = Vector2.zero;
                return;
            }

            // Get node position
            var node = currentPath.NodePath[currentPathIndex];
            var nodePos = new Vector2(node.X, node.Y);

            // Get distance and direction from agent to node.
            var distTo = Vector2.Distance(transform.position, nodePos);
            var direction = nodePos - (Vector2)transform.position;

            // Get X movement direction
            var moveDirX = nodePos.x < transform.position.x ? Vector2.left : Vector2.right;

            // Set waypoint object to show current node being pathed towards if it isn't null (Only visible in level editor)
            if (waypointObject != null)
            {
                waypointObject.transform.position = nodePos;
            }

            // Dont allow movement to the left if ai is colliding with a wall to the left of it.
            if (isCollidingLeftWall && moveDirX == Vector2.left)
            {
                moveDirX = Vector2.zero;
            }

            // Same for the right.
            if (isCollidingRightWall && moveDirX == Vector2.right)
            {
                moveDirX = Vector2.zero;
            }

            // If the agent is more than 0.1 units away on the X axis and less than 0.4 units away on the Y axis then move left / right
            // Y check is to prevent moving on the X axis too soon when jumping.
            if (Mathf.Abs(direction.x) >= 0.1f && direction.y <= 0.4f)
            {
                // move left/right
                rigidBodyComponent.RigidBody.AddImpulse(moveDirX * movementForce);
            }

            // If the difference between the agent and the targer node is greater than 0.3f on the Y axis, then a jump is needed.
            // of if the difference on the Y is less than or equal to 0.3 and the X difference is greater than 1.75 jump over a gap.
            if ((direction.y >= 0.3f && isGrounded) || (Mathf.Abs(direction.x) > 1.75f && Mathf.Abs(direction.y) <= 0.3f && isGrounded))
            {
                // Reset x and y velocity or everything will break, especially jumping.
                var velTmp = rigidBodyComponent.RigidBody.LinearVelocity;
                velTmp.y = 0.0f;
                velTmp.x = 0.0f;
                rigidBodyComponent.RigidBody.LinearVelocity = velTmp;

                // Move slightly in the opposite direction if the distance on the x axis to the tile we want to jump to is greater than 1
                // (Prevents agent jumping and hitting its head on an above tile over and over)
                if(Mathf.Abs(direction.x) < 1.0f && moveDirX != Vector2.zero)
                {
                    rigidBodyComponent.RigidBody.AddImpulse(-moveDirX * movementForce * 2.0f);
                    return;
                }

                // Add a small impulse towards the node on the X axis to prevent infinite jumping issue for jumps larger than 1 tile on the x axis.
                rigidBodyComponent.RigidBody.AddImpulse(moveDirX * movementForce);
                // Add the jump impulse to the agent.
                rigidBodyComponent.RigidBody.AddImpulse(Vector2.up * jumpHeight * rigidBodyComponent.RigidBody.Mass * 2.0f);
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
