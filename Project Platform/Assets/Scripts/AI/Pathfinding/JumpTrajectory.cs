using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.Pathfinding
{
    public enum TrajectoryDirection
    {
        Left,
        Right
    }

    public class JumpTrajectory
    {

        /// <summary>
        /// Start point of the trajectory.
        /// </summary>
        public Vector2 StartPoint { get; private set; }

        /// <summary>
        /// The direction for this trajectory. (left of right)
        /// </summary>
        public TrajectoryDirection Direction { get; private set; }

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

        public JumpTrajectory(Vector2 _start, TrajectoryDirection _direction, float _jumpHeight, float _jumpSpeed)
        {
            Trajectory = new List<Vector2>();
            StartPoint = _start;
            Direction = _direction;
            JumpHeight = _jumpHeight;
            JumpSpeed = _jumpSpeed;
        }

        private void CalculateTrajectory()
        {
            Trajectory.Clear();

        }

        /// <summary>
        /// Returns if this jump trajectory is valid (AI wont collide with other tiles before reaching a valid destination with a path node).
        /// Rules to determine a valid jump are that the tile any point is inside of must be an empty type as well as the tile above.
        /// </summary>
        /// <returns></returns>
        public bool IsValidJump()
        {

            return false;
        }
    }
}
