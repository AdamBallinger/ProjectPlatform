using System.Collections.Generic;
using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.AI.Pathfinding
{
    public enum TrajectoryDirection
    {
        Left = -1,      // Invert left direction (-velocity)
        Right = 1
    }

    public class JumpTrajectory
    {

        /// <summary>
        /// Start point of the trajectory.
        /// </summary>
        public Vector2 StartPoint { get; private set; }

        /// <summary>
        /// Starting node for the trajectory.
        /// </summary>
        public PathNode StartNode { get; private set; }

        /// <summary>
        /// The direction for this trajectory. (left of right)
        /// </summary>
        private TrajectoryDirection Direction { get; set; }

        /// <summary>
        /// Jump height for this jump trajectory.
        /// </summary>
        public float JumpHeight { get; private set; }

        /// <summary>
        /// Speed for this jump trajectory.
        /// </summary>
        public float JumpSpeed { get; private set; }

        /// <summary>
        /// List of coordinates (Not tile bound) for each point of the trajectory.
        /// </summary>
        public List<Vector2> Trajectory { get; private set; }

        /// <summary>
        /// Path node this trajectory lands on (if any).
        /// </summary>
        public PathNode LandingNode { get; private set; }

        /// <summary>
        /// Max number of points to create when calculating a trajectory.
        /// </summary>
        private const int maxTrajectoryPoints = 25;

        /// <summary>
        /// Time to simulate between each trajectory point.
        /// </summary>
        private const float timeBetweenPoints = 0.2f;

        public JumpTrajectory(Vector2 _start, TrajectoryDirection _direction, float _jumpHeight, float _jumpSpeed)
        {
            Trajectory = new List<Vector2>();
            StartPoint = _start;
            Direction = _direction;
            JumpHeight = _jumpHeight;
            JumpSpeed = _jumpSpeed * (float)Direction;

            StartNode = World.Current.NavGraph.Nodes[(int)StartPoint.x, (int)StartPoint.y];
            
            CalculateTrajectory();
        }

        private void CalculateTrajectory()
        {
            Trajectory.Clear();

            // time between each point
            var time = 0.0f;

            for(var i = 0; i < maxTrajectoryPoints; i++)
            {
                var dx = JumpSpeed * time;
                var dy = JumpHeight * time - (World.Current.PhysicsWorld.Gravity.magnitude * time * time / 2.0f);
                var point = new Vector2(StartPoint.x + dx, StartPoint.y + dy);
                Trajectory.Add(point);
                time += timeBetweenPoints;
            }
        }

        /// <summary>
        /// Returns if this jump trajectory is valid (AI wont collide with other tiles before reaching a valid destination with a path node).
        /// Rules to determine a valid jump are that the tile any point is inside of must be an empty type as well as the tile above.
        /// </summary>
        /// <returns></returns>
        public bool IsValidJump()
        {
            foreach(var point in Trajectory)
            {
                var tileAtPoint = World.Current.GetTileAtWorldCoord(point);
                var tileAbovePoint = World.Current.GetTileAt(tileAtPoint.X, tileAtPoint.Y);

                if((tileAtPoint != null && tileAtPoint.Type == TileType.Empty)
                    && tileAbovePoint != null && tileAbovePoint.Type == TileType.Empty)
                {
                    var nodeAtPoint = World.Current.NavGraph.Nodes[tileAtPoint.X, tileAtPoint.Y];
                    if(nodeAtPoint.NodeType != PathNodeType.None)
                    {
                        LandingNode = nodeAtPoint;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
