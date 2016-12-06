using System.Collections.Generic;
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


        public Path(PathNode _start, PathNode _end)
        {
            Valid = false;
            VectorPath = new List<Vector2>();
            NodePath = new List<PathNode>();
            StartNode = _start;
            EndNode = _end;
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
