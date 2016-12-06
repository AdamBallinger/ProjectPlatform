using System;
using System.Collections.Generic;
using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.AI.Pathfinding
{
    public enum Heuristic
    {
        Manhattan,
        Euclidean
    }

    public class PathFinder
    {
        public Vector2 Start { get; private set; }
        public Vector2 End { get; private set; }

        private Action<Path> OnPatchCompleteCallback = null;

        /// <summary>
        /// Heuristic function to use for calculating cost between nodes.
        /// </summary>
        private Heuristic heuristicFunction;

        private List<PathNode> closedList;
        private List<PathNode> openList;


        /// <summary>
        /// Create a pathfinder instance for the given start and end position. Positions can be either world or grid space coordinates.
        /// </summary>
        /// <param name="_pathCompleteCallback"></param>
        /// <param name="_heuristicFunction"></param>
        public PathFinder(Action<Path> _pathCompleteCallback, Heuristic _heuristicFunction = Heuristic.Manhattan)
        {
            heuristicFunction = _heuristicFunction;
            closedList = new List<PathNode>();
            openList = new List<PathNode>();
            OnPatchCompleteCallback += _pathCompleteCallback;
        }

        /// <summary>
        /// Finds and returns a path from start to end for this PathFinder instance using A* algorithm.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        public void FindPath(Vector2 _start, Vector2 _end)
        {
            // Make sure the start and end points are in grid coordinates.
            Start = World.Current.WorldPointToGridPoint(_start);
            End = World.Current.WorldPointToGridPoint(_end);

            var path = new Path(World.Current.NavGraph.Nodes[(int)Start.x, (int)Start.y], World.Current.NavGraph.Nodes[(int)End.x, (int)End.y]);
            
            ResetNodes();

            closedList.Clear();
            openList.Clear();

            var maxSearchCount = 5000;
            var currentSearchCount = 0;
            
            openList.Add(path.StartNode);
            path.StartNode.H = GetHeuristicCost(path.StartNode, path.EndNode);

            while(openList.Count != 0)
            {
                if (currentSearchCount >= maxSearchCount)
                {
                    Debug.LogWarning("FindPath terminated because it exceeded the maximum allowed search count. Something went wrong.");
                    break;
                }
                currentSearchCount++;

                var currentNode = GetLowestCostNodeFromOpenList();

                if(currentNode == path.EndNode)
                {
                    Debug.Log("Path has reached its target node.");
                    closedList.Add(currentNode);
                    RetracePath(path, currentNode);
                    path.SetValid();
                    OnPatchCompleteCallback(path);
                    break;
                }

                foreach(var link in currentNode.NodeLinks)
                {
                    if(closedList.Contains(link.DestinationNode)) continue;
                    
                    if(link.DestinationNode.Parent == null)
                    {
                        link.DestinationNode.G = link.LinkCost + DistanceBetween(currentNode, link.DestinationNode);
                        link.DestinationNode.Parent = currentNode;
                        link.DestinationNode.H = GetHeuristicCost(link.DestinationNode, path.EndNode);
                        openList.Add(link.DestinationNode);
                    }
                    else
                    {
                        if(link.LinkCost + DistanceBetween(currentNode, link.DestinationNode) < link.DestinationNode.G)
                        {
                            link.DestinationNode.Parent = currentNode;
                            link.DestinationNode.G = link.LinkCost + DistanceBetween(currentNode, link.DestinationNode);
                        }
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);
            }
        }

        /// <summary>
        /// Gets the cheapest F cost node from the open list.
        /// </summary>
        /// <returns></returns>
        private PathNode GetLowestCostNodeFromOpenList()
        {
            var cheapestNode = openList[0];

            foreach(var node in openList)
            {
                if(node.F <= cheapestNode.F)
                {
                    cheapestNode = node;
                }
            }

            return cheapestNode;
        }

        /// <summary>
        /// Retrace a path from the last node to create the path from start to end.
        /// </summary>
        /// <param name="_path"></param>
        /// <param name="_lastNode"></param>
        private void RetracePath(Path _path, PathNode _lastNode)
        {
            // Add end node to start of path.
            _path.NodePath.Insert(0, _path.EndNode);
            _path.VectorPath.Insert(0, new Vector2(_path.EndNode.X, _path.EndNode.Y));

            while(_lastNode.Parent != null)
            {
                // while the last node has a valid parent, add the nodes parent to the start of the path, then set last node to the parent.
                _path.NodePath.Insert(0, _lastNode.Parent);
                _path.VectorPath.Insert(0, new Vector2(_lastNode.Parent.X, _lastNode.Parent.Y));
                _lastNode = _lastNode.Parent;
            }
        }

        /// <summary>
        /// Reset any costs and parent references for nodes from previous path calculations.
        /// </summary>
        private void ResetNodes()
        {
            for(var x = 0; x < World.Current.NavGraph.Width; x++)
            {
                for(var y = 0; y < World.Current.NavGraph.Height; y++)
                {
                    var node = World.Current.NavGraph.Nodes[x, y];
                    node.G = 0.0f;
                    node.H = 0.0f;
                    node.Parent = null;
                }
            }
        }

        /// <summary>
        /// Returns the heuritic (H) cost between 2 nodes.
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <returns></returns>
        private float GetHeuristicCost(PathNode _a, PathNode _b)
        {
            if (heuristicFunction == Heuristic.Euclidean)
            {
                return Vector2.Distance(new Vector2(_a.X, _a.Y), new Vector2(_b.X, _b.Y));
            }

            // Manhattan heuristic
            var dx = Mathf.Abs(_a.X - _b.X);
            var dy = Mathf.Abs(_a.Y - _b.Y);
            return dx + dy;
        }

        /// <summary>
        /// Returns the direct distance between 2 given nodes.
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <returns></returns>
        private float DistanceBetween(PathNode _a, PathNode _b)
        {
            return Vector2.Distance(new Vector2(_a.X, _a.Y), new Vector2(_b.X, _b.Y));
        }
    }
}
