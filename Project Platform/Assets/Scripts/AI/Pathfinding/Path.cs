﻿using System.Collections.Generic;
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

            if(EndNode.NodeType == PathNodeType.None)
            {
                EndNode = FindClosestPathableNode(_end);
            }
        }

        /// <summary>
        /// Find the first closest pathable node to given node.
        /// </summary>
        private PathNode FindClosestPathableNode(PathNode _node)
        {
            var nodes = World.Current.NavGraph.Nodes;
            var lowestDist = float.MaxValue;
            var closestNode = nodes[0, 0];

            foreach(var node in nodes)
            {
                if (node.NodeType == PathNodeType.None) continue;
                var startPos = new Vector2(_node.X, _node.Y);
                var nodePos = new Vector2(node.X, node.Y);
                var distToNode = Vector2.Distance(startPos, nodePos);
                if(distToNode < lowestDist)
                {
                    closestNode = node;
                    lowestDist = distToNode;
                }
            }

            return closestNode;
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
