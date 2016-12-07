using System.Collections.Generic;
using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.AI.Pathfinding
{
    public class Path
    {

        /// <summary>
        /// Is the generated path valid? I.e Does the path get from A to B successfully without issue.
        /// </summary>
        public bool Valid { get; private set; }

        /// <summary>
        /// List of vector positions for the path.
        /// </summary>
        public List<Vector2> VectorPath { get; private set; }

        /// <summary>
        /// List of nodes in the path.
        /// </summary>
        public List<PathNode> NodePath { get; private set; }

        /// <summary>
        /// Node the path starts from.
        /// </summary>
        public PathNode StartNode { get; private set; }

        /// <summary>
        /// Node the path ends at.
        /// </summary>
        public PathNode EndNode { get; private set; }

        /// <summary>
        /// The time taken in milliseconds to create this path.
        /// </summary>
        public float CreationTime { get; set; }


        public Path(PathNode _start, PathNode _end)
        {
            Valid = false;
            VectorPath = new List<Vector2>();
            NodePath = new List<PathNode>();
            StartNode = _start;
            EndNode = _end;

            if(StartNode.NodeType == PathNodeType.None)
            {
                FindClosestNodeToStart();
            }
        }

        /// <summary>
        /// Find the first availabe node below the given start node if the given start was a none type.
        /// </summary>
        private void FindClosestNodeToStart()
        {
            var scanY = StartNode.Y;

            while(scanY > 0)
            {
                if(World.Current.NavGraph.Nodes[StartNode.X, scanY].NodeType == PathNodeType.None)
                {
                    scanY--;
                    continue;
                }

                StartNode = World.Current.NavGraph.Nodes[StartNode.X, scanY];
                break;
            }
        }

        public void SetValid()
        {
            Valid = true;
        }

        /// <summary>
        /// Returns the amount of nodes the path travels through.
        /// </summary>
        /// <returns></returns>
        public int GetPathLength()
        {
            return NodePath != null ? NodePath.Count : 0;
        }
    }
}
