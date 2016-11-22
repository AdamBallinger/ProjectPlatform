using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI.Pathfinding
{
    public class JumpTrajectory
    {

        public Vector2 StartPoint { get; private set; }

        public float JumpHeight { get; private set; }

        public float JumpSpeed { get; private set; }

        public List<Vector2> trajectoryPoints;

        private float AiMinSpeed { get; set; }
        private float AiMinAcceleration { get; set; }

        public JumpTrajectory(Vector2 _start, float _jumpHeight, float _jumpSpeed, float _aiMinSpeed, float _aiMinAcceleration)
        {
            trajectoryPoints = new List<Vector2>();
            StartPoint = _start;
            JumpHeight = _jumpHeight;
            JumpSpeed = _jumpSpeed;

            AiMinSpeed = _aiMinSpeed;
            AiMinAcceleration = _aiMinAcceleration;
        }
    }
}
