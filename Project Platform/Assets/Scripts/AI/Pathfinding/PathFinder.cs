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
        private Heuristic costHeuristic;

        /// <summary>
        /// Create a pathfinder instance for the given start and end position. Positions can be either world or grid space coordinates.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        public PathFinder(Vector2 _start, Vector2 _end, Heuristic _heuristic = Heuristic.Euclidean)
        {
            // Make sure the start and end points are in grid coordinates.
            Start = World.Current.WorldPointToGridPoint(_start);
            End = World.Current.WorldPointToGridPoint(_end);
            costHeuristic = _heuristic;
        }

        /// <summary>
        /// Finds and returns a path from start to end for this PathFinder instance.
        /// </summary>
        /// <returns></returns>
        public Path FindPath()
        {
            var path = new Path(World.Current.NavGraph.Nodes[(int)Start.x, (int)Start.y], World.Current.NavGraph.Nodes[(int)End.x, (int)End.y]);



            return path;
        }

        /// <summary>
        /// Returns the heuritic cost between 2 nodes.
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <returns></returns>
        private float GetCost(PathNode _a, PathNode _b)
        {
            if (costHeuristic == Heuristic.Euclidean)
            {
                return Vector2.Distance(new Vector2(_a.X, _a.Y), new Vector2(_b.X, _b.Y));
            }

            // Manhattan distance
            var dx = Mathf.Abs(_a.X - _b.X);
            var dy = Mathf.Abs(_a.Y - _b.Y);
            return dx + dy;
        }
    }
}
