using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Assets.Scripts.General;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.AI.Pathfinding
{
    public enum Heuristic
    {
        Manhattan = 0,
        Euclidean = 1
    }

    public class PathFinder
    {
        public Vector2 Start { get; private set; }
        public Vector2 End { get; private set; }

        private Action<Path> OnPatchCompleteCallback;

        private Path path;

        /// <summary>
        /// Heuristic function to use for calculating cost between nodes.
        /// </summary>
        private Heuristic heuristicFunction;

        private List<PathNode> closedList;
        private List<PathNode> openList;

        // Thread instance used for threaded pathfinding.
        private Thread finderThread;


        /// <summary>
        /// Create a pathfinder instance for the given start and end position. Positions can be either world or grid space coordinates.
        /// </summary>
        /// <param name="_pathCompleteCallback"></param>
        public PathFinder(Action<Path> _pathCompleteCallback)
        {
            heuristicFunction = PathfindingSettings.HeuristicFunction;
            closedList = new List<PathNode>();
            openList = new List<PathNode>();
            OnPatchCompleteCallback += _pathCompleteCallback;
        }

        /// <summary>
        /// Finds a path for this finder in a seperate thread.
        /// </summary>
        private void ThreadedSearch()
        {
            var watch = new Stopwatch();
            watch.Start();

            // Clear all node parents and costs from previous searches.
            ResetNodes();

            // Clear the closed and open lists from previous searches.
            closedList.Clear();
            openList.Clear();

            // This is a safety net for the while loop, incase something goes wrong and limits the search iterations the loop can perform.
            var maxSearchCount = 5000;
            var currentSearchCount = 0;

            // To start, add the starting node to the open list.
            openList.Add(path.StartNode);

            // Set the H cost for the start node.
            path.StartNode.H = GetHeuristicCost(path.StartNode);

            // While the open list has nodes to evaluate.
            while (openList.Count != 0)
            {
                // Failsafe check
                if (currentSearchCount >= maxSearchCount)
                {
                    Debug.LogWarning("FindPath terminated because it exceeded the maximum allowed search count. Something went wrong.");
                    break;
                }
                currentSearchCount++;

                // Get the node with the lowest F cost from the open list.
                var currentNode = GetLowestCostNodeFromOpenList();

                // If the node is the target node for the path, then the path has been found
                if (currentNode == path.EndNode)
                {
                    // Create the path by going back through each parent node from the current node.
                    RetracePath(path, currentNode);
                    path.SetValid();
                    watch.Stop();
                    path.CreationTime = watch.ElapsedMilliseconds;
                    break;
                }

                // If the node isn't the target then go through any connecting nodes
                foreach (var link in currentNode.NodeLinks)
                {
                    // If the node is in the closed list ignore it as its already been fully evaluated.
                    if (closedList.Contains(link.DestinationNode)) continue;

                    // If the node isn't in the open list already.
                    if(!openList.Contains(link.DestinationNode))
                    {
                        // Set its parent to the current node
                        link.DestinationNode.Parent = currentNode;

                        // Calculate G and H costs for the node. 
                        // G(n) = n.parent + movement cost from n to n.parent + euclidean distance from n to n.parent
                        // H(n) = distance from n to target node
                        link.DestinationNode.G = currentNode.G + link.LinkCost + DistanceBetween(link.DestinationNode, currentNode);
                        link.DestinationNode.H = GetHeuristicCost(link.DestinationNode);

                        // Add link destination to the open list
                        openList.Add(link.DestinationNode);
                    }
                    else
                    {
                        // if the node this link leads to is already on the open list, check to see if the nodes parent G cost 
                        //is higher than the current nodes.
                        if (link.DestinationNode.Parent.G > currentNode.G)
                        {
                            // if the parent of the current links destination has a higher G cost than the current node, then a cheaper route
                            // is present, so set the parent of the links destination node to the current node.
                            link.DestinationNode.Parent = currentNode;
                            // re-calculate costs for this node.
                            link.DestinationNode.G = currentNode.G + link.LinkCost + DistanceBetween(link.DestinationNode, currentNode);
                        }
                    }
                }

                // Once all linked nodes are evaluated, remove the current node from the open list, and add it to the closed list as its been
                // fully evaluated.
                openList.Remove(currentNode);
                closedList.Add(currentNode);
            }

            // Return the path. Checking the Valid property determines a successfull path.
            OnPatchCompleteCallback(path);

            // Terminate the search thread.
            finderThread.Abort();
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

            path = new Path(World.Current.NavGraph.Nodes[(int)Start.x, (int)Start.y], World.Current.NavGraph.Nodes[(int)End.x, (int)End.y]);

            finderThread = new Thread(ThreadedSearch);
            finderThread.Start();
        }

        /// <summary>
        /// Gets the cheapest F cost node from the open list.
        /// </summary>
        /// <returns></returns>
        private PathNode GetLowestCostNodeFromOpenList()
        {
            var cheapestNode = openList[0];

            foreach (var node in openList)
            {
                if (node.F < cheapestNode.F)
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

            while (_lastNode.Parent != null)
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
            for (var x = 0; x < World.Current.NavGraph.Width; x++)
            {
                for (var y = 0; y < World.Current.NavGraph.Height; y++)
                {
                    var node = World.Current.NavGraph.Nodes[x, y];
                    node.G = 0.0f;
                    node.H = 0.0f;
                    node.Parent = null;
                }
            }
        }

        /// <summary>
        /// Returns the heuritic (H) cost for the give node.
        /// </summary>
        /// <param name="_node"></param>
        /// <returns></returns>
        private float GetHeuristicCost(PathNode _node)
        {
            // Euclidean - Straight line cost from given node to paths target node. 
            if (heuristicFunction == Heuristic.Euclidean)
            {
                return DistanceBetween(_node, path.EndNode);
            }

            // Manhattan heuristic - Combined X and Y difference from given node to paths target node.
            var dx = Mathf.Abs(_node.X - path.EndNode.X);
            var dy = Mathf.Abs(_node.Y - path.EndNode.Y);
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
