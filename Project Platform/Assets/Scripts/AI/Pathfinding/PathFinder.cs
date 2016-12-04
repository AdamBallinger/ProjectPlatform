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

        /// <summary>
        /// Heuristic function to use for calculating cost between nodes.
        /// </summary>
        private Heuristic heuristicFunction;

        private List<PathNode> closedList;
        private List<PathNode> openList;


        /// <summary>
        /// Create a pathfinder instance for the given start and end position. Positions can be either world or grid space coordinates.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_heuristicFunction"></param>
        public PathFinder(Vector2 _start, Vector2 _end, Heuristic _heuristicFunction = Heuristic.Euclidean)
        {
            // Make sure the start and end points are in grid coordinates.
            Start = World.Current.WorldPointToGridPoint(_start);
            End = World.Current.WorldPointToGridPoint(_end);
            heuristicFunction = _heuristicFunction;
            closedList = new List<PathNode>();
            openList = new List<PathNode>();
        }

        /// <summary>
        /// Finds and returns a path from start to end for this PathFinder instance.
        /// </summary>
        /// <returns></returns>
        public Path FindPath()
        {
            var path = new Path(World.Current.NavGraph.Nodes[(int)Start.x, (int)Start.y], World.Current.NavGraph.Nodes[(int)End.x, (int)End.y]);
            var nodes = World.Current.NavGraph.Nodes;

            closedList.Clear();
            openList.Clear();

            var maxSearchCount = 1000;
            var currentSearchCount = 0;
            
            openList.Add(path.StartNode);

            while(openList.Count != 0)
            {
                if (currentSearchCount >= maxSearchCount) break;
                currentSearchCount++;


            }

            return path;
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

            // Manhattan distance
            var dx = Mathf.Abs(_a.X - _b.X);
            var dy = Mathf.Abs(_a.Y - _b.Y);
            return dx + dy;
        }

        /// <summary>
        /// Returns the G cost from a to b.
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <returns></returns>
        private float GetCost(PathNode _a, PathNode _b)
        {
            if (heuristicFunction == Heuristic.Euclidean)
            {
                return _a.G + Vector2.Distance(new Vector2(_a.X, _a.Y), new Vector2(_b.X, _b.Y));
            }

            // Manhattan distance
            var dx = Mathf.Abs(_a.X - _b.X);
            var dy = Mathf.Abs(_a.Y - _b.Y);
            return _a.G + (dx + dy);
        }
    }
}
